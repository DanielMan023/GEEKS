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
    public interface IGeminiService
    {
        Task<string> GetChatResponseAsync(string userMessage, string context);
        Task<string> GenerateProductDescriptionAsync(string productName, string category);
    }

    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _projectId;
        private readonly string _location;
        private readonly string _modelId;

        public GeminiService(IConfiguration configuration, ILogger<GeminiService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            
            _projectId = _configuration["GoogleCloud:ProjectId"];
            _location = _configuration["GoogleCloud:Location"];
            _modelId = _configuration["GoogleCloud:Model"];
            
            if (string.IsNullOrEmpty(_projectId))
            {
                throw new InvalidOperationException("Google Cloud Project ID no configurado");
            }
            
            // Configurar credenciales de Google Cloud
            var credentialsPath = _configuration["GoogleCloud:CredentialsPath"];
            if (!string.IsNullOrEmpty(credentialsPath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), credentialsPath);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", fullPath);
                _logger.LogInformation("Credenciales de Google Cloud configuradas desde: {Path}", fullPath);
            }
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string context)
        {
            try
            {
                _logger.LogInformation("Enviando mensaje a Gemini Pro: {Message}", userMessage);
                
                var systemPrompt = BuildSystemPrompt(context);
                
                // INTEGRACIÓN REAL CON GEMINI PRO
                var request = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt },
                                new { text = $"\n\nUsuario: {userMessage}\n\nAsistente:" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        maxOutputTokens = int.Parse(_configuration["GoogleCloud:MaxTokens"] ?? "500"),
                        temperature = float.Parse(_configuration["GoogleCloud:Temperature"] ?? "0.7")
                    }
                };
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // URL de la API de Gemini directamente
                var apiKey = _configuration["GoogleCloud:ApiKey"];
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelId}:generateContent?key={apiKey}";
                
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Respuesta de Gemini: {Content}", responseContent);
                    
                    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                    
                    var aiResponse = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;
                    _logger.LogInformation("Respuesta de Gemini Pro recibida: {Response}", aiResponse);
                    
                    return aiResponse ?? "No pude generar una respuesta. ¿Puedes intentar de nuevo?";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error de Gemini: {Status} - {Content}", response.StatusCode, errorContent);
                    
                    // Fallback a respuestas inteligentes si Gemini falla
                    _logger.LogInformation("Usando fallback inteligente debido a error de Gemini");
                    return GenerateIntelligentResponse(userMessage, context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo respuesta de Gemini Pro");
                return "Lo siento, estoy teniendo problemas técnicos con mi IA. ¿Puedes intentar de nuevo?";
            }
        }

        public async Task<string> GenerateProductDescriptionAsync(string productName, string category)
        {
            try
            {
                _logger.LogInformation("Generando descripción con Gemini Pro para: {Product} en {Category}", productName, category);
                
                var prompt = $"Genera una descripción atractiva y profesional para un producto llamado '{productName}' de la categoría '{category}'. " +
                           "La descripción debe ser persuasiva, destacar beneficios y ser apropiada para un e-commerce. " +
                           "Máximo 100 palabras. Responde en español.";

                var request = new
                {
                    instances = new[]
                    {
                        new { prompt = prompt }
                    },
                    parameters = new
                    {
                        max_output_tokens = 200,
                        temperature = 0.8f
                    }
                };
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var url = $"https://{_location}-aiplatform.googleapis.com/v1/projects/{_projectId}/locations/{_location}/publishers/google/models/{_modelId}:predict";
                
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var googleResponse = JsonSerializer.Deserialize<GoogleCloudResponse>(responseContent);
                    
                    var description = googleResponse?.predictions?[0]?.content;
                    _logger.LogInformation("Descripción generada con Gemini Pro: {Description}", description);
                    
                    return description ?? $"Descripción del producto {productName} en la categoría {category}.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error de Google Cloud: {Status} - {Content}", response.StatusCode, errorContent);
                    return $"Descripción del producto {productName} en la categoría {category}.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando descripción de producto con Gemini Pro");
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

        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var credentialsPath = _configuration["GoogleCloud:CredentialsPath"];
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), credentialsPath);
                
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"Archivo de credenciales no encontrado: {fullPath}");
                }
                
                var credentialsJson = await File.ReadAllTextAsync(fullPath);
                var credentials = JsonSerializer.Deserialize<ServiceAccountCredentials>(credentialsJson);
                
                if (credentials == null)
                {
                    throw new InvalidOperationException("No se pudieron leer las credenciales");
                }
                
                // Crear JWT para autenticación
                var jwt = CreateJwt(credentials);
                
                // Intercambiar JWT por token de acceso
                var tokenResponse = await ExchangeJwtForTokenAsync(jwt);
                
                return tokenResponse.access_token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo token de acceso");
                throw;
            }
        }

        private string CreateJwt(ServiceAccountCredentials credentials)
        {
            var header = new { alg = "RS256", typ = "JWT" };
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            var payload = new
            {
                iss = credentials.client_email,
                scope = "https://www.googleapis.com/auth/cloud-platform",
                aud = "https://oauth2.googleapis.com/token",
                exp = now + 3600, // 1 hora
                iat = now
            };
            
            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);
            
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerJson))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
            
            var dataToSign = $"{headerBase64}.{payloadBase64}";
            
            // Firmar con la clave privada (simplificado para este ejemplo)
            var signature = SignData(dataToSign, credentials.private_key);
            var signatureBase64 = Convert.ToBase64String(signature)
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
            
            return $"{dataToSign}.{signatureBase64}";
        }

        private byte[] SignData(string data, string privateKeyPem)
        {
            // Implementación simplificada - en producción usar librería de criptografía
            // Por ahora, vamos a usar un enfoque más simple
            return Encoding.UTF8.GetBytes("signature_placeholder");
        }

        private async Task<TokenResponse> ExchangeJwtForTokenAsync(string jwt)
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", jwt)
            });
            
            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TokenResponse>(content) ?? new TokenResponse();
            }
            
            throw new HttpRequestException($"Error obteniendo token: {response.StatusCode}");
        }

        private async Task<string> GetApiKeyAsync()
        {
            // Para la API de Gemini, necesitamos una API Key
            // Por ahora, vamos a usar un enfoque simplificado
            return "AIzaSyB" + "placeholder_key"; // Placeholder
        }

        private string GenerateIntelligentResponse(string userMessage, string context)
        {
            var message = userMessage.ToLower();
            
            // Respuestas inteligentes basadas en patrones
            if (message.Contains("hola") || message.Contains("buenos") || message.Contains("buenas"))
            {
                return "¡Hola! 👋 Soy tu asistente virtual de GEEKS. Estoy aquí para ayudarte con cualquier consulta sobre nuestros productos de tecnología y gaming. ¿En qué puedo asistirte hoy? 🎮💻";
            }
            
            if (message.Contains("gaming") || message.Contains("juegos") || message.Contains("gamer"))
            {
                return "¡Excelente elección! 🎮 En GEEKS tenemos una amplia selección de productos gaming:\n\n" +
                       "🎯 **Periféricos Gaming**: Teclados mecánicos, ratones de alta precisión, headsets con sonido envolvente\n" +
                       "🖥️ **Hardware Gaming**: Tarjetas gráficas RTX, procesadores AMD Ryzen, memorias RAM de alta velocidad\n" +
                       "🎪 **Accesorios**: Alfombrillas gaming, soportes para monitor, sillas gaming ergonómicas\n\n" +
                       "¿Te gustaría que te recomiende algún producto específico o tienes alguna preferencia en particular? 😊";
            }
            
            if (message.Contains("tecnología") || message.Contains("tech") || message.Contains("computadora"))
            {
                return "¡Perfecto! 💻 En GEEKS somos expertos en tecnología. Ofrecemos:\n\n" +
                       "🔧 **Componentes PC**: Procesadores, tarjetas madre, almacenamiento SSD/NVMe\n" +
                       "📱 **Dispositivos móviles**: Smartphones, tablets, smartwatches\n" +
                       "🏠 **Smart Home**: Altavoces inteligentes, cámaras de seguridad, termostatos\n" +
                       "🎧 **Audio**: Auriculares, altavoces, micrófonos profesionales\n\n" +
                       "¿Qué tipo de tecnología te interesa más? Puedo darte recomendaciones personalizadas! ✨";
            }
            
            if (message.Contains("precio") || message.Contains("costo") || message.Contains("barato"))
            {
                return "¡Entiendo tu preocupación por el presupuesto! 💰 En GEEKS tenemos opciones para todos los bolsillos:\n\n" +
                       "💎 **Gama Premium**: Productos de última generación con la mejor calidad\n" +
                       "⚖️ **Gama Media**: Excelente relación calidad-precio para la mayoría de usuarios\n" +
                       "💡 **Gama Básica**: Productos funcionales y confiables a precios accesibles\n\n" +
                       "También tenemos ofertas especiales y descuentos frecuentes. ¿Cuál es tu rango de presupuesto? 🤔";
            }
            
            if (message.Contains("ayuda") || message.Contains("soporte") || message.Contains("problema"))
            {
                return "¡Por supuesto! 🆘 Estoy aquí para ayudarte. Puedo asistirte con:\n\n" +
                       "🔍 **Búsqueda de productos** específicos\n" +
                       "📋 **Comparaciones** entre diferentes opciones\n" +
                       "💡 **Recomendaciones** personalizadas\n" +
                       "📱 **Información** sobre características técnicas\n" +
                       "🛒 **Proceso de compra** y envío\n\n" +
                       "¿Qué necesitas específicamente? Estoy listo para ayudarte! 🚀";
            }
            
            if (message.Contains("gracias") || message.Contains("thanks"))
            {
                return "¡De nada! 😊 Es un placer poder ayudarte. Si tienes más preguntas o necesitas asistencia adicional, no dudes en preguntarme. ¡Estoy aquí para ti! 🌟";
            }
            
            if (message.Contains("adiós") || message.Contains("chao") || message.Contains("hasta luego"))
            {
                return "¡Hasta luego! 👋 Ha sido un placer ayudarte. Recuerda que estoy aquí cuando me necesites. ¡Que tengas un excelente día! ✨";
            }
            
            // Respuesta por defecto inteligente
            return "¡Interesante pregunta! 🤔 Como asistente virtual de GEEKS, puedo ayudarte con:\n\n" +
                   "🎮 **Productos Gaming**: Desde periféricos hasta hardware completo\n" +
                   "💻 **Tecnología**: Componentes, dispositivos móviles, smart home\n" +
                   "🔍 **Búsqueda**: Encontrar exactamente lo que necesitas\n" +
                   "💡 **Recomendaciones**: Basadas en tus necesidades específicas\n\n" +
                   "¿Podrías ser más específico sobre lo que buscas? Así podré ayudarte mejor! 😊";
        }
    }

    // Clases para deserializar la respuesta de Gemini
    public class GeminiResponse
    {
        public Candidate[] candidates { get; set; } = Array.Empty<Candidate>();
    }

    public class Candidate
    {
        public Content content { get; set; } = new Content();
    }

    public class Content
    {
        public Part[] parts { get; set; } = Array.Empty<Part>();
    }

    public class Part
    {
        public string text { get; set; } = string.Empty;
    }

    // Clases para deserializar la respuesta de Google Cloud (mantenidas para compatibilidad)
    public class GoogleCloudResponse
    {
        public Prediction[] predictions { get; set; } = Array.Empty<Prediction>();
    }

    public class Prediction
    {
        public string content { get; set; } = string.Empty;
    }

    // Clases para autenticación
    public class ServiceAccountCredentials
    {
        public string type { get; set; } = string.Empty;
        public string project_id { get; set; } = string.Empty;
        public string private_key_id { get; set; } = string.Empty;
        public string private_key { get; set; } = string.Empty;
        public string client_email { get; set; } = string.Empty;
        public string client_id { get; set; } = string.Empty;
        public string auth_uri { get; set; } = string.Empty;
        public string token_uri { get; set; } = string.Empty;
        public string auth_provider_x509_cert_url { get; set; } = string.Empty;
        public string client_x509_cert_url { get; set; } = string.Empty;
        public string universe_domain { get; set; } = string.Empty;
    }

    public class TokenResponse
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public int expires_in { get; set; }
    }
}
