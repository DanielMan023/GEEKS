using GEEKS.Dto;

namespace GEEKS.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(int userId);
        Task<CartDTO> AddToCartAsync(int userId, AddToCartDTO addToCartDto);
        Task<CartDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO updateCartItemDto);
        Task<CartDTO> RemoveFromCartAsync(int userId, RemoveFromCartDTO removeFromCartDto);
        Task<CartDTO> ClearCartAsync(int userId);
        Task<bool> CartExistsAsync(int userId);
    }
}
