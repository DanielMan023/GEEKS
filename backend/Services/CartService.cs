using Microsoft.EntityFrameworkCore;
using GEEKS.Data;
using GEEKS.Dto;
using GEEKS.Interfaces;
using GEEKS.Models;

namespace GEEKS.Services
{
    public class CartService : ICartService
    {
        private readonly DBContext _context;

        public CartService(DBContext context)
        {
            _context = context;
        }

        public async Task<CartDTO> GetCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Crear carrito si no existe
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAtDateTime = DateTime.UtcNow,
                    UpdatedAtDateTime = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return MapToCartDTO(cart);
        }

        public async Task<CartDTO> AddToCartAsync(int userId, AddToCartDTO addToCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == addToCartDto.ProductId);

            if (product == null)
                throw new ArgumentException("Producto no encontrado");

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == addToCartDto.ProductId);

            if (existingItem != null)
            {
                // Actualizar cantidad si el item ya existe
                existingItem.Quantity += addToCartDto.Quantity;
                existingItem.UpdatedAtDateTime = DateTime.UtcNow;
            }
            else
            {
                // Crear nuevo item
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    UnitPrice = product.DiscountPrice ?? product.Price,
                    CreatedAtDateTime = DateTime.UtcNow,
                    UpdatedAtDateTime = DateTime.UtcNow
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Recargar el carrito con los items actualizados
            var updatedCart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            return MapToCartDTO(updatedCart!);
        }

        public async Task<CartDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO updateCartItemDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.Id == updateCartItemDto.CartItemId);

            if (cartItem == null)
                throw new ArgumentException("Item del carrito no encontrado");

            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.UpdatedAtDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToCartDTO(cart);
        }

        public async Task<CartDTO> RemoveFromCartAsync(int userId, RemoveFromCartDTO removeFromCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.Id == removeFromCartDto.CartItemId);

            if (cartItem == null)
                throw new ArgumentException("Item del carrito no encontrado");

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return MapToCartDTO(cart);
        }

        public async Task<CartDTO> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return MapToCartDTO(cart);
        }

        public async Task<bool> CartExistsAsync(int userId)
        {
            return await _context.Carts
                .AnyAsync(c => c.UserId == userId);
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAtDateTime = DateTime.UtcNow,
                    UpdatedAtDateTime = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private CartDTO MapToCartDTO(Cart cart)
        {
            return new CartDTO
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CartItems = cart.CartItems
                    .Select(ci => new CartItemDTO
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        ProductImage = ci.Product.MainImage ?? "",
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Subtotal = ci.Subtotal
                    }).ToList(),
                Total = cart.Total,
                TotalItems = cart.TotalItems
            };
        }
    }
}
