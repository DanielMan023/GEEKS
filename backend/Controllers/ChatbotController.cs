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

                // Obtener el servicio de Gemini directamente
                var geminiService = HttpContext.RequestServices.GetRequiredService<IGeminiService>();
                
                // Probar directamente con Gemini Pro
                var aiResponse = await geminiService.GetChatResponseAsync(message.Message, "Test directo");
                
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

        [HttpGet("diagnose-gemini")]
        [AllowAnonymous] // Endpoint público para diagnosticar Gemini
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> DiagnoseGemini()
        {
            try
            {
                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                
                var apiKey = configuration["GoogleCloud:ApiKey"];
                var model = configuration["GoogleCloud:Model"];
                var maxTokens = configuration["GoogleCloud:MaxTokens"];
                var temperature = configuration["GoogleCloud:Temperature"];
                var projectId = configuration["GoogleCloud:ProjectId"];
                var location = configuration["GoogleCloud:Location"];
                
                var hasApiKey = !string.IsNullOrEmpty(apiKey);
                var apiKeyLength = hasApiKey ? apiKey.Length : 0;
                var apiKeyStart = hasApiKey ? apiKey.Substring(0, Math.Min(10, apiKey.Length)) : "";
                var apiKeyEnd = hasApiKey ? apiKey.Substring(Math.Max(0, apiKey.Length - 10)) : "";
                
                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    geminiConfig = new
                    {
                        hasApiKey = hasApiKey,
                        apiKeyLength = apiKeyLength,
                        apiKeyStart = apiKeyStart,
                        apiKeyEnd = apiKeyEnd,
                        model = model,
                        maxTokens = maxTokens,
                        temperature = temperature,
                        projectId = projectId,
                        location = location
                    },
                    message = "Diagnóstico de configuración de Gemini"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error diagnosticando Gemini");
                return StatusCode(500, new { 
                    message = "Error diagnosticando Gemini", 
                    error = ex.Message
                });
            }
        }



        [HttpGet("test-gemini-direct")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> TestGeminiDirect([FromQuery] string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    return BadRequest(new { message = "El mensaje es requerido" });
                }

                _logger.LogInformation("=== TEST GEMINI DIRECT INICIADO ===");
                _logger.LogInformation("Mensaje recibido: {Message}", message);

                // Obtener el servicio de Gemini directamente
                var geminiService = HttpContext.RequestServices.GetRequiredService<IGeminiService>();
                _logger.LogInformation("GeminiService obtenido: {Service}", geminiService != null);
                
                // Probar directamente con Gemini Pro
                _logger.LogInformation("Llamando a GetChatResponseAsync...");
                var aiResponse = await geminiService.GetChatResponseAsync(message, "Test directo");
                _logger.LogInformation("Respuesta recibida: {Response}", aiResponse);
                
                return Ok(new
                {
                    originalMessage = message,
                    aiResponse = aiResponse,
                    timestamp = DateTime.UtcNow,
                    aiWorking = true,
                    message = "Gemini Pro está funcionando correctamente",
                    testType = "Direct Google Cloud API call"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando Gemini Pro directamente");
                return StatusCode(500, new { message = $"Error probando Gemini Pro: {ex.Message}", aiWorking = false });
            }
        }

        [HttpPost("test-chat")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ChatbotResponseDTO>> TestChat([FromBody] ChatbotMessageDTO message)
        {
            try
            {
                if (string.IsNullOrEmpty(message.Message))
                {
                    return BadRequest(new { message = "El mensaje es requerido" });
                }

                // Simular un usuario ID para la prueba
                message.UserId = 1;

                var response = await _chatbotService.ProcessMessageAsync(message);
                
                _logger.LogInformation("Test Chat procesó mensaje: {Message} -> Intent: {Intent}, Confidence: {Confidence}", 
                    message.Message, response.Intent, response.Confidence);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el test chat");
                return StatusCode(500, new { message = "Error interno del servidor" });
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
