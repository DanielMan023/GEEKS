import { BaseService } from './base/BaseService';
import { 
  ProductListDTO, 
  ProductResponseDTO, 
  ProductFilterDTO, 
  CreateProductDTO, 
  UpdateProductDTO, 
  PaginatedResponse,
  CategoryListDTO 
} from '../types/product';

/**
 * Servicio para operaciones relacionadas con productos
 */
export class ProductService extends BaseService {
  constructor() {
    super('/api/product');
  }

  /**
   * Obtiene productos con filtros y paginación
   */
  async getProducts(filter: ProductFilterDTO): Promise<PaginatedResponse<ProductListDTO>> {
    return this.get<PaginatedResponse<ProductListDTO>>('', filter);
  }

  /**
   * Obtiene un producto por su ID
   */
  async getProduct(id: number): Promise<ProductResponseDTO> {
    return this.get<ProductResponseDTO>(`/${id}`);
  }

  /**
   * Crea un nuevo producto
   */
  async createProduct(productData: CreateProductDTO): Promise<ProductResponseDTO> {
    return this.post<ProductResponseDTO>('', productData);
  }

  /**
   * Actualiza un producto existente
   */
  async updateProduct(id: number, productData: UpdateProductDTO): Promise<ProductResponseDTO> {
    return this.put<ProductResponseDTO>(`/${id}`, productData);
  }

  /**
   * Elimina un producto
   */
  async deleteProduct(id: number): Promise<void> {
    return this.delete<void>(`/${id}`);
  }

  /**
   * Obtiene productos destacados
   */
  async getFeaturedProducts(limit: number = 10): Promise<ProductListDTO[]> {
    return this.get<ProductListDTO[]>(`/featured?limit=${limit}`);
  }

  /**
   * Obtiene categorías
   */
  async getCategories(): Promise<CategoryListDTO[]> {
    return this.get<CategoryListDTO[]>('/categories');
  }

  /**
   * Obtiene productos con stock bajo
   */
  async getLowStockProducts(): Promise<ProductListDTO[]> {
    return this.get<ProductListDTO[]>('/low-stock');
  }

  /**
   * Actualiza el stock de un producto
   */
  async updateStock(id: number, stock: number): Promise<void> {
    return this.put<void>(`/${id}/stock`, stock);
  }

  /**
   * Reduce el stock de un producto
   */
  async reduceStock(id: number, quantity: number): Promise<void> {
    return this.put<void>(`/${id}/reduce-stock`, quantity);
  }

  /**
   * Elimina productos demo
   */
  async clearDemoProducts(): Promise<{ deletedCount: number; message: string }> {
    return this.delete<{ deletedCount: number; message: string }>('/demo/clear');
  }

  /**
   * Valida si un SKU es único
   */
  async validateSku(sku: string, excludeId?: number): Promise<{ isUnique: boolean; message: string }> {
    const params = excludeId ? { sku, excludeId } : { sku };
    return this.get<{ isUnique: boolean; message: string }>('/validate-sku', params);
  }
}

// Instancia singleton del servicio
export const productService = new ProductService();