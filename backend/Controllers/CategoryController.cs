using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GEEKS.Data;
using GEEKS.Dto;
using GEEKS.Models;

namespace GEEKS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(DBContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Category
        [HttpGet]
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

        // GET: api/Category/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponseDTO>> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id && c.State == "Active");

                if (category == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                var categoryDto = new CategoryResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image,
                    State = category.State,
                    CreatedAtDateTime = category.CreatedAtDateTime,
                    UpdatedAtDateTime = category.UpdatedAtDateTime,
                    ProductCount = category.Products.Count(p => p.State == "Active")
                };

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo categoría con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // POST: api/Category
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryResponseDTO>> CreateCategory([FromBody] CreateCategoryDTO createCategoryDto)
        {
            try
            {
                // Validar que el nombre sea único
                var existingName = await _context.Categories
                    .AnyAsync(c => c.Name == createCategoryDto.Name && c.State == "Active");

                if (existingName)
                {
                    return BadRequest(new { message = "Ya existe una categoría con ese nombre" });
                }

                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description,
                    Image = createCategoryDto.Image,
                    State = "Active"
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryResponseDto = new CategoryResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Image = category.Image,
                    State = category.State,
                    CreatedAtDateTime = category.CreatedAtDateTime,
                    UpdatedAtDateTime = category.UpdatedAtDateTime,
                    ProductCount = 0
                };

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando categoría");
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO updateCategoryDto)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id && c.State == "Active");

                if (category == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                // Validar que el nombre sea único si se va a cambiar
                if (!string.IsNullOrEmpty(updateCategoryDto.Name) && updateCategoryDto.Name != category.Name)
                {
                    var existingName = await _context.Categories
                        .AnyAsync(c => c.Name == updateCategoryDto.Name && c.State == "Active" && c.Id != id);

                    if (existingName)
                    {
                        return BadRequest(new { message = "Ya existe una categoría con ese nombre" });
                    }
                }

                // Actualizar solo los campos proporcionados
                if (!string.IsNullOrEmpty(updateCategoryDto.Name))
                    category.Name = updateCategoryDto.Name;

                if (updateCategoryDto.Description != null)
                    category.Description = updateCategoryDto.Description;

                if (updateCategoryDto.Image != null)
                    category.Image = updateCategoryDto.Image;

                if (!string.IsNullOrEmpty(updateCategoryDto.State))
                    category.State = updateCategoryDto.State;

                category.UpdatedAtDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría actualizada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando categoría con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id && c.State == "Active");

                if (category == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                // Verificar que no tenga productos activos
                var activeProducts = category.Products.Count(p => p.State == "Active");
                if (activeProducts > 0)
                {
                    return BadRequest(new { message = $"No se puede eliminar la categoría porque tiene {activeProducts} productos activos" });
                }

                // Soft delete - cambiar estado en lugar de eliminar físicamente
                category.State = "Deleted";
                category.UpdatedAtDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría eliminada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando categoría con ID: {Id}", id);
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }
    }
}

