using GEEKS.Data;
using GEEKS.Models;
using GEEKS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GEEKS.Repositories
{
    /// <summary>
    /// Implementación del repositorio de productos
    /// </summary>
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(DBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId && p.State == "Active")
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedAsync(int limit = 10)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.State == "Active")
                .OrderByDescending(p => p.CreatedAtDateTime)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.SKU == sku);
            
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Product>> GetFilteredAsync(
            string? searchTerm = null,
            int? categoryId = null,
            string? brand = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStockOnly = null,
            bool? featuredOnly = null)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Where(p => p.State == "Active");

            // Aplicar filtros
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTermLower) ||
                    p.Description.ToLower().Contains(searchTermLower) ||
                    (p.Brand != null && p.Brand.ToLower().Contains(searchTermLower)) ||
                    p.Category.Name.ToLower().Contains(searchTermLower));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(p => p.Brand == brand);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (inStockOnly == true)
                query = query.Where(p => p.Stock > 0);

            if (featuredOnly == true)
                query = query.Where(p => p.IsFeatured);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetWithCategoryAsync(Expression<Func<Product, bool>>? predicate = null)
        {
            IQueryable<Product> query = _dbSet.Include(p => p.Category);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.State == "Active");
        }

        public async Task<IEnumerable<Product>> GetLowStockAsync()
        {
            return await _dbSet
                .Where(p => p.State == "Active" && p.Stock <= p.MinStock)
                .ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int newStock)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.Stock = newStock;
                product.UpdatedAtDateTime = DateTime.UtcNow;
                await UpdateAsync(product);
            }
        }

        public async Task ReduceStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null && product.Stock >= quantity)
            {
                product.Stock -= quantity;
                product.UpdatedAtDateTime = DateTime.UtcNow;
                await UpdateAsync(product);
            }
        }
    }
}
