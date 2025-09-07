using GEEKS.Models;
using System.Linq.Expressions;

namespace GEEKS.Repositories.Interfaces
{
    /// <summary>
    /// Interfaz específica para el repositorio de productos
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        /// <param name="categoryId">ID de la categoría</param>
        /// <returns>Lista de productos de la categoría</returns>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Obtiene productos destacados
        /// </summary>
        /// <param name="limit">Límite de productos a devolver</param>
        /// <returns>Lista de productos destacados</returns>
        Task<IEnumerable<Product>> GetFeaturedAsync(int limit = 10);

        /// <summary>
        /// Verifica si un SKU es único
        /// </summary>
        /// <param name="sku">SKU a verificar</param>
        /// <param name="excludeId">ID de producto a excluir de la verificación</param>
        /// <returns>True si el SKU es único, false en caso contrario</returns>
        Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null);

        /// <summary>
        /// Obtiene productos con filtros avanzados
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <param name="categoryId">ID de categoría</param>
        /// <param name="brand">Marca</param>
        /// <param name="minPrice">Precio mínimo</param>
        /// <param name="maxPrice">Precio máximo</param>
        /// <param name="inStockOnly">Solo productos en stock</param>
        /// <param name="featuredOnly">Solo productos destacados</param>
        /// <returns>Lista de productos filtrados</returns>
        Task<IEnumerable<Product>> GetFilteredAsync(
            string? searchTerm = null,
            int? categoryId = null,
            string? brand = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStockOnly = null,
            bool? featuredOnly = null);

        /// <summary>
        /// Obtiene productos con información de categoría incluida
        /// </summary>
        /// <param name="predicate">Condición opcional</param>
        /// <returns>Lista de productos con categoría</returns>
        Task<IEnumerable<Product>> GetWithCategoryAsync(Expression<Func<Product, bool>>? predicate = null);

        /// <summary>
        /// Obtiene un producto con información de categoría incluida
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto con categoría o null</returns>
        Task<Product?> GetByIdWithCategoryAsync(int id);

        /// <summary>
        /// Obtiene productos con stock bajo
        /// </summary>
        /// <returns>Lista de productos con stock bajo</returns>
        Task<IEnumerable<Product>> GetLowStockAsync();

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="newStock">Nuevo stock</param>
        Task UpdateStockAsync(int productId, int newStock);

        /// <summary>
        /// Reduce el stock de un producto
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="quantity">Cantidad a reducir</param>
        Task ReduceStockAsync(int productId, int quantity);
    }
}
