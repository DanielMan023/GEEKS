import axios from 'axios';
import { Product, ProductList, ProductFilter, PaginatedResponse, CreateProduct, UpdateProduct } from '../types/product';

const API_URL = '/api/Product';

export const productService = {
  // Obtener productos con filtros y paginación
  async getProducts(filter: ProductFilter = {}): Promise<PaginatedResponse<ProductList>> {
    const params = new URLSearchParams();
    
    if (filter.searchTerm) params.append('searchTerm', filter.searchTerm);
    if (filter.categoryId) params.append('categoryId', filter.categoryId.toString());
    if (filter.brand) params.append('brand', filter.brand);
    if (filter.minPrice) params.append('minPrice', filter.minPrice.toString());
    if (filter.maxPrice) params.append('maxPrice', filter.maxPrice.toString());
    if (filter.inStockOnly) params.append('inStockOnly', filter.inStockOnly.toString());
    if (filter.featuredOnly) params.append('featuredOnly', filter.featuredOnly.toString());
    if (filter.sortBy) params.append('sortBy', filter.sortBy);
    if (filter.sortOrder) params.append('sortOrder', filter.sortOrder);
    if (filter.page) params.append('page', filter.page.toString());
    if (filter.pageSize) params.append('pageSize', filter.pageSize.toString());

    const response = await axios.get(`${API_URL}?${params.toString()}`);
    return response.data;
  },

  // Obtener un producto por ID
  async getProduct(id: number): Promise<Product> {
    const response = await axios.get(`${API_URL}/${id}`);
    return response.data;
  },

  // Crear un nuevo producto
  async createProduct(product: CreateProduct): Promise<Product> {
    const response = await axios.post(API_URL, product);
    return response.data;
  },

  // Actualizar un producto
  async updateProduct(id: number, product: UpdateProduct): Promise<Product> {
    const response = await axios.put(`${API_URL}/${id}`, product);
    return response.data;
  },

  // Eliminar un producto (soft delete)
  async deleteProduct(id: number): Promise<void> {
    await axios.delete(`${API_URL}/${id}`);
  },

  // Obtener productos destacados
  async getFeaturedProducts(): Promise<ProductList[]> {
    const response = await axios.get(`${API_URL}/featured`);
    return response.data;
  }
};
