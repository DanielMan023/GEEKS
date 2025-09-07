import { BaseService } from './base/BaseService';
import { LoginDTO, RegisterDTO, AuthResponse, User } from '../types/auth';

/**
 * Servicio refactorizado para operaciones de autenticación
 */
export class AuthServiceRefactored extends BaseService {
  constructor() {
    super('/api/auth');
  }

  /**
   * Inicia sesión de usuario
   */
  async login(credentials: LoginDTO): Promise<AuthResponse> {
    const response = await this.post<AuthResponse>('/login', credentials);
    
    // Guardar token y usuario en localStorage si la respuesta es exitosa
    if (response.data?.token && response.data?.user) {
      this.saveAuthData(response.data.token, response.data.user);
    }
    
    return response;
  }

  /**
   * Registra un nuevo usuario
   */
  async register(userData: RegisterDTO): Promise<AuthResponse> {
    const response = await this.post<AuthResponse>('/register', userData);
    
    // Guardar token y usuario en localStorage si la respuesta es exitosa
    if (response.data?.token && response.data?.user) {
      this.saveAuthData(response.data.token, response.data.user);
    }
    
    return response;
  }

  /**
   * Cierra sesión del usuario
   */
  async logout(): Promise<void> {
    try {
      await this.post<void>('/logout');
    } catch (error) {
      console.error('Error en logout:', error);
    } finally {
      this.clearAuthData();
    }
  }

  /**
   * Valida el token actual
   */
  async validateToken(): Promise<boolean> {
    try {
      const token = this.getToken();
      if (!token) return false;
      
      await this.get<{ valid: boolean }>('/validate');
      return true;
    } catch (error) {
      console.error('Token validation failed:', error);
      return false;
    }
  }

  /**
   * Obtiene el usuario actual del localStorage
   */
  getCurrentUser(): User | null {
    try {
      const userStr = localStorage.getItem('user');
      const token = this.getToken();
      
      if (!userStr || !token) {
        return null;
      }
      
      const user = JSON.parse(userStr);
      
      // Validar que el usuario tenga los campos básicos
      if (!user || !user.id || !user.email) {
        return null;
      }
      
      return user;
    } catch (error) {
      console.error('Error parsing user from localStorage:', error);
      return null;
    }
  }

  /**
   * Verifica si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  /**
   * Obtiene el token del localStorage
   */
  getToken(): string | null {
    return localStorage.getItem('auth-token');
  }

  /**
   * Verifica si el token ha expirado
   */
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

  /**
   * Limpia los datos de autenticación del localStorage
   */
  clearAuthData(): void {
    localStorage.removeItem('auth-token');
    localStorage.removeItem('user');
  }

  /**
   * Guarda los datos de autenticación en localStorage
   */
  private saveAuthData(token: string, user: User): void {
    localStorage.setItem('auth-token', token);
    localStorage.setItem('user', JSON.stringify(user));
  }
}

// Instancia singleton del servicio
export const authServiceRefactored = new AuthServiceRefactored();
