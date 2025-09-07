/**
 * Constantes relacionadas con la API
 */
export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: '/api/auth/login',
    REGISTER: '/api/auth/register',
    LOGOUT: '/api/auth/logout',
    VALIDATE: '/api/auth/validate'
  },
  PRODUCTS: {
    BASE: '/api/product',
    FEATURED: '/api/product/featured',
    CATEGORIES: '/api/product/categories',
    LOW_STOCK: '/api/product/low-stock',
    VALIDATE_SKU: '/api/product/validate-sku',
    CLEAR_DEMO: '/api/product/demo/clear'
  },
  CART: {
    BASE: '/api/cart',
    ADD_ITEM: '/api/cart/add',
    REMOVE_ITEM: '/api/cart/remove',
    UPDATE_QUANTITY: '/api/cart/update-quantity',
    CLEAR: '/api/cart/clear'
  },
  CATEGORIES: {
    BASE: '/api/category'
  },
  FILES: {
    UPLOAD: '/api/file/upload'
  }
} as const;

/**
 * Configuración de paginación
 */
export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 10,
  MAX_PAGE_SIZE: 100,
  MIN_PAGE: 1
} as const;

/**
 * Configuración de timeouts
 */
export const TIMEOUTS = {
  API_REQUEST: 30000, // 30 segundos
  TOKEN_VALIDATION: 5000, // 5 segundos
  NOTIFICATION_DURATION: 5000 // 5 segundos
} as const;

/**
 * Configuración de archivos
 */
export const FILE_CONFIG = {
  MAX_SIZE: 5 * 1024 * 1024, // 5MB
  ALLOWED_TYPES: ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
  ALLOWED_EXTENSIONS: ['.jpg', '.jpeg', '.png', '.gif', '.webp']
} as const;
