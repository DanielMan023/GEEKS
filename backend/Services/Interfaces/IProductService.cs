using GEEKS.Dto;
using GEEKS.Utils;

namespace GEEKS.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de productos
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="dto">Datos del producto a crear</param>
        /// <returns>Resultado de la operación con el producto creado</returns>
        Task<ServiceResult<ProductResponseDTO>> CreateProductAsync(CreateProductDTO dto);

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="id">ID del producto a actualizar</param>
        /// <param name="dto">Datos actualizados del producto</param>
        /// <returns>Resultado de la operación</returns>
        Task<ServiceResult<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductDTO dto);

        /// <summary>
        /// Elimina un producto (soft delete)
        /// </summary>
        /// <param name="id">ID del producto a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<ServiceResult> DeleteProductAsync(int id);

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Resultado de la operación con el producto</returns>
        Task<ServiceResult<ProductResponseDTO>> GetProductByIdAsync(int id);

        /// <summary>
        /// Obtiene productos con filtros y paginación
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Resultado de la operación con productos paginados</returns>
        Task<ServiceResult<PaginatedResponseDTO<ProductListDTO>>> GetProductsAsync(ProductFilterDTO filter);

        /// <summary>
        /// Obtiene productos destacados
        /// </summary>
        /// <param name="limit">Límite de productos a devolver</param>
        /// <returns>Resultado de la operación con productos destacados</returns>
        Task<ServiceResult<IEnumerable<ProductListDTO>>> GetFeaturedProductsAsync(int limit = 10);

        /// <summary>
        /// Obtiene categorías con conteo de productos
        /// </summary>
        /// <returns>Resultado de la operación con categorías</returns>
        Task<ServiceResult<IEnumerable<CategoryListDTO>>> GetCategoriesAsync();

        /// <summary>
        /// Verifica si un SKU es único
        /// </summary>
        /// <param name="sku">SKU a verificar</param>
        /// <param name="excludeId">ID de producto a excluir</param>
        /// <returns>True si el SKU es único</returns>
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);

        /// <summary>
        /// Obtiene productos con stock bajo
        /// </summary>
        /// <returns>Resultado de la operación con productos de stock bajo</returns>
        Task<ServiceResult<IEnumerable<ProductListDTO>>> GetLowStockProductsAsync();

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="newStock">Nuevo stock</param>
        /// <returns>Resultado de la operación</returns>
        Task<ServiceResult> UpdateStockAsync(int productId, int newStock);

        /// <summary>
        /// Reduce el stock de un producto
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="quantity">Cantidad a reducir</param>
        /// <returns>Resultado de la operación</returns>
        Task<ServiceResult> ReduceStockAsync(int productId, int quantity);

        /// <summary>
        /// Elimina productos demo
        /// </summary>
        /// <returns>Resultado de la operación con cantidad eliminada</returns>
        Task<ServiceResult<int>> ClearDemoProductsAsync();
    }
}
