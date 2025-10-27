using GEEKS.Dto;

namespace GEEKS.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(int userId, CreateOrderDTO createOrderDto);
        Task<OrderDTO?> GetOrderByIdAsync(int orderId);
        Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO updateOrderStatusDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<List<OrderDTO>> GetOrdersByStatusAsync(string status);
        Task<OrderDTO> GetOrderByOrderNumberAsync(string orderNumber);
    }
}
