using Microsoft.AspNetCore.Mvc;
using GEEKS.Dto;
using GEEKS.Interfaces;
using GEEKS.Services;
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
                    "Natural Language Processing",
                    "Gemini Pro Integration"
                }
            });
        }

        [HttpPost("test-ai")]
        [AllowAnonymous] // Endpoint público para probar IA
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> TestAI([FromBody] ChatbotMessageDTO message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message))
                {
                    return BadRequest(new { message = "El mensaje es requerido" });
                }

                // Probar directamente el servicio de IA
                var aiResponse = await _chatbotService.ProcessMessageAsync(message);
                
                return Ok(new
                {
                    originalMessage = message.Message,
                    aiResponse = aiResponse.Message,
                    intent = aiResponse.Intent,
                    confidence = aiResponse.Confidence,
                    timestamp = DateTime.UtcNow,
                    aiWorking = true,
                    message = "Gemini Pro está funcionando correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando IA");
                return StatusCode(500, new { 
                    message = "Error probando IA", 
                    error = ex.Message,
                    aiWorking = false,
                    details = "Gemini Pro no está funcionando"
                });
            }
        }

        [HttpPost("test-openai-direct")]
        [AllowAnonymous] // Endpoint público para probar OpenAI directamente
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> TestOpenAIDirect([FromBody] ChatbotMessageDTO message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message))
                {
                    return BadRequest(new { message = "El mensaje es requerido" });
                }

                // Obtener el servicio de OpenAI directamente
                var openAIService = HttpContext.RequestServices.GetRequiredService<IOpenAIService>();
                
                // Probar directamente con OpenAI
                var aiResponse = await openAIService.GetChatResponseAsync(message.Message, "Test directo");
                
                return Ok(new
                {
                    originalMessage = message.Message,
                    aiResponse = aiResponse,
                    timestamp = DateTime.UtcNow,
                    aiWorking = true,
                    message = "Gemini Pro está funcionando correctamente",
                    testType = "Direct Google Cloud API call"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando OpenAI directamente");
                return StatusCode(500, new { 
                    message = "Error probando OpenAI directamente", 
                    error = ex.Message,
                    aiWorking = false,
                    details = "Gemini Pro no está funcionando",
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("diagnose-openai")]
        [AllowAnonymous] // Endpoint público para diagnosticar OpenAI
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> DiagnoseOpenAI()
        {
            try
            {
                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                
                var apiKey = configuration["OpenAI:ApiKey"];
                var model = configuration["OpenAI:Model"];
                var maxTokens = configuration["OpenAI:MaxTokens"];
                var temperature = configuration["OpenAI:Temperature"];
                
                var hasApiKey = !string.IsNullOrEmpty(apiKey);
                var apiKeyLength = hasApiKey ? apiKey.Length : 0;
                var apiKeyStart = hasApiKey ? apiKey.Substring(0, Math.Min(10, apiKey.Length)) : "";
                var apiKeyEnd = hasApiKey ? apiKey.Substring(Math.Max(0, apiKey.Length - 10)) : "";
                
                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    openAIConfig = new
                    {
                        hasApiKey = hasApiKey,
                        apiKeyLength = apiKeyLength,
                        apiKeyStart = apiKeyStart,
                        apiKeyEnd = apiKeyEnd,
                        model = model,
                        maxTokens = maxTokens,
                        temperature = temperature
                    },
                    message = "Diagnóstico de configuración de OpenAI"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error diagnosticando OpenAI");
                return StatusCode(500, new { 
                    message = "Error diagnosticando OpenAI", 
                    error = ex.Message
                });
            }
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
