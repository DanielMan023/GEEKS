using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GEEKS.Dto;
using GEEKS.Interfaces;
using GEEKS.Utils;
using System.Security.Claims;

namespace GEEKS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<OrderDTO>>> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.CreateOrderAsync(userId, createOrderDto);

                return Ok(new ServiceResponse<OrderDTO>
                {
                    Success = true,
                    Data = order,
                    Message = "Pedido creado exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = $"Error al crear el pedido: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<OrderDTO>>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound(new ServiceResponse<OrderDTO>
                    {
                        Success = false,
                        Message = "Pedido no encontrado"
                    });
                }

                return Ok(new ServiceResponse<OrderDTO>
                {
                    Success = true,
                    Data = order,
                    Message = "Pedido obtenido exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = $"Error al obtener el pedido: {ex.Message}"
                });
            }
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<ServiceResponse<List<OrderDTO>>>> GetMyOrders()
        {
            try
            {
                var userId = GetCurrentUserId();
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);

                return Ok(new ServiceResponse<List<OrderDTO>>
                {
                    Success = true,
                    Data = orders,
                    Message = "Pedidos obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<OrderDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener los pedidos: {ex.Message}"
                });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<OrderDTO>>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                return Ok(new ServiceResponse<List<OrderDTO>>
                {
                    Success = true,
                    Data = orders,
                    Message = "Pedidos obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<OrderDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener los pedidos: {ex.Message}"
                });
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<OrderDTO>>>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);

                return Ok(new ServiceResponse<List<OrderDTO>>
                {
                    Success = true,
                    Data = orders,
                    Message = "Pedidos obtenidos exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<List<OrderDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener los pedidos: {ex.Message}"
                });
            }
        }

        [HttpGet("order-number/{orderNumber}")]
        public async Task<ActionResult<ServiceResponse<OrderDTO>>> GetOrderByOrderNumber(string orderNumber)
        {
            try
            {
                var order = await _orderService.GetOrderByOrderNumberAsync(orderNumber);

                return Ok(new ServiceResponse<OrderDTO>
                {
                    Success = true,
                    Data = order,
                    Message = "Pedido obtenido exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = $"Error al obtener el pedido: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<OrderDTO>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDTO updateOrderStatusDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);

                return Ok(new ServiceResponse<OrderDTO>
                {
                    Success = true,
                    Data = order,
                    Message = "Estado del pedido actualizado exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<OrderDTO>
                {
                    Success = false,
                    Message = $"Error al actualizar el estado del pedido: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);

                if (!result)
                {
                    return NotFound(new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Pedido no encontrado"
                    });
                }

                return Ok(new ServiceResponse<bool>
                {
                    Success = true,
                    Data = result,
                    Message = "Pedido eliminado exitosamente"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error al eliminar el pedido: {ex.Message}"
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Usuario no autenticado");
        }
    }
}
