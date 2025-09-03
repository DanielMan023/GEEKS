using GEEKS.Interfaces;
using GEEKS.Dto;
using GEEKS.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GEEKS.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly DBContext _context;
        private readonly ILogger<ChatbotService> _logger;
        private readonly IOpenAIService _openAIService;

        public ChatbotService(DBContext context, ILogger<ChatbotService> logger, IOpenAIService openAIService)
        {
            _context = context;
            _logger = logger;
            _openAIService = openAIService;
        }

        public async Task<ChatbotResponseDTO> ProcessMessageAsync(ChatbotMessageDTO message)
        {
            try
            {
                var userMessage = message.Message.ToLower().Trim();
                
                // Detectar intención del usuario
                var intent = DetectIntent(userMessage);
                var confidence = CalculateConfidence(userMessage, intent);
                
                // Generar respuesta con ChatGPT
                var response = await GenerateResponseWithAIAsync(intent, userMessage, message.UserId);
                
                // Agregar respuestas rápidas contextuales
                response.QuickReplies = GenerateQuickReplies(intent);
                
                response.Intent = intent;
                response.Confidence = confidence;
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando mensaje del chatbot");
                return new ChatbotResponseDTO
                {
                    Message = "Lo siento, estoy teniendo problemas técnicos. ¿Puedes intentar de nuevo?",
                    Type = "text",
                    Intent = "error",
                    Confidence = 0.0
                };
            }
        }

        public async Task<List<ProductRecommendationDTO>> GetProductRecommendationsAsync(string userQuery, int? userId = null)
        {
            try
            {
                var query = userQuery.ToLower();
                var products = new List<ProductRecommendationDTO>();
                
                // Buscar productos por nombre, descripción o categoría
                var searchResults = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.State == "Active" && (
                        p.Name.ToLower().Contains(query) ||
                        p.Description.ToLower().Contains(query) ||
                        p.Category.Name.ToLower().Contains(query) ||
                        (p.Brand != null && p.Brand.ToLower().Contains(query))
                    ))
                    .Take(5)
                    .ToListAsync();

                foreach (var product in searchResults)
                {
                    var relevanceScore = CalculateRelevanceScore(query, product);
                    products.Add(new ProductRecommendationDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        ShortDescription = product.ShortDescription,
                        Price = product.Price,
                        DiscountPrice = product.DiscountPrice,
                        MainImage = product.MainImage,
                        CategoryName = product.Category.Name,
                        Brand = product.Brand,
                        RelevanceScore = relevanceScore,
                        Reason = GetRecommendationReason(query, product, relevanceScore)
                    });
                }

                return products.OrderByDescending(p => p.RelevanceScore).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo recomendaciones de productos");
                return new List<ProductRecommendationDTO>();
            }
        }

        public async Task<string> GenerateProductDescriptionAsync(string productName, string category)
        {
            // Usar el servicio de IA para generar descripciones
            return await _openAIService.GenerateProductDescriptionAsync(productName, category);
        }

        public async Task<ChatbotContextDTO> GetChatbotContextAsync(int? userId = null)
        {
            try
            {
                var context = new ChatbotContextDTO();
                
                // Obtener categorías populares
                var popularCategories = await _context.Categories
                    .Where(c => c.State == "Active")
                    .OrderByDescending(c => c.Products.Count(p => p.State == "Active"))
                    .Take(5)
                    .Select(c => c.Name)
                    .ToListAsync();
                
                context.PopularCategories = popularCategories;
                
                // Obtener productos destacados
                var trendingProducts = await _context.Products
                    .Where(p => p.IsFeatured && p.State == "Active")
                    .OrderByDescending(p => p.CreatedAtDateTime)
                    .Take(5)
                    .Select(p => p.Name)
                    .ToListAsync();
                
                context.TrendingProducts = trendingProducts;
                
                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo contexto del chatbot");
                return new ChatbotContextDTO();
            }
        }

        private string DetectIntent(string message)
        {
            // Patrones para detectar intenciones
            if (Regex.IsMatch(message, @"\b(hola|buenos|buenas|saludos|hey|hi|hello)\b"))
                return "greeting";
            
            if (Regex.IsMatch(message, @"\b(buscar|encontrar|producto|comprar|precio|cuanto|vale)\b"))
                return "product_search";
            
            if (Regex.IsMatch(message, @"\b(ayuda|ayudar|soporte|problema|error|como|funciona)\b"))
                return "help";
            
            if (Regex.IsMatch(message, @"\b(categoria|tipo|clase|gama)\b"))
                return "category_inquiry";
            
            if (Regex.IsMatch(message, @"\b(gracias|thanks|thank|perfecto|excelente|genial)\b"))
                return "gratitude";
            
            if (Regex.IsMatch(message, @"\b(despedida|adios|chao|bye|goodbye|hasta|luego)\b"))
                return "farewell";
            
            return "general_inquiry";
        }

        private double CalculateConfidence(string message, string intent)
        {
            // Algoritmo simple de confianza basado en palabras clave
            var keywordMatches = 0;
            var totalKeywords = 0;
            
            switch (intent)
            {
                case "greeting":
                    var greetingKeywords = new[] { "hola", "buenos", "buenas", "saludos", "hey", "hi", "hello" };
                    totalKeywords = greetingKeywords.Length;
                    keywordMatches = greetingKeywords.Count(k => message.Contains(k));
                    break;
                    
                case "product_search":
                    var searchKeywords = new[] { "buscar", "encontrar", "producto", "comprar", "precio", "cuanto", "vale" };
                    totalKeywords = searchKeywords.Length;
                    keywordMatches = searchKeywords.Count(k => message.Contains(k));
                    break;
                    
                case "help":
                    var helpKeywords = new[] { "ayuda", "ayudar", "soporte", "problema", "error", "como", "funciona" };
                    totalKeywords = helpKeywords.Length;
                    keywordMatches = helpKeywords.Count(k => message.Contains(k));
                    break;
            }
            
            return totalKeywords > 0 ? (double)keywordMatches / totalKeywords : 0.5;
        }

        private async Task<ChatbotResponseDTO> GenerateResponseWithAIAsync(string intent, string message, int? userId)
        {
            try
            {
                // Construir contexto para ChatGPT
                var context = await BuildContextForAI(userId);
                
                // Generar prompt basado en la intención
                var prompt = BuildPromptForIntent(intent, message, context);
                
                // Obtener respuesta de ChatGPT
                var aiResponse = await _openAIService.GetChatResponseAsync(prompt, context);
                
                // Procesar la respuesta según el tipo de intención
                return await ProcessAIResponse(intent, aiResponse, message, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error usando IA, fallback a respuestas básicas");
                return await GenerateResponseAsync(intent, message, userId);
            }
        }

        private async Task<ChatbotResponseDTO> GenerateResponseAsync(string intent, string message, int? userId)
        {
            switch (intent)
            {
                case "greeting":
                    return await GenerateGreetingResponseAsync(userId);
                    
                case "product_search":
                    return await GenerateProductSearchResponseAsync(message);
                    
                case "help":
                    return GenerateHelpResponse();
                    
                case "category_inquiry":
                    return await GenerateCategoryResponseAsync();
                    
                case "gratitude":
                    return new ChatbotResponseDTO
                    {
                        Message = "¡De nada! Estoy aquí para ayudarte. ¿Hay algo más en lo que pueda asistirte?",
                        Type = "text"
                    };
                    
                case "farewell":
                    return new ChatbotResponseDTO
                    {
                        Message = "¡Hasta luego! Ha sido un placer ayudarte. ¡Vuelve pronto!",
                        Type = "text"
                    };
                    
                default:
                    return await GenerateGeneralResponseAsync(message);
            }
        }

        private async Task<ChatbotResponseDTO> GenerateGreetingResponseAsync(int? userId)
        {
            var timeOfDay = DateTime.Now.Hour;
            string greeting;
            
            if (timeOfDay < 12)
                greeting = "¡Buenos días!";
            else if (timeOfDay < 18)
                greeting = "¡Buenas tardes!";
            else
                greeting = "¡Buenas noches!";
            
            var context = await GetChatbotContextAsync(userId);
            var popularCategories = string.Join(", ", context.PopularCategories.Take(3));
            
            return new ChatbotResponseDTO
            {
                Message = $"{greeting} Soy tu asistente virtual de GEEKS. " +
                         "Puedo ayudarte a encontrar productos, explorar categorías o resolver cualquier duda. " +
                         $"Algunas categorías populares son: {popularCategories}. " +
                         "¿En qué puedo ayudarte hoy?",
                Type = "text"
            };
        }

        private async Task<ChatbotResponseDTO> GenerateProductSearchResponseAsync(string message)
        {
            // Extraer términos de búsqueda
            var searchTerms = ExtractSearchTerms(message);
            
            if (string.IsNullOrEmpty(searchTerms))
            {
                return new ChatbotResponseDTO
                {
                    Message = "¿Qué producto estás buscando? Puedes decirme el nombre, categoría o características que te interesan.",
                    Type = "text"
                };
            }
            
            // Buscar productos
            var recommendations = await GetProductRecommendationsAsync(searchTerms);
            
            if (recommendations.Any())
            {
                return new ChatbotResponseDTO
                {
                    Message = $"He encontrado {recommendations.Count} productos relacionados con '{searchTerms}':",
                    Type = "product_list",
                    ProductSuggestions = recommendations
                };
            }
            else
            {
                return new ChatbotResponseDTO
                {
                    Message = $"No encontré productos específicos para '{searchTerms}'. " +
                             "¿Podrías ser más específico o probar con otros términos?",
                    Type = "text"
                };
            }
        }

        private ChatbotResponseDTO GenerateHelpResponse()
        {
            return new ChatbotResponseDTO
            {
                Message = "¡Por supuesto! Te explico lo que puedo hacer:\n\n" +
                         "🔍 **Buscar productos**: Dime qué buscas y te ayudo a encontrarlo\n" +
                         "📂 **Explorar categorías**: Te muestro las categorías disponibles\n" +
                         "💰 **Información de precios**: Te doy detalles sobre precios y descuentos\n" +
                         "❓ **Ayuda general**: Resuelvo dudas sobre el proceso de compra\n\n" +
                         "¿Qué te gustaría hacer?",
                Type = "text"
            };
        }

        private async Task<ChatbotResponseDTO> GenerateCategoryResponseAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.State == "Active")
                .Select(c => c.Name)
                .ToListAsync();
            
            var categoryList = string.Join(", ", categories);
            
            return new ChatbotResponseDTO
            {
                Message = $"Tenemos las siguientes categorías disponibles:\n\n{categoryList}\n\n" +
                         "¿Te interesa alguna categoría en particular? Puedo mostrarte los productos destacados de cada una.",
                Type = "text"
            };
        }

        private async Task<ChatbotResponseDTO> GenerateGeneralResponseAsync(string message)
        {
            // Intentar buscar productos relacionados
            var recommendations = await GetProductRecommendationsAsync(message);
            
            if (recommendations.Any())
            {
                return new ChatbotResponseDTO
                {
                    Message = $"Creo que podrías estar interesado en estos productos relacionados con tu consulta:",
                    Type = "product_list",
                    ProductSuggestions = recommendations
                };
            }
            
            return new ChatbotResponseDTO
            {
                Message = "Entiendo tu consulta. ¿Te gustaría que busque productos específicos o prefieres que te ayude con algo más concreto? " +
                         "Puedo ayudarte a buscar productos, explorar categorías o resolver dudas sobre el proceso de compra.",
                Type = "text"
            };
        }

        private async Task<string> BuildContextForAI(int? userId)
        {
            var context = new List<string>();
            
            // Información del usuario
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    context.Add($"Usuario: {user.FirstName} {user.LastName}");
                    context.Add($"Rol: {user.Role?.Name ?? "Usuario"}");
                }
            }
            
            // Categorías disponibles
            var categories = await _context.Categories
                .Where(c => c.State == "Active")
                .Select(c => c.Name)
                .ToListAsync();
            context.Add($"Categorías disponibles: {string.Join(", ", categories)}");
            
            // Productos populares
            var popularProducts = await _context.Products
                .Where(p => p.State == "Active")
                .OrderByDescending(p => p.CreatedAtDateTime)
                .Take(5)
                .Select(p => p.Name)
                .ToListAsync();
            context.Add($"Productos populares: {string.Join(", ", popularProducts)}");
            
            return string.Join(" | ", context);
        }

        private string BuildPromptForIntent(string intent, string message, string context)
        {
            return intent switch
            {
                "greeting" => $"El usuario está saludando. Responde de forma amigable y muéstrale las opciones disponibles. Mensaje: {message}",
                
                "product_search" => $"El usuario está buscando productos. Ayúdalo a encontrar lo que necesita y sugiere opciones relevantes. Búsqueda: {message}",
                
                "help" => $"El usuario necesita ayuda. Explícale claramente cómo puedes ayudarlo y qué opciones tiene disponibles.",
                
                "category_inquiry" => $"El usuario quiere saber sobre categorías. Muéstrale las categorías disponibles y ayúdalo a elegir.",
                
                "gratitude" => $"El usuario está agradeciendo. Responde de forma cálida y ofrécete a seguir ayudando.",
                
                "farewell" => $"El usuario se está despidiendo. Despídete de forma amigable y anímalo a volver.",
                
                _ => $"El usuario dijo: '{message}'. Responde de forma útil y natural, ayudándolo con lo que necesite."
            };
        }

        private async Task<ChatbotResponseDTO> ProcessAIResponse(string intent, string aiResponse, string message, int? userId)
        {
            // Si es búsqueda de productos, combinar IA con búsqueda real
            if (intent == "product_search")
            {
                var searchTerms = ExtractSearchTerms(message);
                var recommendations = await GetProductRecommendationsAsync(searchTerms, userId);
                
                if (recommendations.Any())
                {
                    return new ChatbotResponseDTO
                    {
                        Message = aiResponse,
                        Type = "product_list",
                        ProductSuggestions = recommendations.Take(3).ToList()
                    };
                }
            }
            
            // Para otras intenciones, usar solo la respuesta de IA
            return new ChatbotResponseDTO
            {
                Message = aiResponse,
                Type = "text"
            };
        }

        private List<QuickReplyDTO> GenerateQuickReplies(string intent)
        {
            var quickReplies = new List<QuickReplyDTO>();
            
            switch (intent)
            {
                case "greeting":
                    quickReplies.AddRange(new[]
                    {
                        new QuickReplyDTO { Text = "🔍 Buscar productos", Action = "search_products" },
                        new QuickReplyDTO { Text = "📂 Ver categorías", Action = "view_categories" },
                        new QuickReplyDTO { Text = "❓ Necesito ayuda", Action = "help" }
                    });
                    break;
                    
                case "product_search":
                    quickReplies.AddRange(new[]
                    {
                        new QuickReplyDTO { Text = "📱 Electrónicos", Action = "search_category", Value = "Electrónicos" },
                        new QuickReplyDTO { Text = "🎮 Gaming", Action = "search_category", Value = "Gaming" },
                        new QuickReplyDTO { Text = "👕 Ropa", Action = "search_category", Value = "Ropa" }
                    });
                    break;
                    
                case "help":
                    quickReplies.AddRange(new[]
                    {
                        new QuickReplyDTO { Text = "🔍 Cómo buscar", Action = "search_help" },
                        new QuickReplyDTO { Text = "💰 Precios y descuentos", Action = "pricing_help" },
                        new QuickReplyDTO { Text = "📦 Proceso de compra", Action = "purchase_help" }
                    });
                    break;
                    
                default:
                    quickReplies.AddRange(new[]
                    {
                        new QuickReplyDTO { Text = "🔍 Buscar productos", Action = "search_products" },
                        new QuickReplyDTO { Text = "📂 Ver categorías", Action = "view_categories" },
                        new QuickReplyDTO { Text = "❓ Ayuda", Action = "help" }
                    });
                    break;
            }
            
            return quickReplies;
        }

        private string ExtractSearchTerms(string message)
        {
            // Eliminar palabras comunes que no son términos de búsqueda
            var stopWords = new[] { "buscar", "encontrar", "producto", "comprar", "precio", "cuanto", "vale", "quiero", "necesito", "me", "gustaria" };
            
            var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var searchTerms = words.Where(word => !stopWords.Contains(word.ToLower())).ToArray();
            
            return string.Join(" ", searchTerms);
        }

        private double CalculateRelevanceScore(string query, Models.Product product)
        {
            var score = 0.0;
            var queryLower = query.ToLower();
            
            // Puntuación por nombre del producto
            if (product.Name.ToLower().Contains(queryLower))
                score += 10.0;
            
            // Puntuación por descripción
            if (product.Description.ToLower().Contains(queryLower))
                score += 5.0;
            
            // Puntuación por categoría
            if (product.Category.Name.ToLower().Contains(queryLower))
                score += 8.0;
            
            // Puntuación por marca
            if (!string.IsNullOrEmpty(product.Brand) && product.Brand.ToLower().Contains(queryLower))
                score += 6.0;
            
            // Puntuación por producto destacado
            if (product.IsFeatured)
                score += 2.0;
            
            return score;
        }

        private string GetRecommendationReason(string query, Models.Product product, double score)
        {
            if (score >= 15)
                return "Coincidencia exacta con tu búsqueda";
            else if (score >= 10)
                return "Muy relacionado con lo que buscas";
            else if (score >= 5)
                return "Relacionado con tu consulta";
            else
                return "Producto popular en esta categoría";
        }
    }
}
