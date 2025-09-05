import React from 'react';
import { CartItemDTO } from '../../types/cart';
import { useCart } from '../../contexts/CartContext';
import { useConfirm } from '../../hooks/useConfirm';
import PlaceholderImage from '../common/PlaceholderImage';
import ConfirmModal from '../common/ConfirmModal';

interface CartItemProps {
  item: CartItemDTO;
}

export const CartItem: React.FC<CartItemProps> = ({ item }) => {
  const { updateQuantity, removeFromCart } = useCart();
  const { isOpen, options, showConfirm, handleConfirm, handleCancel } = useConfirm();

  const handleQuantityChange = async (newQuantity: number) => {
    if (newQuantity < 1) return;
    await updateQuantity(item.id, newQuantity);
  };

  const handleRemove = () => {
    showConfirm(
      {
        title: 'Eliminar Producto',
        message: `¿Estás seguro de que quieres eliminar "${item.productName}" del carrito?`,
        confirmText: 'Eliminar',
        cancelText: 'Cancelar',
        type: 'danger',
      },
      () => removeFromCart(item.id)
    );
  };

  return (
    <div className="flex items-center space-x-4 p-4 bg-white border border-gray-100 rounded-lg hover:shadow-md transition-all duration-200">
      {/* Imagen del producto */}
      <div className="flex-shrink-0">
        {item.productImage && item.productImage.trim() !== '' ? (
          <div className="relative w-16 h-16 rounded-lg overflow-hidden bg-gray-100">
            <img
              src={item.productImage.startsWith('http') ? item.productImage : `http://localhost:5000${item.productImage}`}
              alt={item.productName}
              className="w-full h-full object-cover transition-transform duration-200 hover:scale-105"
              onLoad={() => {
                console.log('Imagen cargada correctamente:', item.productImage);
              }}
              onError={(e) => {
                console.log('Error cargando imagen:', item.productImage);
                const target = e.target as HTMLImageElement;
                target.style.display = 'none';
                const placeholder = target.nextElementSibling as HTMLElement;
                if (placeholder) placeholder.style.display = 'flex';
              }}
            />
            <div className="absolute inset-0 flex items-center justify-center bg-gray-100" style={{ display: 'none' }}>
              <PlaceholderImage width={64} height={64} className="w-16 h-16 rounded-lg" />
            </div>
          </div>
        ) : (
          <PlaceholderImage width={64} height={64} className="w-16 h-16 rounded-lg" />
        )}
      </div>

      {/* Información del producto */}
      <div className="flex-1 min-w-0">
        <h4 className="text-base font-semibold text-gray-900 truncate mb-1">
          {item.productName}
        </h4>
        <p className="text-sm text-gray-500">
          ${item.unitPrice.toFixed(2)} c/u
        </p>
      </div>

      {/* Controles de cantidad */}
      <div className="flex items-center space-x-1">
        <button
          onClick={() => handleQuantityChange(item.quantity - 1)}
          className="w-7 h-7 flex items-center justify-center bg-green-100 text-green-600 rounded-full hover:bg-green-200 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          disabled={item.quantity <= 1}
        >
          <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 12H4" />
          </svg>
        </button>
        
        <span className="w-8 text-center text-sm font-bold text-gray-700 bg-gray-50 rounded px-2 py-1">
          {item.quantity}
        </span>
        
        <button
          onClick={() => handleQuantityChange(item.quantity + 1)}
          className="w-7 h-7 flex items-center justify-center bg-green-100 text-green-600 rounded-full hover:bg-green-200 transition-colors"
        >
          <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
        </button>
      </div>

      {/* Subtotal y botón eliminar */}
      <div className="flex flex-col items-end space-y-2">
        <span className="text-lg font-bold text-green-600">
          ${item.subtotal.toFixed(2)}
        </span>
        <button
          onClick={handleRemove}
          className="text-red-500 hover:text-red-700 hover:bg-red-50 rounded-full p-1 transition-all duration-200"
          title="Eliminar del carrito"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
      
      {/* Modal de confirmación */}
      <ConfirmModal
        isOpen={isOpen}
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
