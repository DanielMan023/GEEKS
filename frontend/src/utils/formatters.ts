/**
 * Utilidades para formatear datos
 */

/**
 * Formatea un precio como moneda
 */
export const formatPrice = (price: number, currency: string = 'USD'): string => {
  return new Intl.NumberFormat('es-ES', {
    style: 'currency',
    currency: currency
  }).format(price);
};

/**
 * Formatea un número con separadores de miles
 */
export const formatNumber = (number: number): string => {
  return new Intl.NumberFormat('es-ES').format(number);
};

/**
 * Formatea una fecha
 */
export const formatDate = (date: Date | string, options?: Intl.DateTimeFormatOptions): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date;
  
  const defaultOptions: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  };

  return new Intl.DateTimeFormat('es-ES', { ...defaultOptions, ...options }).format(dateObj);
};

/**
 * Formatea una fecha y hora
 */
export const formatDateTime = (date: Date | string): string => {
  return formatDate(date, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

/**
 * Formatea un texto truncando si es muy largo
 */
export const truncateText = (text: string, maxLength: number): string => {
  if (text.length <= maxLength) return text;
  return text.substring(0, maxLength) + '...';
};

/**
 * Formatea un SKU para mostrar
 */
export const formatSku = (sku: string): string => {
  return sku.toUpperCase();
};

/**
 * Formatea un nombre capitalizando la primera letra
 */
export const formatName = (name: string): string => {
  return name
    .toLowerCase()
    .split(' ')
    .map(word => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');
};

/**
 * Formatea un email para mostrar (oculta parte del dominio)
 */
export const formatEmail = (email: string): string => {
  const [username, domain] = email.split('@');
  if (username.length <= 2) return email;
  
  const hiddenUsername = username.substring(0, 2) + '*'.repeat(username.length - 2);
  return `${hiddenUsername}@${domain}`;
};

/**
 * Formatea un tamaño de archivo
 */
export const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes';
  
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

/**
 * Formatea un porcentaje
 */
export const formatPercentage = (value: number, decimals: number = 0): string => {
  return `${value.toFixed(decimals)}%`;
};

/**
 * Formatea un peso
 */
export const formatWeight = (weight: number, unit: string = 'kg'): string => {
  return `${weight.toFixed(2)} ${unit}`;
};

/**
 * Formatea dimensiones
 */
export const formatDimensions = (length: number, width: number, height: number, unit: string = 'cm'): string => {
  return `${length} × ${width} × ${height} ${unit}`;
};
