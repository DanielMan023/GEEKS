import React from 'react';
import { ProductList } from '../../types/product';

interface ProductCardProps {
  product: ProductList;
  onClick?: (product: ProductList) => void;
  className?: string;
}

export const ProductCard: React.FC<ProductCardProps> = ({ 
  product, 
  onClick, 
  className = '' 
}) => {
  const handleClick = () => {
    if (onClick) {
      onClick(product);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(price);
  };

  const getStockStatus = (stock: number) => {
    if (stock === 0) return { text: 'Sin stock', color: 'text-red-500' };
    if (stock <= 5) return { text: `Solo ${stock} disponibles`, color: 'text-orange-500' };
    return { text: `${stock} disponibles`, color: 'text-green-500' };
  };

  const stockStatus = getStockStatus(product.stock);

  return (
    <div 
      className={`bg-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-300 overflow-hidden cursor-pointer group ${className}`}
      onClick={handleClick}
    >
      {/* Imagen del producto */}
      <div className="relative h-48 bg-gray-100 overflow-hidden">
        {product.mainImage ? (
          <img
            src={product.mainImage}
            alt={product.name}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
            onError={(e) => {
              const target = e.target as HTMLImageElement;
              target.src = 'https://via.placeholder.com/400x400/6B7280/FFFFFF?text=Sin+Imagen';
            }}
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center bg-gray-200">
            <span className="text-gray-400 text-sm">Sin imagen</span>
          </div>
        )}
        
        {/* Badge de destacado */}
        {product.isFeatured && (
          <div className="absolute top-2 left-2 bg-yellow-400 text-yellow-900 px-2 py-1 rounded-full text-xs font-semibold">
            ⭐ Destacado
          </div>
        )}
        
        {/* Badge de descuento */}
        {product.discountPrice && product.discountPrice < product.price && (
          <div className="absolute top-2 right-2 bg-red-500 text-white px-2 py-1 rounded-full text-xs font-semibold">
            -{Math.round(((product.price - product.discountPrice) / product.price) * 100)}%
          </div>
        )}
      </div>

      {/* Contenido de la tarjeta */}
      <div className="p-4">
        {/* Categoría */}
        <div className="text-xs text-gray-500 mb-1">
          {product.categoryName}
        </div>

        {/* Nombre del producto */}
        <h3 className="font-semibold text-gray-900 text-sm mb-2 line-clamp-2 group-hover:text-green-600 transition-colors">
          {product.name}
        </h3>

        {/* Marca */}
        {product.brand && (
          <div className="text-xs text-gray-600 mb-2">
            {product.brand}
          </div>
        )}

        {/* Descripción corta */}
        {product.shortDescription && (
          <p className="text-xs text-gray-600 mb-3 line-clamp-2">
            {product.shortDescription}
          </p>
        )}

        {/* Precio */}
        <div className="flex items-center gap-2 mb-2">
          {product.discountPrice && product.discountPrice < product.price ? (
            <>
              <span className="text-lg font-bold text-green-600">
                {formatPrice(product.discountPrice)}
              </span>
              <span className="text-sm text-gray-400 line-through">
                {formatPrice(product.price)}
              </span>
            </>
          ) : (
            <span className="text-lg font-bold text-gray-900">
              {formatPrice(product.price)}
            </span>
          )}
        </div>

        {/* Stock */}
        <div className={`text-xs font-medium ${stockStatus.color}`}>
          {stockStatus.text}
        </div>
      </div>
    </div>
  );
};
