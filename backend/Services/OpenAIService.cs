using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GEEKS.Interfaces;

namespace GEEKS.Services
{
    public interface IOpenAIService
    {
        Task<string> GetChatResponseAsync(string userMessage, string context);
        Task<string> GenerateProductDescriptionAsync(string productName, string category);
    }

    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            
            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key no configurada");
            }
            
            // Configurar headers para OpenAI
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GEEKS-Chatbot/1.0");
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string context)
        {
            try
            {
                _logger.LogInformation("Enviando mensaje a ChatGPT: {Message}", userMessage);
                
                var systemPrompt = BuildSystemPrompt(context);
                
                // Crear request para OpenAI API
                var request = new
                {
                    model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    max_tokens = int.Parse(_configuration["OpenAI:MaxTokens"] ?? "500"),
                    temperature = float.Parse(_configuration["OpenAI:Temperature"] ?? "0.7")
                };
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // Llamar a OpenAI API
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
                    
                    var aiResponse = openAIResponse?.choices?[0]?.message?.content;
                    _logger.LogInformation("Respuesta de ChatGPT recibida: {Response}", aiResponse);
                    
                    return aiResponse ?? "No pude generar una respuesta. ¿Puedes intentar de nuevo?";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error de OpenAI API: {Status} - {Content}", response.StatusCode, errorContent);
                    return "Lo siento, estoy teniendo problemas técnicos con mi IA. ¿Puedes intentar de nuevo?";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo respuesta de ChatGPT");
                return "Lo siento, estoy teniendo problemas técnicos con mi IA. ¿Puedes intentar de nuevo?";
            }
        }

        public async Task<string> GenerateProductDescriptionAsync(string productName, string category)
        {
            try
            {
                _logger.LogInformation("Generando descripción con ChatGPT para: {Product} en {Category}", productName, category);
                
                var prompt = $"Genera una descripción atractiva y profesional para un producto llamado '{productName}' de la categoría '{category}'. " +
                           "La descripción debe ser persuasiva, destacar beneficios y ser apropiada para un e-commerce. " +
                           "Máximo 100 palabras. Responde en español.";

                var request = new
                {
                    model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = "Eres un experto en marketing digital y copywriting para e-commerce." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 200,
                    temperature = 0.8f
                };
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
                    
                    var description = openAIResponse?.choices?[0]?.message?.content;
                    _logger.LogInformation("Descripción generada con ChatGPT: {Description}", description);
                    
                    return description ?? $"Descripción del producto {productName} en la categoría {category}.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error de OpenAI API: {Status} - {Content}", response.StatusCode, errorContent);
                    return $"Descripción del producto {productName} en la categoría {category}.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando descripción de producto con ChatGPT");
                return $"Descripción del producto {productName} en la categoría {category}.";
            }
        }

        private string BuildSystemPrompt(string context)
        {
            return $@"Eres un asistente virtual experto en e-commerce de GEEKS, una tienda de tecnología y productos gaming.

CONTEXTO ACTUAL: {context}

INSTRUCCIONES:
- Responde de forma natural, amigable y profesional en español
- Sé útil y específico con las respuestas
- Si te preguntan sobre productos, sugiere opciones relevantes
- Mantén un tono conversacional pero informativo
- Si no estás seguro de algo, sugiere alternativas útiles
- Usa emojis ocasionalmente para hacer la conversación más amigable

CAPACIDADES:
- Búsqueda de productos
- Información sobre categorías
- Ayuda con el proceso de compra
- Recomendaciones personalizadas
- Soporte técnico básico

Responde de manera natural y útil, como un asistente de tienda real.";
        }
    }

    // Clases para deserializar la respuesta de OpenAI
    public class OpenAIResponse
    {
        public Choice[] choices { get; set; } = Array.Empty<Choice>();
    }

    public class Choice
    {
        public Message message { get; set; } = new Message();
    }

    public class Message
    {
        public string content { get; set; } = string.Empty;
    }
}
