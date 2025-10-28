import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User } from '../types/auth';
import { authService } from '../services/authService';

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<{ success: boolean; message: string }>;
  register: (userData: { email: string; password: string; firstName: string; lastName: string }) => Promise<{ success: boolean; message: string }>;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    // Inicializar el estado con el usuario del localStorage si existe
    const savedUser = authService.getCurrentUser();
    console.log('AuthContext: Usuario inicial:', savedUser);
    return savedUser;
  });
  const [loading, setLoading] = useState(false);

  // Verificar autenticación al cargar
  useEffect(() => {
    const checkAuth = () => {
      setLoading(true);
      try {
        const currentUser = authService.getCurrentUser();
        const isAuth = authService.isAuthenticated();
        
        console.log('AuthContext: Verificando autenticación:', { currentUser, isAuth });
        
        if (currentUser && isAuth) {
          // Verificar si el token ha expirado
          const token = localStorage.getItem('auth-token');
          if (token && authService.isTokenExpired(token)) {
            console.log('AuthContext: Token expirado, cerrando sesión');
            authService.clearSession();
            setUser(null);
            return;
          }
          
          setUser(currentUser);
          console.log('AuthContext: Usuario autenticado:', currentUser);
        } else {
          console.log('AuthContext: No hay usuario válido, limpiando sesión');
          setUser(null);
          authService.clearSession();
        }
      } catch (error) {
        console.error('Error checking authentication:', error);
        setUser(null);
        authService.clearSession();
      } finally {
        setLoading(false);
      }
    };

    checkAuth();

    // Configurar verificación periódica del token (cada 5 minutos)
    const interval = setInterval(() => {
      const token = localStorage.getItem('auth-token');
      if (token && authService.isTokenExpired(token)) {
        console.log('AuthContext: Token expirado en verificación periódica, cerrando sesión');
        authService.clearSession();
        setUser(null);
      }
    }, 5 * 60 * 1000); // 5 minutos

    return () => clearInterval(interval);
  }, []);

  const login = async (email: string, password: string) => {
    try {
      const response = await authService.login({ email, password });
      if (response.data?.user) {
        setUser(response.data.user);
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Error en el login' };
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error en el login';
      return { success: false, message };
    }
  };

  const register = async (userData: { email: string; password: string; firstName: string; lastName: string }) => {
    try {
      const response = await authService.register(userData);
      if (response.data?.user) {
        // NO hacer login automático después del registro
        // setUser(response.data.user); // ← Comentado para evitar login automático
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Error en el registro' };
      }
    } catch (error: any) {
      const message = error.response?.data?.message || 'Error en el registro';
      return { success: false, message };
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    login,
    register,
    logout,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

