import React, { useState } from 'react';
import { useCart } from '../../contexts/CartContext';
import { useConfirm } from '../../hooks/useConfirm';
import { CartItem } from './CartItem';
import PlaceholderImage from '../common/PlaceholderImage';
import ConfirmModal from '../common/ConfirmModal';

export const CartWidget: React.FC = () => {
  const { cart, loading, error, clearCart } = useCart();
  const { isOpen: confirmOpen, options, showConfirm, handleConfirm, handleCancel } = useConfirm();
  const [isOpen, setIsOpen] = useState(false);

  const toggleCart = () => {
    setIsOpen(!isOpen);
  };

  const handleClearCart = () => {
    showConfirm(
      {
        title: 'Vaciar Carrito',
        message: '¿Estás seguro de que quieres vaciar todo el carrito? Esta acción no se puede deshacer.',
        confirmText: 'Vaciar Todo',
        cancelText: 'Cancelar',
        type: 'danger',
      },
      () => clearCart()
    );
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
    <div className="fixed top-4 right-4 z-50">
      {/* Botón del carrito */}
      <button
        onClick={toggleCart}
        className="bg-green-600 text-white p-3 rounded-full shadow-lg hover:bg-green-700 transition-all duration-300 hover:scale-105 relative"
      >
        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
        </svg>
        
        {/* Badge con cantidad */}
        {cart && cart.totalItems > 0 && (
          <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-6 w-6 flex items-center justify-center font-bold animate-pulse">
            {cart.totalItems}
          </span>
        )}
      </button>

      {/* Panel del carrito */}
      {isOpen && (
        <div className="absolute top-16 right-0 w-96 bg-white rounded-lg shadow-2xl border border-gray-200 max-h-[500px] overflow-hidden animate-in slide-in-from-top-2 duration-200">
          {/* Header */}
          <div className="p-4 border-b bg-gradient-to-r from-green-50 to-emerald-50">
            <div className="flex justify-between items-center">
              <div className="flex items-center space-x-2">
                <div className="w-8 h-8 bg-green-600 rounded-full flex items-center justify-center">
                  <svg className="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
                  </svg>
                </div>
                <h3 className="text-lg font-bold text-gray-800">
                  Carrito ({cart?.totalItems || 0})
                </h3>
              </div>
              <button
                onClick={toggleCart}
                className="text-gray-500 hover:text-gray-700 hover:bg-gray-200 rounded-full p-1 transition-colors"
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
                <div className="w-20 h-20 mx-auto mb-4 bg-gray-100 rounded-full flex items-center justify-center">
                  <svg className="w-10 h-10 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
                  </svg>
                </div>
                <h4 className="text-lg font-semibold text-gray-700 mb-2">Tu carrito está vacío</h4>
                <p className="text-sm text-gray-500 mb-4">¡Agrega algunos productos para comenzar!</p>
                <button
                  onClick={() => {
                    toggleCart();
                    // Aquí podrías navegar al catálogo
                  }}
                  className="px-4 py-2 bg-green-600 text-white text-sm rounded-lg hover:bg-green-700 transition-colors"
                >
                  Ver Productos
                </button>
              </div>
            ) : (
              <div className="p-4 space-y-4">
                {cart.cartItems.map((item) => (
                  <CartItem key={item.id} item={item} />
                ))}
              </div>
            )}
          </div>

          {/* Footer */}
          {cart && cart.cartItems.length > 0 && (
            <div className="p-4 border-t bg-gradient-to-r from-gray-50 to-green-50">
              <div className="flex justify-between items-center mb-4">
                <div className="flex items-center space-x-2">
                  <span className="text-lg font-semibold text-gray-700">Total:</span>
                  <span className="text-xs text-gray-500">({cart.totalItems} items)</span>
                </div>
                <span className="text-2xl font-bold text-green-600">
                  ${cart.total.toFixed(2)}
                </span>
              </div>
              
              <div className="flex space-x-2">
                <button
                  onClick={handleClearCart}
                  className="flex-1 px-4 py-2 text-sm text-red-600 border border-red-300 rounded-lg hover:bg-red-50 hover:border-red-400 transition-all duration-200 font-medium"
                >
                  <svg className="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                  Vaciar
                </button>
                <button className="flex-1 px-4 py-2 text-sm bg-green-600 text-white rounded-lg hover:bg-green-700 transition-all duration-200 font-medium shadow-lg hover:shadow-xl">
                  <svg className="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                  </svg>
                  Comprar
                </button>
              </div>
            </div>
          )}
        </div>
      )}
      
      {/* Modal de confirmación */}
      <ConfirmModal
        isOpen={confirmOpen}
        title={options?.title || ''}
        message={options?.message || ''}
        confirmText={options?.confirmText}
        cancelText={options?.cancelText}
        type={options?.type}
        onConfirm={handleConfirm}
        onCancel={handleCancel}
      />
    </div>
  );
};
