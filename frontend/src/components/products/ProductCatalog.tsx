import React, { useState, useEffect } from 'react';
import { ProductCard } from './ProductCard';
import { ProductFilters } from './ProductFilters';
import { productService } from '../../services/productService';
import { ProductList, ProductFilter, PaginatedResponse } from '../../types/product';

interface ProductCatalogProps {
  className?: string;
  onProductClick?: (product: ProductList) => void;
}

export const ProductCatalog: React.FC<ProductCatalogProps> = ({
  className = '',
  onProductClick
}) => {
  const [products, setProducts] = useState<ProductList[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pagination, setPagination] = useState<PaginatedResponse<ProductList> | null>(null);
  const [filters, setFilters] = useState<ProductFilter>({
    page: 1,
    pageSize: 12
  });

  // Estados para filtros
  const [categories, setCategories] = useState<Array<{ id: number; name: string }>>([]);
  const [brands, setBrands] = useState<string[]>([]);

  // Cargar productos
  const loadProducts = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await productService.getProducts(filters);
      setProducts(response.data);
      setPagination(response);
    } catch (err) {
      console.error('Error cargando productos:', err);
      setError('Error al cargar los productos. Por favor, intenta de nuevo.');
    } finally {
      setLoading(false);
    }
  };

  // Cargar categorías y marcas para filtros
  const loadFilterOptions = async () => {
    try {
      // Cargar categorías (esto requeriría un endpoint adicional)
      // Por ahora usamos datos de ejemplo
      setCategories([
        { id: 1, name: 'Electrónicos' },
        { id: 2, name: 'Ropa' },
        { id: 3, name: 'Hogar' },
        { id: 4, name: 'Deportes' }
      ]);

      // Cargar marcas únicas de los productos
      const allProducts = await productService.getProducts({ pageSize: 1000 });
      const uniqueBrands = [...new Set(allProducts.data.map(p => p.brand).filter(Boolean))];
      setBrands(uniqueBrands as string[]);
    } catch (err) {
      console.error('Error cargando opciones de filtros:', err);
    }
  };

  // Efectos
  useEffect(() => {
    loadProducts();
  }, [filters]);

  useEffect(() => {
    loadFilterOptions();
  }, []);

  // Manejadores
  const handleFiltersChange = (newFilters: ProductFilter) => {
    setFilters(newFilters);
  };

  const handlePageChange = (page: number) => {
    setFilters(prev => ({ ...prev, page }));
  };

  const handleProductClick = (product: ProductList) => {
    if (onProductClick) {
      onProductClick(product);
    }
  };

  // Renderizado de paginación
  const renderPagination = () => {
    if (!pagination || pagination.totalPages <= 1) return null;

    const pages = [];
    const currentPage = pagination.page;
    const totalPages = pagination.totalPages;

    // Mostrar máximo 5 páginas alrededor de la actual
    let startPage = Math.max(1, currentPage - 2);
    let endPage = Math.min(totalPages, currentPage + 2);

    if (endPage - startPage < 4) {
      if (startPage === 1) {
        endPage = Math.min(totalPages, startPage + 4);
      } else {
        startPage = Math.max(1, endPage - 4);
      }
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return (
      <div className="flex items-center justify-center space-x-2 mt-8">
        {/* Botón anterior */}
        <button
          onClick={() => handlePageChange(currentPage - 1)}
          disabled={!pagination.hasPreviousPage}
          className="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Anterior
        </button>

        {/* Páginas */}
        {pages.map(page => (
          <button
            key={page}
            onClick={() => handlePageChange(page)}
            className={`px-3 py-2 text-sm font-medium rounded-lg ${
              page === currentPage
                ? 'bg-green-600 text-white'
                : 'text-gray-500 bg-white border border-gray-300 hover:bg-gray-50'
            }`}
          >
            {page}
          </button>
        ))}

        {/* Botón siguiente */}
        <button
          onClick={() => handlePageChange(currentPage + 1)}
          disabled={!pagination.hasNextPage}
          className="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Siguiente
        </button>
      </div>
    );
  };

  return (
    <div className={`min-h-screen bg-gray-50 ${className}`}>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">Catálogo de Productos</h1>
          <p className="text-gray-600">
            {pagination ? `${pagination.totalCount} productos encontrados` : 'Cargando productos...'}
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          {/* Filtros */}
          <div className="lg:col-span-1">
            <ProductFilters
              filters={filters}
              onFiltersChange={handleFiltersChange}
              categories={categories}
              brands={brands}
            />
          </div>

          {/* Productos */}
          <div className="lg:col-span-3">
            {/* Estado de carga */}
            {loading && (
              <div className="flex items-center justify-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-600"></div>
              </div>
            )}

            {/* Estado de error */}
            {error && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <svg className="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                      <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                    </svg>
                  </div>
                  <div className="ml-3">
                    <p className="text-sm text-red-800">{error}</p>
                  </div>
                </div>
              </div>
            )}

            {/* Grid de productos */}
            {!loading && !error && (
              <>
                {products.length === 0 ? (
                  <div className="text-center py-12">
                    <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                    </svg>
                    <h3 className="mt-2 text-sm font-medium text-gray-900">No se encontraron productos</h3>
                    <p className="mt-1 text-sm text-gray-500">
                      Intenta ajustar los filtros de búsqueda.
                    </p>
                  </div>
                ) : (
                  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                    {products.map((product) => (
                      <ProductCard
                        key={product.id}
                        product={product}
                        onClick={handleProductClick}
                      />
                    ))}
                  </div>
                )}

                {/* Paginación */}
                {renderPagination()}
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
