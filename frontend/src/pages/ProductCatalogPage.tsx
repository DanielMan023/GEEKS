import React from 'react';
import { ProductCatalog } from '../components/products/ProductCatalog';
import { ProductList } from '../types/product';

const ProductCatalogPage: React.FC = () => {
  const handleProductClick = (product: ProductList) => {
    // Aquí puedes navegar al detalle del producto o abrir un modal
    console.log('Producto clickeado:', product);
    // Por ahora solo mostramos en consola
    alert(`Producto: ${product.name}\nPrecio: ${new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP'
    }).format(product.price)}`);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <ProductCatalog onProductClick={handleProductClick} />
    </div>
  );
};

export default ProductCatalogPage;
