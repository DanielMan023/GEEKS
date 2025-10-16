import React from 'react';
import { ProductCatalog } from '../components/products/ProductCatalog';
import { ProductList } from '../types/product';
import Sidebar from '../components/Sidebar';
import { useSidebar } from '../contexts/SidebarContext';
import ChatbotWidget from '../components/chatbot/ChatbotWidget';

const ProductCatalogPage: React.FC = () => {
  const { isCollapsed } = useSidebar();
  
  const handleProductClick = (product: ProductList) => {
    // Aquí puedes navegar al detalle del producto o abrir un modal
    console.log('Producto clickeado:', product);
    // Por ahora solo mostramos en consola, sin alertas molestas
  };

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className={`flex-1 overflow-y-auto transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <ProductCatalog onProductClick={handleProductClick} />
      </div>
      <ChatbotWidget />
    </div>
  );
};

export default ProductCatalogPage;
