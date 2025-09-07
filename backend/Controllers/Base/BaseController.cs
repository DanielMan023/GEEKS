using Microsoft.AspNetCore.Mvc;
using GEEKS.Utils;

namespace GEEKS.Controllers.Base
{
    /// <summary>
    /// Controlador base con funcionalidades comunes
    /// </summary>
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Maneja el resultado de un servicio y devuelve la respuesta HTTP apropiada
        /// </summary>
        /// <typeparam name="T">Tipo de datos del resultado</typeparam>
        /// <param name="result">Resultado del servicio</param>
        /// <returns>Respuesta HTTP</returns>
        protected IActionResult HandleServiceResult<T>(ServiceResult<T> result)
        {
            if (result.Success)
            {
                return Ok(new { message = result.Message, data = result.Data });
            }

            if (result.Errors.Any())
            {
                return BadRequest(new { message = result.Message, errors = result.Errors });
            }

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Maneja el resultado de un servicio sin datos y devuelve la respuesta HTTP apropiada
        /// </summary>
        /// <param name="result">Resultado del servicio</param>
        /// <returns>Respuesta HTTP</returns>
        protected IActionResult HandleServiceResult(ServiceResult result)
        {
            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }

            if (result.Errors.Any())
            {
                return BadRequest(new { message = result.Message, errors = result.Errors });
            }

            return BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// Maneja excepciones y devuelve una respuesta de error apropiada
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <param name="logger">Logger para registrar el error</param>
        /// <param name="operation">Descripción de la operación que falló</param>
        /// <returns>Respuesta HTTP de error</returns>
        protected IActionResult HandleException(Exception ex, ILogger logger, string operation)
        {
            logger.LogError(ex, "Error en {Operation}", operation);
            return StatusCode(500, new { message = Constants.ErrorMessages.InternalServerError });
        }

        /// <summary>
        /// Valida que el ID sea válido
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <returns>True si es válido, false en caso contrario</returns>
        protected bool IsValidId(int id)
        {
            return id > 0;
        }

        /// <summary>
        /// Devuelve un error 400 si el ID no es válido
        /// </summary>
        /// <param name="id">ID a validar</param>
        /// <returns>BadRequest si el ID no es válido, null si es válido</returns>
        protected IActionResult? ValidateId(int id)
        {
            if (!IsValidId(id))
            {
                return BadRequest(new { message = "ID inválido" });
            }
            return null;
        }
    }
}
