import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { LoginCredentials, RegisterCredentials, AuthState } from '../types/auth';
import { authService } from '../services/authService';

export const useAuth = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
  });
  const [isChecking, setIsChecking] = useState<boolean>(true);

  const checkAuth = useCallback(async (): Promise<void> => {
    try {
      const isAuthenticated = await authService.checkAuth();
      if (isAuthenticated) {
        // Aquí podrías obtener los datos del usuario si es necesario
        setAuthState({
          isAuthenticated: true,
          user: null, // O obtener el usuario actual
        });
      }
    } catch (error) {
      console.error('Error checking auth:', error);
    } finally {
      setIsChecking(false);
    }
  }, []);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  const login = useCallback(async (credentials: LoginCredentials): Promise<void> => {
    try {
      setLoading(true);
      setError(null);
      const response = await authService.login(credentials);
      
      if (response.success && response.data.user) {
        setAuthState({
          isAuthenticated: true,
          user: response.data.user,
        });
        
        // Redirigir según el rol del usuario
        const roleScope = response.data.user.role.scope.toLowerCase();
        if (roleScope === 'all') {
          navigate('/admin/dashboard', { replace: true });
        } else {
          navigate('/dashboard', { replace: true });
        }
      }
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError('Error al iniciar sesión');
      }
    } finally {
      setLoading(false);
    }
  }, [navigate]);

  const register = useCallback(async (credentials: RegisterCredentials): Promise<void> => {
    try {
      setLoading(true);
      setError(null);
      const response = await authService.register(credentials);
      
      if (response.success) {
        // Después del registro exitoso, hacer login automáticamente
        await login({ email: credentials.email, password: credentials.password });
      }
    } catch (error) {
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError('Error al registrar usuario');
      }
    } finally {
      setLoading(false);
    }
  }, [login]);

  const logout = useCallback(async (): Promise<void> => {
    try {
      await authService.logout();
      setAuthState({
        isAuthenticated: false,
        user: null,
      });
      navigate('/login', { replace: true });
    } catch (error) {
      console.error('Error en logout:', error);
    }
  }, [navigate]);

  return {
    authState,
    loading,
    error,
    isChecking,
    login,
    register,
    logout,
    checkAuth,
  };
};
