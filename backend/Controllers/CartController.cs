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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<CartDTO>>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartAsync(userId);

                return Ok(new ServiceResponse<CartDTO>
                {
                    Success = true,
                    Data = cart,
                    Message = "Carrito obtenido exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = $"Error al obtener el carrito: {ex.Message}"
                });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<CartDTO>>> AddToCart([FromBody] AddToCartDTO addToCartDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.AddToCartAsync(userId, addToCartDto);

                return Ok(new ServiceResponse<CartDTO>
                {
                    Success = true,
                    Data = cart,
                    Message = "Producto agregado al carrito exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = $"Error al agregar producto al carrito: {ex.Message}"
                });
            }
        }

        [HttpPut("update")]
        public async Task<ActionResult<ServiceResponse<CartDTO>>> UpdateCartItem([FromBody] UpdateCartItemDTO updateCartItemDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.UpdateCartItemAsync(userId, updateCartItemDto);

                return Ok(new ServiceResponse<CartDTO>
                {
                    Success = true,
                    Data = cart,
                    Message = "Cantidad actualizada exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = $"Error al actualizar cantidad: {ex.Message}"
                });
            }
        }

        [HttpDelete("remove")]
        public async Task<ActionResult<ServiceResponse<CartDTO>>> RemoveFromCart([FromBody] RemoveFromCartDTO removeFromCartDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.RemoveFromCartAsync(userId, removeFromCartDto);

                return Ok(new ServiceResponse<CartDTO>
                {
                    Success = true,
                    Data = cart,
                    Message = "Producto eliminado del carrito exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = $"Error al eliminar producto del carrito: {ex.Message}"
                });
            }
        }

        [HttpDelete("clear")]
        public async Task<ActionResult<ServiceResponse<CartDTO>>> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.ClearCartAsync(userId);

                return Ok(new ServiceResponse<CartDTO>
                {
                    Success = true,
                    Data = cart,
                    Message = "Carrito vaciado exitosamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse<CartDTO>
                {
                    Success = false,
                    Message = $"Error al vaciar el carrito: {ex.Message}"
                });
            }
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ServiceResponse<OrderDTO>>> ProcessCheckout([FromBody] CreateOrderDTO createOrderDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Crear el pedido (esto incluye la validación del carrito y el descuento de stock)
                var order = await _orderService.CreateOrderAsync(userId, createOrderDto);

                return Ok(new ServiceResponse<OrderDTO>
                {
                    Success = true,
                    Data = order,
                    Message = "Compra procesada exitosamente"
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
                    Message = $"Error al procesar la compra: {ex.Message}"
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
