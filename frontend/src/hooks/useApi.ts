import { useState, useCallback } from 'react';

/**
 * Hook personalizado para manejar llamadas a API
 * @template T Tipo de datos que devuelve la API
 */
export const useApi = <T>() => {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const execute = useCallback(async (apiCall: () => Promise<T>) => {
    setLoading(true);
    setError(null);
    try {
      const result = await apiCall();
      setData(result);
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const reset = useCallback(() => {
    setData(null);
    setError(null);
    setLoading(false);
  }, []);

  return { 
    data, 
    loading, 
    error, 
    execute, 
    reset 
  };
};

/**
 * Hook para manejar operaciones de mutación (POST, PUT, DELETE)
 * @template T Tipo de datos que devuelve la API
 * @template P Tipo de parámetros que recibe la función
 */
export const useMutation = <T, P = void>() => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const mutate = useCallback(async (apiCall: (params: P) => Promise<T>, params: P) => {
    setLoading(true);
    setError(null);
    try {
      const result = await apiCall(params);
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const reset = useCallback(() => {
    setError(null);
    setLoading(false);
  }, []);

  return { 
    loading, 
    error, 
    mutate, 
    reset 
  };
};
