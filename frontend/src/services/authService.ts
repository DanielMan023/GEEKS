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

export const authService = {
  async login(credentials: LoginDTO): Promise<AuthResponse> {
    const response = await axios.post(`${API_URL}/login`, credentials);
    if (response.data.data?.token) {
      localStorage.setItem('auth-token', response.data.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.data.user));
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
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('auth-token');
  }
};
