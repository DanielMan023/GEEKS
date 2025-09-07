namespace GEEKS.Utils
{
    /// <summary>
    /// Constantes de la aplicación
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Estados de entidades
        /// </summary>
        public static class States
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
            public const string Deleted = "Deleted";
        }

        /// <summary>
        /// Roles del sistema
        /// </summary>
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string User = "User";
            public const string Manager = "Manager";
        }

        /// <summary>
        /// Scopes de roles
        /// </summary>
        public static class Scopes
        {
            public const string Admin = "ADMIN";
            public const string User = "USER";
            public const string Manager = "MANAGER";
        }

        /// <summary>
        /// Límites de validación
        /// </summary>
        public static class Validation
        {
            public const int PasswordMinLength = 6;
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const int ShortDescriptionMaxLength = 200;
            public const int SkuMaxLength = 50;
            public const int BrandMaxLength = 50;
            public const int EmailMaxLength = 255;
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int CategoryNameMaxLength = 100;
            public const int CategoryDescriptionMaxLength = 300;
        }

        /// <summary>
        /// Patrones de validación
        /// </summary>
        public static class Patterns
        {
            public const string SkuPattern = @"^[A-Z0-9-]+$";
            public const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        }

        /// <summary>
        /// Mensajes de error comunes
        /// </summary>
        public static class ErrorMessages
        {
            public const string Required = "El campo es requerido";
            public const string InvalidEmail = "El formato del email no es válido";
            public const string InvalidPassword = "La contraseña debe tener al menos 6 caracteres";
            public const string InvalidSku = "El SKU debe contener solo letras mayúsculas, números y guiones";
            public const string DuplicateSku = "El SKU ya existe";
            public const string EntityNotFound = "Entidad no encontrada";
            public const string Unauthorized = "No autorizado";
            public const string Forbidden = "Acceso denegado";
            public const string InternalServerError = "Error interno del servidor";
            public const string InvalidCredentials = "Credenciales inválidas";
            public const string EmailAlreadyExists = "El email ya está registrado";
            public const string CategoryNotFound = "La categoría especificada no existe";
            public const string ProductNotFound = "Producto no encontrado";
        }

        /// <summary>
        /// Mensajes de éxito comunes
        /// </summary>
        public static class SuccessMessages
        {
            public const string Created = "Creado exitosamente";
            public const string Updated = "Actualizado exitosamente";
            public const string Deleted = "Eliminado exitosamente";
            public const string LoginSuccess = "Login exitoso";
            public const string LogoutSuccess = "Sesión cerrada exitosamente";
            public const string RegisterSuccess = "Usuario registrado exitosamente";
            public const string TokenValid = "Token válido";
        }

        /// <summary>
        /// Configuración de paginación
        /// </summary>
        public static class Pagination
        {
            public const int DefaultPageSize = 10;
            public const int MaxPageSize = 100;
            public const int MinPage = 1;
        }

        /// <summary>
        /// Configuración de JWT
        /// </summary>
        public static class Jwt
        {
            public const int DefaultExpirationMinutes = 30;
            public const string CookieName = "auth-token";
        }
    }
}
