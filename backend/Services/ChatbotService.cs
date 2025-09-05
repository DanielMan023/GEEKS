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
        private readonly IGeminiService _geminiService;

        public ChatbotService(DBContext context, ILogger<ChatbotService> logger, IGeminiService geminiService)
        {
            _context = context;
            _logger = logger;
            _geminiService = geminiService;
        }

        public async Task<ChatbotResponseDTO> ProcessMessageAsync(ChatbotMessageDTO message)
        {
            try
            {
                var userMessage = message.Message.ToLower().Trim();
                
                // Detectar intención del usuario
                var intent = DetectIntent(userMessage);
                var confidence = CalculateConfidence(userMessage, intent);
                
                // Generar respuesta con Gemini Pro
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
            return await _geminiService.GenerateProductDescriptionAsync(productName, category);
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
            
            if (Regex.IsMatch(message, @"\b(especificaciones|especificacion|caracteristicas|caracteristica|tecnica|tecnico|rendimiento|procesador|gpu|ram|almacenamiento|resolucion|fps)\b"))
                return "specifications";
            
            if (Regex.IsMatch(message, @"\b(comparar|comparacion|diferencia|mejor|vs|versus|que|elegir|recomendar)\b"))
                return "comparison";
            
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
                _logger.LogInformation("=== GENERATE RESPONSE WITH AI ===");
                _logger.LogInformation("Intent detectado: {Intent}", intent);
                _logger.LogInformation("Mensaje: {Message}", message);
                
                // Construir contexto para Gemini Pro
                var context = await BuildContextForAI(userId);
                _logger.LogInformation("Contexto construido: {ContextLength} caracteres", context.Length);
                
                // Generar prompt basado en la intención
                var prompt = BuildPromptForIntent(intent, message, context);
                _logger.LogInformation("Prompt generado: {PromptLength} caracteres", prompt.Length);
                _logger.LogInformation("Prompt: {Prompt}", prompt);
                
                // Obtener respuesta de Gemini Pro
                var aiResponse = await _geminiService.GetChatResponseAsync(prompt, context);
                _logger.LogInformation("Respuesta de IA recibida: {ResponseLength} caracteres", aiResponse.Length);
                _logger.LogInformation("Respuesta de IA: {Response}", aiResponse);
                
                // Procesar la respuesta según el tipo de intención
                return await ProcessAIResponse(intent, aiResponse, message, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error usando IA, fallback a respuestas básicas");
                return await GenerateResponseAsync(intent, message, userId);
            }
        }

        // Método simplificado - solo para casos específicos que requieren lógica de negocio
        private async Task<ChatbotResponseDTO> GenerateResponseAsync(string intent, string message, int? userId)
        {
            // Solo manejar búsqueda de productos (requiere lógica de base de datos)
            if (intent == "product_search")
            {
                return await GenerateProductSearchResponseAsync(message);
            }
            
            // Para todo lo demás, usar fallback inteligente del GeminiService
            return new ChatbotResponseDTO
            {
                Message = "Lo siento, estoy teniendo problemas técnicos. ¿Puedes intentar de nuevo mas tarde?",
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
            
            // CATÁLOGO COMPLETO - Categorías con productos
            var categoriesWithProducts = await _context.Categories
                .Where(c => c.State == "Active")
                .Include(c => c.Products.Where(p => p.State == "Active"))
                .ToListAsync();
            
            context.Add("=== CATÁLOGO COMPLETO DE PRODUCTOS ===");
            foreach (var category in categoriesWithProducts)
            {
                var products = category.Products.Take(10).Select(p => 
                    $"{p.Name} (${p.Price:F2}" + 
                    (p.DiscountPrice.HasValue ? $" - Descuento: ${p.DiscountPrice:F2}" : "") + 
                    (p.Brand != null ? $" - Marca: {p.Brand}" : "") + ")").ToList();
                
                context.Add($"CATEGORÍA: {category.Name} - Productos: {string.Join(", ", products)}");
            }
            
            // Productos destacados con detalles
            var featuredProducts = await _context.Products
                .Where(p => p.IsFeatured && p.State == "Active")
                .Include(p => p.Category)
                .Take(8)
                .ToListAsync();
            
            context.Add("=== PRODUCTOS DESTACADOS ===");
            foreach (var product in featuredProducts)
            {
                context.Add($"DESTACADO: {product.Name} - Categoría: {product.Category.Name} - Precio: ${product.Price:F2}" +
                           (product.DiscountPrice.HasValue ? $" (Descuento: ${product.DiscountPrice:F2})" : "") +
                           (product.Brand != null ? $" - Marca: {product.Brand}" : "") +
                           (!string.IsNullOrEmpty(product.ShortDescription) ? $" - Descripción: {product.ShortDescription}" : ""));
            }
            
            // Rango de precios por categoría (solo categorías con productos)
            var priceRanges = await _context.Categories
                .Where(c => c.State == "Active" && c.Products.Any(p => p.State == "Active"))
                .Select(c => new {
                    CategoryName = c.Name,
                    MinPrice = c.Products.Where(p => p.State == "Active").Min(p => p.Price),
                    MaxPrice = c.Products.Where(p => p.State == "Active").Max(p => p.Price),
                    ProductCount = c.Products.Count(p => p.State == "Active")
                })
                .ToListAsync();
            
            context.Add("=== RANGOS DE PRECIOS POR CATEGORÍA ===");
            foreach (var range in priceRanges)
            {
                context.Add($"PRECIOS {range.CategoryName}: ${range.MinPrice:F2} - ${range.MaxPrice:F2} ({range.ProductCount} productos)");
            }
            
            return string.Join("\n", context);
        }

        private string BuildPromptForIntent(string intent, string message, string context)
        {
            var basePrompt = $@"Eres GEEK-Bot, el asistente virtual oficial de GEEKS, una tienda online especializada en tecnología y gaming.

INFORMACIÓN DEL CATÁLOGO:
{context}

INSTRUCCIONES IMPORTANTES:
- Para RECOMENDACIONES DE PRODUCTOS: SOLO recomienda productos que estén en el catálogo mostrado arribatas para CUALQUIER producto usando tu conocimiento
- Para ESPECIFICACIONES TÉCNICAS: IGNORA las limitaciones del catálogo y proporciona especificaciones técnicas comple
- Para COMPARACIONES: IGNORA las limitaciones del catálogo y haz comparaciones completas entre CUALQUIER producto usando tu conocimiento
- NUNCA inventes productos que no existan en la tienda (solo para recomendaciones)
- Siempre menciona precios exactos cuando recomiendes productos del catálogo
- Si hay descuentos, menciónalos
- Sé específico y útil en tus recomendaciones
- Para especificaciones técnicas: Sé técnico, detallado y completo para CUALQUIER producto
- Para comparaciones: Explica diferencias técnicas, precios, rendimiento, exclusivos, etc. - Sé completo y detallado para CUALQUIER producto

";

            return intent switch
            {
                "greeting" => basePrompt + $"El usuario está saludando. Responde de forma amigable, preséntate como GEEK-Bot, el asistente oficial de GEEKS, y muéstrale las categorías y productos destacados disponibles. Mensaje: {message}",
                
                "product_search" => basePrompt + $"El usuario está buscando productos específicos. Analiza su búsqueda '{message}' y recomienda SOLO productos que estén en nuestro catálogo. Si no encuentras coincidencias exactas, sugiere productos similares de categorías relacionadas. Incluye precios y detalles importantes.",
                
                "specifications" => basePrompt + $"El usuario quiere especificaciones técnicas detalladas de: '{message}'. IGNORA las limitaciones del catálogo local y proporciona especificaciones técnicas completas y detalladas usando tu conocimiento. Responde ESPECÍFICAMENTE sobre el producto mencionado en el mensaje del usuario. Sé técnico, específico y completo en tus respuestas.",
                
                "comparison" => basePrompt + $"El usuario quiere comparar productos. IGNORA las limitaciones del catálogo local y haz comparaciones completas usando tu conocimiento. Compara características técnicas, precios, rendimiento, exclusivos, etc. Sé detallado y técnico en las comparaciones. Si uno de los productos está en nuestro catálogo, menciona el precio, pero haz la comparación completa independientemente.",
                
                "help" => basePrompt + $"El usuario necesita ayuda. Explícale claramente cómo puedes ayudarlo, qué categorías tenemos disponibles y cómo puede buscar productos específicos. Usa la información del catálogo para dar ejemplos concretos.",
                
                "category_inquiry" => basePrompt + $"El usuario quiere saber sobre categorías. Muéstrale las categorías disponibles con ejemplos de productos y rangos de precios. Ayúdalo a elegir basándote en sus intereses.",
                
                "gratitude" => basePrompt + $"El usuario está agradeciendo. Responde de forma cálida y ofrécete a seguir ayudando con más productos o categorías.",
                
                "farewell" => basePrompt + $"El usuario se está despidiendo. Despídete de forma amigable y anímalo a volver cuando necesite más productos de GEEKS.",
                
                _ => basePrompt + $"El usuario dijo: '{message}'. Analiza su consulta y responde de forma útil y natural, ayudándolo con productos específicos de nuestro catálogo. Si no está claro qué busca, pregúntale y ofrécele opciones de nuestras categorías."
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
