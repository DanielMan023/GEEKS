import axios from 'axios';
import { LoginDTO, RegisterDTO, AuthResponse } from '../types/auth';

const API_URL = '/api/auth';

// Configurar axios para incluir el token en las cabeceras
axios.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth-token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor de respuesta para manejar tokens expirados
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expirado o inválido - solo limpiar localStorage
      // No redirigir automáticamente para evitar loops
      console.log('Token expirado o inválido');
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginDTO): Promise<AuthResponse> {
    const response = await axios.post(`${API_URL}/login`, credentials);
    console.log('authService: login - response.data:', response.data);
    console.log('authService: login - response.data.data:', response.data.data);
    console.log('authService: login - response.data.data?.token:', response.data.data?.token);
    
    if (response.data.data?.token) {
      console.log('authService: login - Guardando token y usuario en localStorage');
      localStorage.setItem('auth-token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
      console.log('authService: login - Token guardado:', localStorage.getItem('auth-token') ? 'exitoso' : 'fallido');
      console.log('authService: login - Usuario guardado:', localStorage.getItem('user') ? 'exitoso' : 'fallido');
    } else {
      console.log('authService: login - No se encontró token en la respuesta');
    }
    return response.data;
  },

  async register(userData: RegisterDTO): Promise<AuthResponse> {
    const response = await axios.post(`${API_URL}/register`, userData);
    if (response.data.data?.token) {
      localStorage.setItem('auth-token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
    }
    return response.data;
  },

  async logout(): Promise<void> {
    try {
      await axios.post(`${API_URL}/logout`);
    } catch (error) {
      console.error('Error en logout:', error);
    } finally {
      localStorage.removeItem('auth-token');
      localStorage.removeItem('user');
    }
  },

  getCurrentUser() {
    try {
      const userStr = localStorage.getItem('user');
      const token = localStorage.getItem('auth-token');
      
      console.log('authService: getCurrentUser - userStr:', userStr);
      console.log('authService: getCurrentUser - token:', token ? 'exists' : 'null');
      
      if (!userStr || !token) {
        console.log('authService: getCurrentUser - No hay userStr o token');
        return null;
      }
      
      const user = JSON.parse(userStr);
      console.log('authService: getCurrentUser - parsed user:', user);
      
      // Validar que el usuario tenga los campos básicos
      if (!user || !user.id || !user.email) {
        console.log('authService: getCurrentUser - Usuario inválido, faltan campos');
        return null;
      }
      
      console.log('authService: getCurrentUser - Usuario válido retornado');
      return user;
    } catch (error) {
      console.error('Error parsing user from localStorage:', error);
      return null;
    }
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('auth-token');
  },

  async validateToken(): Promise<boolean> {
    try {
      const token = localStorage.getItem('auth-token');
      if (!token) return false;
      
      // Hacer una petición al servidor para validar el token
      await axios.get(`${API_URL}/validate`);
      return true;
    } catch (error) {
      console.error('Token validation failed:', error);
      return false;
    }
  },

  clearSession() {
    localStorage.removeItem('auth-token');
    localStorage.removeItem('user');
  },

  isTokenExpired(token: string): boolean {
    try {
      // Decodificar el token JWT para obtener la fecha de expiración
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000; // Convertir a milisegundos
      const currentTime = Date.now();
      
      // El token ha expirado si la hora actual es mayor que la hora de expiración
      return currentTime >= expirationTime;
    } catch (error) {
      console.error('Error al verificar expiración del token:', error);
      // Si hay error al decodificar, considerar el token como expirado por seguridad
      return true;
    }
  }
};

