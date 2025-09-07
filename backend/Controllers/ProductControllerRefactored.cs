using Microsoft.AspNetCore.Mvc;
using GEEKS.Controllers.Base;
using GEEKS.Dto;
using GEEKS.Services.Interfaces;
using GEEKS.Utils;

namespace GEEKS.Controllers
{
    /// <summary>
    /// Controlador refactorizado para productos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductControllerRefactored : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductControllerRefactored> _logger;

        public ProductControllerRefactored(IProductService productService, ILogger<ProductControllerRefactored> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene productos con filtros y paginación
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de productos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDTO filter)
        {
            try
            {
                var result = await _productService.GetProductsAsync(filter);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "obteniendo productos");
            }
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Producto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var idValidation = ValidateId(id);
                if (idValidation != null) return idValidation;

                var result = await _productService.GetProductByIdAsync(id);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"obteniendo producto con ID: {id}");
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="createProductDto">Datos del producto a crear</param>
        /// <returns>Producto creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createProductDto)
        {
            try
            {
                if (createProductDto == null)
                {
                    return BadRequest(new { message = "Datos del producto son requeridos" });
                }

                var result = await _productService.CreateProductAsync(createProductDto);
                
                if (result.Success && result.Data != null)
                {
                    return CreatedAtAction(nameof(GetProduct), new { id = result.Data.Id }, 
                        new { message = result.Message, data = result.Data });
                }

                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "creando producto");
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="id">ID del producto a actualizar</param>
        /// <param name="updateProductDto">Datos actualizados del producto</param>
        /// <returns>Producto actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateProductDto)
        {
            try
            {
                var idValidation = ValidateId(id);
                if (idValidation != null) return idValidation;

                if (updateProductDto == null)
                {
                    return BadRequest(new { message = "Datos del producto son requeridos" });
                }

                var result = await _productService.UpdateProductAsync(id, updateProductDto);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"actualizando producto con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina un producto (soft delete)
        /// </summary>
        /// <param name="id">ID del producto a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var idValidation = ValidateId(id);
                if (idValidation != null) return idValidation;

                var result = await _productService.DeleteProductAsync(id);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"eliminando producto con ID: {id}");
            }
        }

        /// <summary>
        /// Obtiene productos destacados
        /// </summary>
        /// <param name="limit">Límite de productos a devolver (por defecto 10)</param>
        /// <returns>Lista de productos destacados</returns>
        [HttpGet("featured")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFeaturedProducts([FromQuery] int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return BadRequest(new { message = "El límite debe estar entre 1 y 100" });
                }

                var result = await _productService.GetFeaturedProductsAsync(limit);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "obteniendo productos destacados");
            }
        }

        /// <summary>
        /// Obtiene categorías con conteo de productos
        /// </summary>
        /// <returns>Lista de categorías</returns>
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var result = await _productService.GetCategoriesAsync();
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "obteniendo categorías");
            }
        }

        /// <summary>
        /// Obtiene productos con stock bajo
        /// </summary>
        /// <returns>Lista de productos con stock bajo</returns>
        [HttpGet("low-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLowStockProducts()
        {
            try
            {
                var result = await _productService.GetLowStockProductsAsync();
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "obteniendo productos con stock bajo");
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="stock">Nuevo stock</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}/stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int stock)
        {
            try
            {
                var idValidation = ValidateId(id);
                if (idValidation != null) return idValidation;

                if (stock < 0)
                {
                    return BadRequest(new { message = "El stock no puede ser negativo" });
                }

                var result = await _productService.UpdateStockAsync(id, stock);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"actualizando stock del producto con ID: {id}");
            }
        }

        /// <summary>
        /// Reduce el stock de un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="quantity">Cantidad a reducir</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}/reduce-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReduceStock(int id, [FromBody] int quantity)
        {
            try
            {
                var idValidation = ValidateId(id);
                if (idValidation != null) return idValidation;

                if (quantity <= 0)
                {
                    return BadRequest(new { message = "La cantidad debe ser mayor a 0" });
                }

                var result = await _productService.ReduceStockAsync(id, quantity);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, $"reduciendo stock del producto con ID: {id}");
            }
        }

        /// <summary>
        /// Elimina productos demo
        /// </summary>
        /// <returns>Resultado de la operación con cantidad eliminada</returns>
        [HttpDelete("demo/clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClearDemoProducts()
        {
            try
            {
                var result = await _productService.ClearDemoProductsAsync();
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "eliminando productos demo");
            }
        }

        /// <summary>
        /// Verifica si un SKU es único
        /// </summary>
        /// <param name="sku">SKU a verificar</param>
        /// <param name="excludeId">ID de producto a excluir de la verificación</param>
        /// <returns>True si el SKU es único</returns>
        [HttpGet("validate-sku")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateSku([FromQuery] string sku, [FromQuery] int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(sku))
                {
                    return BadRequest(new { message = "El SKU es requerido" });
                }

                var isUnique = await _productService.IsSkuUniqueAsync(sku, excludeId);
                return Ok(new { isUnique = isUnique, message = isUnique ? "SKU disponible" : "SKU ya existe" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, _logger, "validando SKU");
            }
        }
    }
}
