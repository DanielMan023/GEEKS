using GEEKS.Dto;

namespace GEEKS.Interfaces
{
    public interface IChatbotService
    {
        Task<ChatbotResponseDTO> ProcessMessageAsync(ChatbotMessageDTO message);
        Task<List<ProductRecommendationDTO>> GetProductRecommendationsAsync(string userQuery, int? userId = null);
        Task<string> GenerateProductDescriptionAsync(string productName, string category);
        Task<ChatbotContextDTO> GetChatbotContextAsync(int? userId = null);
    }
}
