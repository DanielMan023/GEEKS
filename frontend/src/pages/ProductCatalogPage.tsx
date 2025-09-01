import React from 'react';
import { ProductCatalog } from '../components/products/ProductCatalog';
import { ProductList } from '../types/product';
import Sidebar from '../components/Sidebar';
import { useSidebar } from '../contexts/SidebarContext';

const ProductCatalogPage: React.FC = () => {
  const { isCollapsed } = useSidebar();
  
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
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className={`flex-1 transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <ProductCatalog onProductClick={handleProductClick} />
      </div>
    </div>
  );
};

export default ProductCatalogPage;
