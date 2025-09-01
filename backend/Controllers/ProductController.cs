using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEEKS.Data;
using GEEKS.Dto;
using GEEKS.Models;
using System.Linq.Dynamic.Core;

namespace GEEKS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(DBContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Product
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedResponseDTO<ProductListDTO>>> GetProducts([FromQuery] ProductFilterDTO filter)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.State == "Active");

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(p => 
                        p.Name.ToLower().Contains(searchTerm) ||
                        p.Description.ToLower().Contains(searchTerm) ||
                        (p.Brand != null && p.Brand.ToLower().Contains(searchTerm)) ||
                        p.Category.Name.ToLower().Contains(searchTerm));
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
                }

                if (!string.IsNullOrEmpty(filter.Brand))
                {
                    query = query.Where(p => p.Brand == filter.Brand);
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                if (filter.InStockOnly == true)
                {
                    query = query.Where(p => p.Stock > 0);
                }

                if (filter.FeaturedOnly == true)
                {
                    query = query.Where(p => p.IsFeatured);
                }

                // Aplicar ordenamiento
                var sortBy = filter.SortBy?.ToLower() switch
                {
                    "price" => "Price",
                    "stock" => "Stock",
                    "createdat" => "CreatedAtDateTime",
                    "name" => "Name",
                    _ => "Name"
                };

                var sortOrder = filter.SortOrder?.ToLower() == "desc" ? "desc" : "asc";
                query = query.OrderBy($"{sortBy} {sortOrder}");

                // Contar total antes de paginar
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var page = Math.Max(1, filter.Page);
                var pageSize = Math.Max(1, Math.Min(100, filter.PageSize)); // Máximo 100 por página
                var skip = (page - 1) * pageSize;

                var products = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new ProductListDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ShortDescription = p.ShortDescription,
                        Price = p.Price,
                        DiscountPrice = p.DiscountPrice,
                        Stock = p.Stock,
                        MainImage = p.MainImage,
                        CategoryName = p.Category.Name,
                        Brand = p.Brand,
                        IsFeatured = p.IsFeatured,
                        State = p.State
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var response = new PaginatedResponseDTO<ProductListDTO>
                {
                    Data = products,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDTO>> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id && p.State == "Active");

                if (product == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                var productDto = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ShortDescription = product.ShortDescription,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    Stock = product.Stock,
                    MinStock = product.MinStock,
                    SKU = product.SKU,
                    MainImage = product.MainImage,
                    Images = product.Images,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    Brand = product.Brand,
                    State = product.State,
                    IsFeatured = product.IsFeatured,
                    Weight = product.Weight,
                    Length = product.Length,
                    Width = product.Width,
                    Height = product.Height,
                    CreatedAtDateTime = product.CreatedAtDateTime,
                    UpdatedAtDateTime = product.UpdatedAtDateTime
                };

                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo producto con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // POST: api/Product
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponseDTO>> CreateProduct([FromBody] CreateProductDTO createProductDto)
        {
            try
            {
                // Validar que la categoría exista
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == createProductDto.CategoryId && c.State == "Active");

                if (category == null)
                {
                    return BadRequest(new { message = "La categoría especificada no existe" });
                }

                // Validar que el SKU sea único
                var existingSku = await _context.Products
                    .AnyAsync(p => p.SKU == createProductDto.SKU);

                if (existingSku)
                {
                    return BadRequest(new { message = "El SKU ya existe" });
                }

                var product = new Product
                {
                    Name = createProductDto.Name,
                    Description = createProductDto.Description,
                    ShortDescription = createProductDto.ShortDescription,
                    Price = createProductDto.Price,
                    DiscountPrice = createProductDto.DiscountPrice,
                    Stock = createProductDto.Stock,
                    MinStock = createProductDto.MinStock,
                    SKU = createProductDto.SKU,
                    MainImage = createProductDto.MainImage,
                    Images = createProductDto.Images,
                    CategoryId = createProductDto.CategoryId,
                    Brand = createProductDto.Brand,
                    IsFeatured = createProductDto.IsFeatured,
                    Weight = createProductDto.Weight,
                    Length = createProductDto.Length,
                    Width = createProductDto.Width,
                    Height = createProductDto.Height,
                    State = "Active"
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Obtener el producto creado con la categoría
                var createdProduct = await _context.Products
                    .Include(p => p.Category)
                    .FirstAsync(p => p.Id == product.Id);

                var productResponseDto = new ProductResponseDTO
                {
                    Id = createdProduct.Id,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    ShortDescription = createdProduct.ShortDescription,
                    Price = createdProduct.Price,
                    DiscountPrice = createdProduct.DiscountPrice,
                    Stock = createdProduct.Stock,
                    MinStock = createdProduct.MinStock,
                    SKU = createdProduct.SKU,
                    MainImage = createdProduct.MainImage,
                    Images = createdProduct.Images,
                    CategoryId = createdProduct.CategoryId,
                    CategoryName = createdProduct.Category.Name,
                    Brand = createdProduct.Brand,
                    State = createdProduct.State,
                    IsFeatured = createdProduct.IsFeatured,
                    Weight = createdProduct.Weight,
                    Length = createdProduct.Length,
                    Width = createdProduct.Width,
                    Height = createdProduct.Height,
                    CreatedAtDateTime = createdProduct.CreatedAtDateTime,
                    UpdatedAtDateTime = createdProduct.UpdatedAtDateTime
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando producto");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateProductDto)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.State == "Active");

                if (product == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                // Validar que la categoría exista si se va a cambiar
                if (updateProductDto.CategoryId.HasValue)
                {
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Id == updateProductDto.CategoryId.Value && c.State == "Active");

                    if (category == null)
                    {
                        return BadRequest(new { message = "La categoría especificada no existe" });
                    }
                }

                // Validar que el SKU sea único si se va a cambiar
                if (!string.IsNullOrEmpty(updateProductDto.SKU) && updateProductDto.SKU != product.SKU)
                {
                    var existingSku = await _context.Products
                        .AnyAsync(p => p.SKU == updateProductDto.SKU && p.Id != id);

                    if (existingSku)
                    {
                        return BadRequest(new { message = "El SKU ya existe" });
                    }
                }

                // Actualizar solo los campos proporcionados
                if (!string.IsNullOrEmpty(updateProductDto.Name))
                    product.Name = updateProductDto.Name;

                if (!string.IsNullOrEmpty(updateProductDto.Description))
                    product.Description = updateProductDto.Description;

                if (updateProductDto.ShortDescription != null)
                    product.ShortDescription = updateProductDto.ShortDescription;

                if (updateProductDto.Price.HasValue)
                    product.Price = updateProductDto.Price.Value;

                if (updateProductDto.DiscountPrice.HasValue)
                    product.DiscountPrice = updateProductDto.DiscountPrice;

                if (updateProductDto.Stock.HasValue)
                    product.Stock = updateProductDto.Stock.Value;

                if (updateProductDto.MinStock.HasValue)
                    product.MinStock = updateProductDto.MinStock.Value;

                if (!string.IsNullOrEmpty(updateProductDto.SKU))
                    product.SKU = updateProductDto.SKU;

                if (updateProductDto.MainImage != null)
                    product.MainImage = updateProductDto.MainImage;

                if (updateProductDto.Images != null)
                    product.Images = updateProductDto.Images;

                if (updateProductDto.CategoryId.HasValue)
                    product.CategoryId = updateProductDto.CategoryId.Value;

                if (updateProductDto.Brand != null)
                    product.Brand = updateProductDto.Brand;

                if (updateProductDto.IsFeatured.HasValue)
                    product.IsFeatured = updateProductDto.IsFeatured.Value;

                if (updateProductDto.Weight.HasValue)
                    product.Weight = updateProductDto.Weight.Value;

                if (updateProductDto.Length.HasValue)
                    product.Length = updateProductDto.Length.Value;

                if (updateProductDto.Width.HasValue)
                    product.Width = updateProductDto.Width.Value;

                if (updateProductDto.Height.HasValue)
                    product.Height = updateProductDto.Height.Value;

                if (!string.IsNullOrEmpty(updateProductDto.State))
                    product.State = updateProductDto.State;

                product.UpdatedAtDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando producto con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id && p.State == "Active");

                if (product == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                // Soft delete - cambiar estado en lugar de eliminar físicamente
                product.State = "Deleted";
                product.UpdatedAtDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando producto con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/Product/demo/clear
        [HttpDelete("demo/clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearDemoProducts()
        {
            try
            {
                // Eliminar productos que contengan "demo" en el nombre o que sean de categorías demo
                var demoProducts = await _context.Products
                    .Where(p => p.State == "Active" && 
                               (p.Name.ToLower().Contains("demo") || 
                                p.Name.ToLower().Contains("ejemplo") ||
                                p.SKU.ToLower().Contains("demo")))
                    .ToListAsync();

                var deletedCount = demoProducts.Count;

                foreach (var product in demoProducts)
                {
                    product.State = "Deleted";
                    product.UpdatedAtDateTime = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = $"Se eliminaron {deletedCount} productos demo correctamente",
                    deletedCount = deletedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando productos demo");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Product/featured
        [HttpGet("featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductListDTO>>> GetFeaturedProducts()
        {
            try
            {
                var featuredProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsFeatured && p.State == "Active")
                    .OrderByDescending(p => p.CreatedAtDateTime)
                    .Take(10)
                    .Select(p => new ProductListDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ShortDescription = p.ShortDescription,
                        Price = p.Price,
                        DiscountPrice = p.DiscountPrice,
                        Stock = p.Stock,
                        MainImage = p.MainImage,
                        CategoryName = p.Category.Name,
                        Brand = p.Brand,
                        IsFeatured = p.IsFeatured,
                        State = p.State
                    })
                    .ToListAsync();

                return Ok(featuredProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos destacados");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // GET: api/Product/categories
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoryListDTO>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.State == "Active")
                    .Select(c => new CategoryListDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Image = c.Image,
                        State = c.State,
                        ProductCount = c.Products.Count(p => p.State == "Active")
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }
    }
}
