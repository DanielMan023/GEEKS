using AutoMapper;
using FluentValidation;
using GEEKS.Dto;
using GEEKS.Models;
using GEEKS.Repositories.Interfaces;
using GEEKS.Services.Interfaces;
using GEEKS.Utils;
using Microsoft.Extensions.Logging;

namespace GEEKS.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con productos
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductDTO> _createValidator;
        private readonly IValidator<UpdateProductDTO> _updateValidator;
        private readonly IValidator<ProductFilterDTO> _filterValidator;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateProductDTO> createValidator,
            IValidator<UpdateProductDTO> updateValidator,
            IValidator<ProductFilterDTO> filterValidator,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductResponseDTO>> CreateProductAsync(CreateProductDTO dto)
        {
            try
            {
                // Validar DTO
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ServiceResult<ProductResponseDTO>.ErrorResult("Datos de entrada inválidos", errors);
                }

                // Verificar que la categoría existe
                var categoryExists = await _unitOfWork.Categories.ExistsAsync(dto.CategoryId);
                if (!categoryExists)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.CategoryNotFound);
                }

                // Verificar que el SKU es único
                var isSkuUnique = await _unitOfWork.Products.IsSkuUniqueAsync(dto.SKU);
                if (!isSkuUnique)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.DuplicateSku);
                }

                // Mapear DTO a entidad
                var product = _mapper.Map<Product>(dto);
                product.State = Constants.States.Active;

                // Agregar producto
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Obtener producto creado con categoría
                var createdProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id);
                if (createdProduct == null)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.InternalServerError);
                }

                // Mapear a DTO de respuesta
                var responseDto = _mapper.Map<ProductResponseDTO>(createdProduct);

                _logger.LogInformation("Producto creado exitosamente: {ProductId}", product.Id);
                return ServiceResult<ProductResponseDTO>.SuccessResult(responseDto, Constants.SuccessMessages.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando producto");
                return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<ProductResponseDTO>> UpdateProductAsync(int id, UpdateProductDTO dto)
        {
            try
            {
                // Validar DTO
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ServiceResult<ProductResponseDTO>.ErrorResult("Datos de entrada inválidos", errors);
                }

                // Obtener producto existente
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null || product.State != Constants.States.Active)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.ProductNotFound);
                }

                // Verificar categoría si se va a cambiar
                if (dto.CategoryId.HasValue)
                {
                    var categoryExists = await _unitOfWork.Categories.ExistsAsync(dto.CategoryId.Value);
                    if (!categoryExists)
                    {
                        return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.CategoryNotFound);
                    }
                }

                // Verificar SKU único si se va a cambiar
                if (!string.IsNullOrEmpty(dto.SKU) && dto.SKU != product.SKU)
                {
                    var isSkuUnique = await _unitOfWork.Products.IsSkuUniqueAsync(dto.SKU, id);
                    if (!isSkuUnique)
                    {
                        return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.DuplicateSku);
                    }
                }

                // Actualizar campos
                _mapper.Map(dto, product);
                product.UpdatedAtDateTime = DateTime.UtcNow;

                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Obtener producto actualizado con categoría
                var updatedProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
                if (updatedProduct == null)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.InternalServerError);
                }

                var responseDto = _mapper.Map<ProductResponseDTO>(updatedProduct);

                _logger.LogInformation("Producto actualizado exitosamente: {ProductId}", id);
                return ServiceResult<ProductResponseDTO>.SuccessResult(responseDto, Constants.SuccessMessages.Updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando producto: {ProductId}", id);
                return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null || product.State != Constants.States.Active)
                {
                    return ServiceResult.ErrorResult(Constants.ErrorMessages.ProductNotFound);
                }

                await _unitOfWork.Products.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Producto eliminado exitosamente: {ProductId}", id);
                return ServiceResult.SuccessResult(Constants.SuccessMessages.Deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando producto: {ProductId}", id);
                return ServiceResult.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<ProductResponseDTO>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
                if (product == null)
                {
                    return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.ProductNotFound);
                }

                var responseDto = _mapper.Map<ProductResponseDTO>(product);
                return ServiceResult<ProductResponseDTO>.SuccessResult(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo producto: {ProductId}", id);
                return ServiceResult<ProductResponseDTO>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<PaginatedResponseDTO<ProductListDTO>>> GetProductsAsync(ProductFilterDTO filter)
        {
            try
            {
                // Validar filtros
                var validationResult = await _filterValidator.ValidateAsync(filter);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ServiceResult<PaginatedResponseDTO<ProductListDTO>>.ErrorResult("Filtros inválidos", errors);
                }

                // Obtener productos filtrados
                var products = await _unitOfWork.Products.GetFilteredAsync(
                    filter.SearchTerm,
                    filter.CategoryId,
                    filter.Brand,
                    filter.MinPrice,
                    filter.MaxPrice,
                    filter.InStockOnly,
                    filter.FeaturedOnly);

                // Aplicar ordenamiento
                var sortedProducts = ApplySorting(products, filter.SortBy, filter.SortOrder);

                // Aplicar paginación
                var page = Math.Max(Constants.Pagination.MinPage, filter.Page);
                var pageSize = Math.Max(1, Math.Min(Constants.Pagination.MaxPageSize, filter.PageSize));
                var skip = (page - 1) * pageSize;

                var pagedProducts = sortedProducts.Skip(skip).Take(pageSize).ToList();
                var totalCount = sortedProducts.Count();

                // Mapear a DTOs
                var productDtos = _mapper.Map<List<ProductListDTO>>(pagedProducts);

                var response = new PaginatedResponseDTO<ProductListDTO>
                {
                    Data = productDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasNextPage = page * pageSize < totalCount,
                    HasPreviousPage = page > 1
                };

                return ServiceResult<PaginatedResponseDTO<ProductListDTO>>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos");
                return ServiceResult<PaginatedResponseDTO<ProductListDTO>>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<IEnumerable<ProductListDTO>>> GetFeaturedProductsAsync(int limit = 10)
        {
            try
            {
                var products = await _unitOfWork.Products.GetFeaturedAsync(limit);
                var productDtos = _mapper.Map<IEnumerable<ProductListDTO>>(products);
                return ServiceResult<IEnumerable<ProductListDTO>>.SuccessResult(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos destacados");
                return ServiceResult<IEnumerable<ProductListDTO>>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<IEnumerable<CategoryListDTO>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetWhereAsync(c => c.State == Constants.States.Active);
                var categoryDtos = _mapper.Map<IEnumerable<CategoryListDTO>>(categories);
                return ServiceResult<IEnumerable<CategoryListDTO>>.SuccessResult(categoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categorías");
                return ServiceResult<IEnumerable<CategoryListDTO>>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? excludeId = null)
        {
            return await _unitOfWork.Products.IsSkuUniqueAsync(sku, excludeId);
        }

        public async Task<ServiceResult<IEnumerable<ProductListDTO>>> GetLowStockProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetLowStockAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductListDTO>>(products);
                return ServiceResult<IEnumerable<ProductListDTO>>.SuccessResult(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos con stock bajo");
                return ServiceResult<IEnumerable<ProductListDTO>>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult> UpdateStockAsync(int productId, int newStock)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.State != Constants.States.Active)
                {
                    return ServiceResult.ErrorResult(Constants.ErrorMessages.ProductNotFound);
                }

                await _unitOfWork.Products.UpdateStockAsync(productId, newStock);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Stock actualizado para producto: {ProductId}", productId);
                return ServiceResult.SuccessResult("Stock actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando stock del producto: {ProductId}", productId);
                return ServiceResult.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult> ReduceStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.State != Constants.States.Active)
                {
                    return ServiceResult.ErrorResult(Constants.ErrorMessages.ProductNotFound);
                }

                if (product.Stock < quantity)
                {
                    return ServiceResult.ErrorResult("Stock insuficiente");
                }

                await _unitOfWork.Products.ReduceStockAsync(productId, quantity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Stock reducido para producto: {ProductId}", productId);
                return ServiceResult.SuccessResult("Stock reducido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reduciendo stock del producto: {ProductId}", productId);
                return ServiceResult.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        public async Task<ServiceResult<int>> ClearDemoProductsAsync()
        {
            try
            {
                var demoProducts = await _unitOfWork.Products.GetWhereAsync(p => 
                    p.State == Constants.States.Active && 
                    (p.Name.ToLower().Contains("demo") || 
                     p.Name.ToLower().Contains("ejemplo") ||
                     p.SKU.ToLower().Contains("demo")));

                var deletedCount = 0;
                foreach (var product in demoProducts)
                {
                    await _unitOfWork.Products.DeleteAsync(product);
                    deletedCount++;
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Productos demo eliminados: {Count}", deletedCount);
                return ServiceResult<int>.SuccessResult(deletedCount, $"Se eliminaron {deletedCount} productos demo correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando productos demo");
                return ServiceResult<int>.ErrorResult(Constants.ErrorMessages.InternalServerError);
            }
        }

        private IEnumerable<Product> ApplySorting(IEnumerable<Product> products, string? sortBy, string? sortOrder)
        {
            var isDescending = sortOrder?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "price" => isDescending ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                "stock" => isDescending ? products.OrderByDescending(p => p.Stock) : products.OrderBy(p => p.Stock),
                "createdat" => isDescending ? products.OrderByDescending(p => p.CreatedAtDateTime) : products.OrderBy(p => p.CreatedAtDateTime),
                _ => isDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name)
            };
        }
    }
}
