using System.Collections.Generic;

namespace GEEKS.Utils
{
    /// <summary>
    /// Resultado tipado para operaciones de servicio
    /// </summary>
    /// <typeparam name="T">Tipo de datos devueltos</typeparam>
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Crea un resultado exitoso
        /// </summary>
        /// <param name="data">Datos a devolver</param>
        /// <param name="message">Mensaje de éxito</param>
        /// <returns>Resultado exitoso</returns>
        public static ServiceResult<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Crea un resultado de error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="errors">Lista de errores específicos</param>
        /// <returns>Resultado de error</returns>
        public static ServiceResult<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        /// <summary>
        /// Crea un resultado de error con un solo error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <returns>Resultado de error</returns>
        public static ServiceResult<T> ErrorResult(string message)
        {
            return ErrorResult(message, new List<string> { message });
        }
    }

    /// <summary>
    /// Resultado sin datos para operaciones que no devuelven información
    /// </summary>
    public class ServiceResult : ServiceResult<object>
    {
        /// <summary>
        /// Crea un resultado exitoso sin datos
        /// </summary>
        /// <param name="message">Mensaje de éxito</param>
        /// <returns>Resultado exitoso</returns>
        public static ServiceResult SuccessResult(string message = "Operación exitosa")
        {
            return new ServiceResult
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Crea un resultado de error sin datos
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="errors">Lista de errores específicos</param>
        /// <returns>Resultado de error</returns>
        public static new ServiceResult ErrorResult(string message, List<string>? errors = null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        /// <summary>
        /// Crea un resultado de error sin datos con un solo error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <returns>Resultado de error</returns>
        public static new ServiceResult ErrorResult(string message)
        {
            return ErrorResult(message, new List<string> { message });
        }
    }
}
