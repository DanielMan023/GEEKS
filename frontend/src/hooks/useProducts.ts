import { useCallback } from 'react';
import { useApi, useMutation } from './useApi';
import { productService } from '../services/productService';
import { ProductListDTO, ProductResponseDTO, ProductFilterDTO, CreateProductDTO, UpdateProductDTO, PaginatedResponse } from '../types/product';

/**
 * Hook para manejar operaciones relacionadas con productos
 */
export const useProducts = () => {
  const { data, loading, error, execute } = useApi<PaginatedResponse<ProductListDTO>>();

  const getProducts = useCallback((filter: ProductFilterDTO) => {
    return execute(() => productService.getProducts(filter));
  }, [execute]);

  return { 
    products: data, 
    loading, 
    error, 
    getProducts 
  };
};

/**
 * Hook para obtener un producto específico
 */
export const useProduct = (id?: number) => {
  const { data, loading, error, execute } = useApi<ProductResponseDTO>();

  const getProduct = useCallback((productId: number) => {
    return execute(() => productService.getProduct(productId));
  }, [execute]);

  return { 
    product: data, 
    loading, 
    error, 
    getProduct 
  };
};

/**
 * Hook para obtener productos destacados
 */
export const useFeaturedProducts = () => {
  const { data, loading, error, execute } = useApi<ProductListDTO[]>();

  const getFeaturedProducts = useCallback((limit: number = 10) => {
    return execute(() => productService.getFeaturedProducts(limit));
  }, [execute]);

  return { 
    featuredProducts: data, 
    loading, 
    error, 
    getFeaturedProducts 
  };
};

/**
 * Hook para crear productos
 */
export const useCreateProduct = () => {
  const { loading, error, mutate, reset } = useMutation<ProductResponseDTO, CreateProductDTO>();

  const createProduct = useCallback((productData: CreateProductDTO) => {
    return mutate(productService.createProduct, productData);
  }, [mutate]);

  return { 
    loading, 
    error, 
    createProduct, 
    reset 
  };
};

/**
 * Hook para actualizar productos
 */
export const useUpdateProduct = () => {
  const { loading, error, mutate, reset } = useMutation<ProductResponseDTO, { id: number; data: UpdateProductDTO }>();

  const updateProduct = useCallback((id: number, productData: UpdateProductDTO) => {
    return mutate(
      (params: { id: number; data: UpdateProductDTO }) => 
        productService.updateProduct(params.id, params.data),
      { id, data: productData }
    );
  }, [mutate]);

  return { 
    loading, 
    error, 
    updateProduct, 
    reset 
  };
};

/**
 * Hook para eliminar productos
 */
export const useDeleteProduct = () => {
  const { loading, error, mutate, reset } = useMutation<void, number>();

  const deleteProduct = useCallback((id: number) => {
    return mutate(productService.deleteProduct, id);
  }, [mutate]);

  return { 
    loading, 
    error, 
    deleteProduct, 
    reset 
  };
};

/**
 * Hook para validar SKU
 */
export const useValidateSku = () => {
  const { data, loading, error, execute } = useApi<{ isUnique: boolean; message: string }>();

  const validateSku = useCallback((sku: string, excludeId?: number) => {
    return execute(() => productService.validateSku(sku, excludeId));
  }, [execute]);

  return { 
    validation: data, 
    loading, 
    error, 
    validateSku 
  };
};
