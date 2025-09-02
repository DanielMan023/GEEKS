using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace GEEKS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No se ha seleccionado ningún archivo");
                }

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Solo se permiten archivos de imagen (JPG, PNG, GIF, WEBP)");
                }

                // Validar tamaño (máximo 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("El archivo es demasiado grande. Máximo 5MB");
                }

                // Crear directorio si no existe
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único para el archivo
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornar URL relativa
                var imageUrl = $"/uploads/products/{fileName}";

                return Ok(new { 
                    success = true, 
                    imageUrl = imageUrl,
                    fileName = fileName,
                    message = "Imagen subida correctamente" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error al subir la imagen: {ex.Message}" 
                });
            }
        }

        [HttpDelete("delete-image/{fileName}")]
        public IActionResult DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { 
                        success = true, 
                        message = "Imagen eliminada correctamente" 
                    });
                }

                return NotFound("Archivo no encontrado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error al eliminar la imagen: {ex.Message}" 
                });
            }
        }
    }
}
