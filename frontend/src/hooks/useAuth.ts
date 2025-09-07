import { useCallback } from 'react';
import { useMutation } from './useApi';
import { authService } from '../services/authService';
import { LoginDTO, RegisterDTO, AuthResponse } from '../types/auth';

/**
 * Hook para manejar operaciones de autenticación
 */
export const useAuth = () => {
  const { loading, error, mutate, reset } = useMutation<AuthResponse, LoginDTO>();

  const login = useCallback((credentials: LoginDTO) => {
    return mutate(authService.login, credentials);
  }, [mutate]);

  return { 
    loading, 
    error, 
    login, 
    reset 
  };
};

/**
 * Hook para manejar registro de usuarios
 */
export const useRegister = () => {
  const { loading, error, mutate, reset } = useMutation<AuthResponse, RegisterDTO>();

  const register = useCallback((userData: RegisterDTO) => {
    return mutate(authService.register, userData);
  }, [mutate]);

  return { 
    loading, 
    error, 
    register, 
    reset 
  };
};

/**
 * Hook para manejar logout
 */
export const useLogout = () => {
  const { loading, error, mutate, reset } = useMutation<void, void>();

  const logout = useCallback(() => {
    return mutate(authService.logout, undefined);
  }, [mutate]);

  return { 
    loading, 
    error, 
    logout, 
    reset 
  };
};
