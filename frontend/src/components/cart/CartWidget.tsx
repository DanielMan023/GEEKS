import React, { useState } from 'react';
import { useCart } from '../../contexts/CartContext';
import { CartItem } from './CartItem';
import PlaceholderImage from '../common/PlaceholderImage';

export const CartWidget: React.FC = () => {
  const { cart, loading, error, clearCart } = useCart();
  const [isOpen, setIsOpen] = useState(false);

  const toggleCart = () => {
    setIsOpen(!isOpen);
  };

  const handleClearCart = async () => {
    if (window.confirm('¿Estás seguro de que quieres vaciar el carrito?')) {
      await clearCart();
    }
  };

  if (loading) {
    return (
      <div className="fixed bottom-4 right-4 z-50">
        <button className="bg-blue-600 text-white p-3 rounded-full shadow-lg hover:bg-blue-700 transition-colors">
          <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-white"></div>
        </button>
      </div>
    );
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      {/* Botón del carrito */}
      <button
        onClick={toggleCart}
        className="bg-blue-600 text-white p-3 rounded-full shadow-lg hover:bg-blue-700 transition-colors relative"
      >
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
        </svg>
        
        {/* Badge con cantidad */}
        {cart && cart.totalItems > 0 && (
          <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-6 w-6 flex items-center justify-center">
            {cart.totalItems}
          </span>
        )}
      </button>

      {/* Panel del carrito */}
      {isOpen && (
        <div className="absolute bottom-16 right-0 w-80 bg-white rounded-lg shadow-xl border max-h-96 overflow-hidden">
          {/* Header */}
          <div className="p-4 border-b bg-gray-50">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-semibold text-gray-800">
                Carrito ({cart?.totalItems || 0})
              </h3>
              <button
                onClick={toggleCart}
                className="text-gray-500 hover:text-gray-700"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>

          {/* Contenido */}
          <div className="max-h-64 overflow-y-auto">
            {error && (
              <div className="p-4 text-red-600 text-sm">
                {error}
              </div>
            )}

            {!cart || cart.cartItems.length === 0 ? (
              <div className="p-8 text-center text-gray-500">
                <PlaceholderImage width={64} height={64} className="w-16 h-16 mx-auto mb-4" />
                <p>Tu carrito está vacío</p>
                <p className="text-sm">¡Agrega algunos productos!</p>
              </div>
            ) : (
              <div className="p-4 space-y-3">
                {cart.cartItems.map((item) => (
                  <CartItem key={item.id} item={item} />
                ))}
              </div>
            )}
          </div>

          {/* Footer */}
          {cart && cart.cartItems.length > 0 && (
            <div className="p-4 border-t bg-gray-50">
              <div className="flex justify-between items-center mb-3">
                <span className="text-lg font-semibold">Total:</span>
                <span className="text-xl font-bold text-blue-600">
                  ${cart.total.toFixed(2)}
                </span>
              </div>
              
              <div className="flex space-x-2">
                <button
                  onClick={handleClearCart}
                  className="flex-1 px-3 py-2 text-sm text-red-600 border border-red-300 rounded hover:bg-red-50 transition-colors"
                >
                  Vaciar
                </button>
                <button className="flex-1 px-3 py-2 text-sm bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors">
                  Comprar
                </button>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
