import { useState, useCallback } from 'react';

/**
 * Tipo para funciones de validación
 */
export type ValidationRule<T> = (value: T) => string | null;

/**
 * Hook para manejar validaciones de formularios
 * @template T Tipo de datos del formulario
 */
export const useValidation = <T extends Record<string, any>>(
  initialValues: T,
  validationRules: Partial<Record<keyof T, ValidationRule<any>[]>>
) => {
  const [values, setValues] = useState<T>(initialValues);
  const [errors, setErrors] = useState<Partial<Record<keyof T, string>>>({});
  const [touched, setTouched] = useState<Partial<Record<keyof T, boolean>>>({});

  /**
   * Valida un campo específico
   */
  const validateField = useCallback((field: keyof T, value: any) => {
    const rules = validationRules[field];
    if (!rules) return null;

    for (const rule of rules) {
      const error = rule(value);
      if (error) return error;
    }
    return null;
  }, [validationRules]);

  /**
   * Valida todos los campos
   */
  const validateAll = useCallback(() => {
    const newErrors: Partial<Record<keyof T, string>> = {};
    let isValid = true;

    Object.keys(validationRules).forEach((field) => {
      const fieldKey = field as keyof T;
      const error = validateField(fieldKey, values[fieldKey]);
      if (error) {
        newErrors[fieldKey] = error;
        isValid = false;
      }
    });

    setErrors(newErrors);
    return isValid;
  }, [values, validateField, validationRules]);

  /**
   * Actualiza el valor de un campo
   */
  const setValue = useCallback((field: keyof T, value: any) => {
    setValues(prev => ({ ...prev, [field]: value }));
    
    // Validar el campo si ya fue tocado
    if (touched[field]) {
      const error = validateField(field, value);
      setErrors(prev => ({ ...prev, [field]: error || undefined }));
    }
  }, [touched, validateField]);

  /**
   * Marca un campo como tocado
   */
  const setTouchedField = useCallback((field: keyof T) => {
    setTouched(prev => ({ ...prev, [field]: true }));
    
    // Validar el campo cuando se toca
    const error = validateField(field, values[field]);
    setErrors(prev => ({ ...prev, [field]: error || undefined }));
  }, [values, validateField]);

  /**
   * Resetea el formulario
   */
  const reset = useCallback(() => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
  }, [initialValues]);

  /**
   * Resetea solo los errores
   */
  const resetErrors = useCallback(() => {
    setErrors({});
  }, []);

  /**
   * Verifica si el formulario es válido
   */
  const isValid = Object.keys(errors).length === 0 && Object.values(errors).every(error => !error);

  return {
    values,
    errors,
    touched,
    isValid,
    setValue,
    setTouchedField,
    validateField,
    validateAll,
    reset,
    resetErrors
  };
};

/**
 * Reglas de validación comunes
 */
export const validationRules = {
  required: <T>(value: T): string | null => {
    if (value === null || value === undefined || value === '') {
      return 'Este campo es requerido';
    }
    return null;
  },

  email: (value: string): string | null => {
    if (!value) return null;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(value) ? null : 'El formato del email no es válido';
  },

  minLength: (min: number) => (value: string): string | null => {
    if (!value) return null;
    return value.length >= min ? null : `Debe tener al menos ${min} caracteres`;
  },

  maxLength: (max: number) => (value: string): string | null => {
    if (!value) return null;
    return value.length <= max ? null : `No puede exceder ${max} caracteres`;
  },

  min: (min: number) => (value: number): string | null => {
    if (value === null || value === undefined) return null;
    return value >= min ? null : `Debe ser mayor o igual a ${min}`;
  },

  max: (max: number) => (value: number): string | null => {
    if (value === null || value === undefined) return null;
    return value <= max ? null : `Debe ser menor o igual a ${max}`;
  },

  pattern: (regex: RegExp, message: string) => (value: string): string | null => {
    if (!value) return null;
    return regex.test(value) ? null : message;
  },

  sku: (value: string): string | null => {
    if (!value) return null;
    const skuRegex = /^[A-Z0-9-]+$/;
    return skuRegex.test(value) ? null : 'El SKU debe contener solo letras mayúsculas, números y guiones';
  },

  password: (value: string): string | null => {
    if (!value) return null;
    if (value.length < 6) return 'La contraseña debe tener al menos 6 caracteres';
    if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(value)) {
      return 'La contraseña debe contener al menos una letra minúscula, una mayúscula y un número';
    }
    return null;
  }
};
