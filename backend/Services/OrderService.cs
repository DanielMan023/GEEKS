using Microsoft.EntityFrameworkCore;
using GEEKS.Data;
using GEEKS.Dto;
using GEEKS.Interfaces;
using GEEKS.Models;

namespace GEEKS.Services
{
    public class OrderService : IOrderService
    {
        private readonly DBContext _context;

        public OrderService(DBContext context)
        {
            _context = context;
        }

        public async Task<OrderDTO> CreateOrderAsync(int userId, CreateOrderDTO createOrderDto)
        {
            // Obtener el carrito del usuario
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("El carrito está vacío");
            }

            // Validar stock de todos los productos antes de procesar
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

                if (product == null)
                {
                    throw new ArgumentException($"Producto con ID {cartItem.ProductId} no encontrado");
                }

                if (cartItem.Quantity > product.Stock)
                {
                    throw new ArgumentException($"Stock insuficiente para el producto '{product.Name}'. Disponible: {product.Stock}, Solicitado: {cartItem.Quantity}");
                }
            }

            // Descontar stock de todos los productos
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

                if (product != null)
                {
                    product.Stock -= cartItem.Quantity;
                    product.UpdatedAtDateTime = DateTime.UtcNow;
                }
            }

            // Generar número de pedido único
            var orderNumber = GenerateOrderNumber();

            // Crear el pedido
            var order = new Order
            {
                UserId = userId,
                OrderNumber = orderNumber,
                Status = "Pending",
                Total = cart.Total,
                CustomerName = createOrderDto.CustomerName,
                CustomerEmail = createOrderDto.CustomerEmail,
                CustomerPhone = createOrderDto.CustomerPhone,
                ShippingAddress = createOrderDto.ShippingAddress,
                City = createOrderDto.City,
                ZipCode = createOrderDto.ZipCode,
                PaymentMethod = createOrderDto.PaymentMethod,
                Notes = createOrderDto.Notes,
                CreatedAtDateTime = DateTime.UtcNow,
                UpdatedAtDateTime = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Guardar primero para obtener el ID

            // Crear los items del pedido
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    ProductImage = cartItem.Product.MainImage,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    CreatedAtDateTime = DateTime.UtcNow,
                    UpdatedAtDateTime = DateTime.UtcNow
                };

                _context.OrderItems.Add(orderItem);
            }

            // Limpiar el carrito
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Recargar el pedido con los items
            var createdOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return MapToOrderDTO(createdOrder!);
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order != null ? MapToOrderDTO(order) : null;
        }

        public async Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAtDateTime)
                .ToListAsync();

            return orders.Select(MapToOrderDTO).ToList();
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAtDateTime)
                .ToListAsync();

            return orders.Select(MapToOrderDTO).ToList();
        }

        public async Task<OrderDTO> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new ArgumentException("Pedido no encontrado");
            }

            var oldStatus = order.Status;
            order.Status = updateOrderStatusDto.Status;
            order.UpdatedAtDateTime = DateTime.UtcNow;

            // Actualizar fechas según el estado
            if (updateOrderStatusDto.Status == "Shipped" && oldStatus != "Shipped")
            {
                order.ShippedDate = DateTime.UtcNow;
            }
            else if (updateOrderStatusDto.Status == "Delivered" && oldStatus != "Delivered")
            {
                order.DeliveredDate = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(updateOrderStatusDto.Notes))
            {
                order.Notes = updateOrderStatusDto.Notes;
            }

            await _context.SaveChangesAsync();

            return MapToOrderDTO(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            // Solo permitir eliminar pedidos pendientes
            if (order.Status != "Pending")
            {
                throw new InvalidOperationException("Solo se pueden eliminar pedidos pendientes");
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderDTO>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAtDateTime)
                .ToListAsync();

            return orders.Select(MapToOrderDTO).ToList();
        }

        public async Task<OrderDTO> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                throw new ArgumentException("Pedido no encontrado");
            }

            return MapToOrderDTO(order);
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }

        private OrderDTO MapToOrderDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                Total = order.Total,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                City = order.City,
                ZipCode = order.ZipCode,
                PaymentMethod = order.PaymentMethod,
                Notes = order.Notes,
                CreatedAtDateTime = order.CreatedAtDateTime,
                ShippedDate = order.ShippedDate,
                DeliveredDate = order.DeliveredDate,
                TotalItems = order.TotalItems,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }
    }
}
