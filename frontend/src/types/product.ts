export interface Product {
  id: number;
  name: string;
  description: string;
  shortDescription?: string;
  price: number;
  discountPrice?: number;
  stock: number;
  minStock: number;
  sku: string;
  mainImage?: string;
  images: string[];
  categoryId: number;
  categoryName: string;
  brand?: string;
  state: string;
  isFeatured: boolean;
  weight: number;
  length: number;
  width: number;
  height: number;
  createdAtDateTime: string;
  updatedAtDateTime?: string;
}

export interface ProductList {
  id: number;
  name: string;
  shortDescription?: string;
  price: number;
  discountPrice?: number;
  stock: number;
  mainImage?: string;
  categoryName: string;
  brand?: string;
  isFeatured: boolean;
  state: string;
}

export interface ProductFilter {
  searchTerm?: string;
  categoryId?: number;
  brand?: string;
  minPrice?: number;
  maxPrice?: number;
  inStockOnly?: boolean;
  featuredOnly?: boolean;
  sortBy?: string;
  sortOrder?: string;
  page?: number;
  pageSize?: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateProduct {
  name: string;
  description: string;
  shortDescription?: string;
  price: number;
  discountPrice?: number;
  stock: number;
  minStock?: number;
  sku: string;
  mainImage?: string;
  images: string[];
  categoryId: number;
  brand?: string;
  isFeatured?: boolean;
  weight?: number;
  length?: number;
  width?: number;
  height?: number;
}

export interface UpdateProduct {
  name?: string;
  description?: string;
  shortDescription?: string;
  price?: number;
  discountPrice?: number;
  stock?: number;
  minStock?: number;
  sku?: string;
  mainImage?: string;
  images?: string[];
  categoryId?: number;
  brand?: string;
  isFeatured?: boolean;
  weight?: number;
  length?: number;
  width?: number;
  height?: number;
  state?: string;
}

// Aliases para coincidir con los nombres del backend
export type ProductResponseDTO = Product;
export type ProductListDTO = ProductList;
export type ProductFilterDTO = ProductFilter;
export type CreateProductDTO = CreateProduct;
export type UpdateProductDTO = UpdateProduct;

// Tipos para categorías
export interface CategoryListDTO {
  id: number;
  name: string;
  description?: string;
  image?: string;
  productCount: number;
  state: string;
}