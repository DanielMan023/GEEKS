import React from 'react';
import { CartItemDTO } from '../../types/cart';
import { useCart } from '../../contexts/CartContext';
import PlaceholderImage from '../common/PlaceholderImage';

interface CartItemProps {
  item: CartItemDTO;
}

export const CartItem: React.FC<CartItemProps> = ({ item }) => {
  const { updateQuantity, removeFromCart } = useCart();

  const handleQuantityChange = async (newQuantity: number) => {
    if (newQuantity < 1) return;
    await updateQuantity(item.id, newQuantity);
  };

  const handleRemove = async () => {
    if (window.confirm('¿Eliminar este producto del carrito?')) {
      await removeFromCart(item.id);
    }
  };

  return (
    <div className="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
      {/* Imagen del producto */}
      <div className="flex-shrink-0">
        {item.productImage ? (
          <img
            src={item.productImage}
            alt={item.productName}
            className="w-12 h-12 object-cover rounded"
          />
        ) : (
          <PlaceholderImage width={48} height={48} className="w-12 h-12 rounded" />
        )}
      </div>

      {/* Información del producto */}
      <div className="flex-1 min-w-0">
        <h4 className="text-sm font-medium text-gray-900 truncate">
          {item.productName}
        </h4>
        <p className="text-sm text-gray-500">
          ${item.unitPrice.toFixed(2)} c/u
        </p>
      </div>

      {/* Controles de cantidad */}
      <div className="flex items-center space-x-2">
        <button
          onClick={() => handleQuantityChange(item.quantity - 1)}
          className="w-6 h-6 flex items-center justify-center bg-gray-200 text-gray-600 rounded hover:bg-gray-300 transition-colors"
          disabled={item.quantity <= 1}
        >
          <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 12H4" />
          </svg>
        </button>
        
        <span className="w-8 text-center text-sm font-medium">
          {item.quantity}
        </span>
        
        <button
          onClick={() => handleQuantityChange(item.quantity + 1)}
          className="w-6 h-6 flex items-center justify-center bg-gray-200 text-gray-600 rounded hover:bg-gray-300 transition-colors"
        >
          <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
        </button>
      </div>

      {/* Subtotal y botón eliminar */}
      <div className="flex flex-col items-end space-y-1">
        <span className="text-sm font-semibold text-gray-900">
          ${item.subtotal.toFixed(2)}
        </span>
        <button
          onClick={handleRemove}
          className="text-red-500 hover:text-red-700 transition-colors"
          title="Eliminar del carrito"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    </div>
  );
};
