using Microsoft.AspNetCore.Mvc;
using GEEKS.Dto;
using GEEKS.Interfaces;

namespace GEEKS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(registerRequest.Email))
                {
                    return BadRequest(new { message = "El email es requerido" });
                }

                if (string.IsNullOrEmpty(registerRequest.Password))
                {
                    return BadRequest(new { message = "La contraseña es requerida" });
                }

                if (registerRequest.Password.Length < 6)
                {
                    return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres" });
                }

                var result = await _authService.Register(registerRequest);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el registro");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.Email))
                {
                    return BadRequest(new { message = "El email es requerido" });
                }

                if (string.IsNullOrEmpty(loginRequest.Password))
                {
                    return BadRequest(new { message = "La contraseña es requerida" });
                }

                var result = await _authService.Login(loginRequest);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el login");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth-token");
            return Ok(new { message = "Sesión cerrada exitosamente" });
        }

        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            // Si llegamos aquí, significa que el token es válido
            // porque el middleware de autenticación ya lo validó
            return Ok(new { message = "Token válido", valid = true });
        }
    }
}
