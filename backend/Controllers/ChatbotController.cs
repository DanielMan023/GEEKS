using Microsoft.AspNetCore.Mvc;
using GEEKS.Dto;
using GEEKS.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GEEKS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Solo usuarios autenticados pueden usar el chatbot
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(IChatbotService chatbotService, ILogger<ChatbotController> logger)
        {
            _chatbotService = chatbotService;
            _logger = logger;
        }

        [HttpPost("chat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ChatbotResponseDTO>> Chat([FromBody] ChatbotMessageDTO message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message))
                {
                    return BadRequest(new { message = "El mensaje es requerido" });
                }

                // Obtener el ID del usuario autenticado
                var userId = GetCurrentUserId();
                message.UserId = userId;

                var response = await _chatbotService.ProcessMessageAsync(message);
                
                _logger.LogInformation("Chatbot procesó mensaje: {Message} -> Intent: {Intent}, Confidence: {Confidence}", 
                    message.Message, response.Intent, response.Confidence);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el chatbot");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("recommendations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ProductRecommendationDTO>>> GetRecommendations([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return BadRequest(new { message = "La consulta es requerida" });
                }

                var userId = GetCurrentUserId();
                var recommendations = await _chatbotService.GetProductRecommendationsAsync(query, userId);
                
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo recomendaciones");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("context")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChatbotContextDTO>> GetContext()
        {
            try
            {
                var userId = GetCurrentUserId();
                var context = await _chatbotService.GetChatbotContextAsync(userId);
                
                return Ok(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo contexto del chatbot");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("generate-description")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GenerateDescription([FromBody] GenerateDescriptionDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ProductName) || string.IsNullOrEmpty(request.Category))
                {
                    return BadRequest(new { message = "El nombre del producto y la categoría son requeridos" });
                }

                var description = await _chatbotService.GenerateProductDescriptionAsync(request.ProductName, request.Category);
                
                return Ok(new { description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando descripción");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("health")]
        [AllowAnonymous] // Endpoint público para verificar estado del chatbot
        public ActionResult<object> Health()
        {
            return Ok(new { 
                status = "healthy", 
                service = "GEEKS Chatbot", 
                timestamp = DateTime.UtcNow,
                features = new[] {
                    "Intent Detection",
                    "Product Recommendations", 
                    "Context Awareness",
                    "Quick Replies",
                    "Natural Language Processing"
                }
            });
        }

        private int? GetCurrentUserId()
        {
            // Extraer el ID del usuario del token JWT
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }

    // DTO para generar descripciones
    public class GenerateDescriptionDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
