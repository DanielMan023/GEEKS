/**
 * Constantes de validación
 */
export const VALIDATION_RULES = {
  PASSWORD_MIN_LENGTH: 6,
  NAME_MAX_LENGTH: 100,
  DESCRIPTION_MAX_LENGTH: 500,
  SHORT_DESCRIPTION_MAX_LENGTH: 200,
  SKU_MAX_LENGTH: 50,
  BRAND_MAX_LENGTH: 50,
  EMAIL_MAX_LENGTH: 255,
  FIRST_NAME_MAX_LENGTH: 50,
  LAST_NAME_MAX_LENGTH: 50,
  CATEGORY_NAME_MAX_LENGTH: 100,
  CATEGORY_DESCRIPTION_MAX_LENGTH: 300
} as const;

/**
 * Patrones de validación
 */
export const VALIDATION_PATTERNS = {
  SKU: /^[A-Z0-9-]+$/,
  EMAIL: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
  PASSWORD: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/,
  NAME: /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/
} as const;

/**
 * Mensajes de error de validación
 */
export const VALIDATION_MESSAGES = {
  REQUIRED: 'Este campo es requerido',
  INVALID_EMAIL: 'El formato del email no es válido',
  INVALID_PASSWORD: 'La contraseña debe tener al menos 6 caracteres',
  INVALID_PASSWORD_COMPLEXITY: 'La contraseña debe contener al menos una letra minúscula, una mayúscula y un número',
  INVALID_SKU: 'El SKU debe contener solo letras mayúsculas, números y guiones',
  INVALID_NAME: 'Solo se permiten letras y espacios',
  MIN_LENGTH: (min: number) => `Debe tener al menos ${min} caracteres`,
  MAX_LENGTH: (max: number) => `No puede exceder ${max} caracteres`,
  MIN_VALUE: (min: number) => `Debe ser mayor o igual a ${min}`,
  MAX_VALUE: (max: number) => `Debe ser menor o igual a ${max}`,
  GREATER_THAN: (value: number) => `Debe ser mayor a ${value}`,
  LESS_THAN: (value: number) => `Debe ser menor a ${value}`
} as const;

/**
 * Mensajes de éxito
 */
export const SUCCESS_MESSAGES = {
  CREATED: 'Creado exitosamente',
  UPDATED: 'Actualizado exitosamente',
  DELETED: 'Eliminado exitosamente',
  LOGIN_SUCCESS: 'Login exitoso',
  LOGOUT_SUCCESS: 'Sesión cerrada exitosamente',
  REGISTER_SUCCESS: 'Usuario registrado exitosamente',
  PASSWORD_CHANGED: 'Contraseña cambiada exitosamente',
  PROFILE_UPDATED: 'Perfil actualizado exitosamente'
} as const;

/**
 * Mensajes de error generales
 */
export const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Error de conexión. Verifique su conexión a internet.',
  SERVER_ERROR: 'Error interno del servidor',
  UNAUTHORIZED: 'No autorizado',
  FORBIDDEN: 'Acceso denegado',
  NOT_FOUND: 'Recurso no encontrado',
  VALIDATION_ERROR: 'Error de validación',
  UNKNOWN_ERROR: 'Error desconocido',
  TOKEN_EXPIRED: 'Sesión expirada. Por favor, inicie sesión nuevamente.',
  INVALID_CREDENTIALS: 'Credenciales inválidas',
  EMAIL_ALREADY_EXISTS: 'El email ya está registrado',
  SKU_ALREADY_EXISTS: 'El SKU ya existe',
  INSUFFICIENT_STOCK: 'Stock insuficiente'
} as const;
