using System.ComponentModel.DataAnnotations;

namespace GEEKS.Dto
{
    // DTO para mensajes del usuario al chatbot
    public class ChatbotMessageDTO
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public int? UserId { get; set; }
        
        public string? SessionId { get; set; }
        
        public string? Context { get; set; } // "shopping", "support", "general"
    }

    // DTO para respuestas del chatbot
    public class ChatbotResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        
        public string Type { get; set; } = "text"; // "text", "product_list", "quick_replies"
        
        public List<QuickReplyDTO>? QuickReplies { get; set; }
        
        public List<ProductRecommendationDTO>? ProductSuggestions { get; set; }
        
        public string? Intent { get; set; } // "greeting", "product_search", "help", "purchase_help"
        
        public double Confidence { get; set; }
        
        public Dictionary<string, object>? Metadata { get; set; }
    }

    // DTO para respuestas rápidas del chatbot
    public class QuickReplyDTO
    {
        public string Text { get; set; } = string.Empty;
        
        public string Action { get; set; } = string.Empty; // "search_products", "view_categories", "help"
        
        public string? Value { get; set; }
    }

    // DTO para recomendaciones de productos
    public class ProductRecommendationDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string? ShortDescription { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal? DiscountPrice { get; set; }
        
        public string? MainImage { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
        
        public string? Brand { get; set; }
        
        public double RelevanceScore { get; set; }
        
        public string Reason { get; set; } = string.Empty; // "Similar to your search", "Popular in this category"
    }

    // DTO para contexto del chatbot
    public class ChatbotContextDTO
    {
        public List<string> RecentSearches { get; set; } = new List<string>();
        
        public List<string> PopularCategories { get; set; } = new List<string>();
        
        public List<string> TrendingProducts { get; set; } = new List<string>();
        
        public Dictionary<string, object> UserPreferences { get; set; } = new Dictionary<string, object>();
    }

    // DTO para análisis de sentimientos (opcional)
    public class SentimentAnalysisDTO
    {
        public string Text { get; set; } = string.Empty;
        
        public string Sentiment { get; set; } = string.Empty; // "positive", "negative", "neutral"
        
        public double Confidence { get; set; }
        
        public List<string> KeyPhrases { get; set; } = new List<string>();
    }
}
