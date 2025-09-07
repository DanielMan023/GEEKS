import axios, { AxiosResponse, AxiosError } from 'axios';

/**
 * Clase base para servicios con funcionalidades comunes
 */
export abstract class BaseService {
  protected readonly baseURL: string;

  constructor(baseURL: string = '/api') {
    this.baseURL = baseURL;
  }

  /**
   * Maneja una petición HTTP y devuelve los datos o lanza una excepción
   */
  protected async handleRequest<T>(request: () => Promise<AxiosResponse<T>>): Promise<T> {
    try {
      const response = await request();
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Maneja errores de peticiones HTTP
   */
  private handleError(error: unknown): Error {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError;
      
      if (axiosError.response) {
        // El servidor respondió con un código de error
        const data = axiosError.response.data as any;
        const message = data?.message || `Error ${axiosError.response.status}: ${axiosError.response.statusText}`;
        return new Error(message);
      } else if (axiosError.request) {
        // La petición se hizo pero no se recibió respuesta
        return new Error('Error de conexión. Verifique su conexión a internet.');
      }
    }
    
    // Error desconocido
    return new Error('Error desconocido');
  }

  /**
   * Realiza una petición GET
   */
  protected async get<T>(endpoint: string, params?: Record<string, any>): Promise<T> {
    return this.handleRequest(() => 
      axios.get(`${this.baseURL}${endpoint}`, { params })
    );
  }

  /**
   * Realiza una petición POST
   */
  protected async post<T>(endpoint: string, data?: any): Promise<T> {
    return this.handleRequest(() => 
      axios.post(`${this.baseURL}${endpoint}`, data)
    );
  }

  /**
   * Realiza una petición PUT
   */
  protected async put<T>(endpoint: string, data?: any): Promise<T> {
    return this.handleRequest(() => 
      axios.put(`${this.baseURL}${endpoint}`, data)
    );
  }

  /**
   * Realiza una petición DELETE
   */
  protected async delete<T>(endpoint: string): Promise<T> {
    return this.handleRequest(() => 
      axios.delete(`${this.baseURL}${endpoint}`)
    );
  }

  /**
   * Realiza una petición PATCH
   */
  protected async patch<T>(endpoint: string, data?: any): Promise<T> {
    return this.handleRequest(() => 
      axios.patch(`${this.baseURL}${endpoint}`, data)
    );
  }
}
