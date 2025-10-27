import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../contexts/CartContext';
import { CreditCard, MapPin, User, Phone, Mail } from 'lucide-react';
import Sidebar from '../components/Sidebar';
import { useSidebar } from '../contexts/SidebarContext';

const CheckoutPage: React.FC = () => {
  const { cart, processCheckout } = useCart();
  const { isCollapsed } = useSidebar();
  const navigate = useNavigate();
  const [isProcessing, setIsProcessing] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    zipCode: '',
    paymentMethod: 'credit'
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsProcessing(true);

    try {
      // Procesar el checkout (esto descontará el stock automáticamente)
      const success = await processCheckout(formData);
      
      if (success) {
        // Redirigir a página de éxito
        navigate('/checkout/success');
      } else {
        // Mostrar error si el checkout falló
        console.error('Error en checkout: success = false');
        alert('Error al procesar la compra. Por favor, verifica el stock disponible.');
      }
    } catch (error) {
      console.error('Error en checkout:', error);
      const errorMessage = error instanceof Error ? error.message : 'Error desconocido';
      alert(`Error al procesar la compra: ${errorMessage}`);
    } finally {
      setIsProcessing(false);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(price);
  };

  if (!cart || cart.cartItems.length === 0) {
    return (
      <div className="flex h-screen bg-gray-50">
        <Sidebar />
        <div className={`flex-1 overflow-y-auto transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
          <div className="p-6">
            <div className="max-w-2xl mx-auto text-center">
              <h1 className="text-2xl font-bold text-gray-900 mb-4">Carrito Vacío</h1>
              <p className="text-gray-600 mb-6">No tienes productos en tu carrito para proceder al pago.</p>
              <button
                onClick={() => navigate('/catalog')}
                className="bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors"
              >
                Ver Productos
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className={`flex-1 overflow-y-auto transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <div className="p-6">
          <div className="max-w-4xl mx-auto">
            <h1 className="text-2xl font-bold text-gray-900 mb-6">Finalizar Compra</h1>
            
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
              {/* Formulario de Checkout */}
              <div className="bg-white rounded-lg shadow-sm p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-6">Información de Pago</h2>
                
                <form onSubmit={handleSubmit} className="space-y-6">
                  {/* Información Personal */}
                  <div>
                    <h3 className="text-md font-medium text-gray-700 mb-4 flex items-center gap-2">
                      <User size={18} />
                      Información Personal
                    </h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Nombre *
                        </label>
                        <input
                          type="text"
                          name="firstName"
                          required
                          value={formData.firstName}
                          onChange={handleInputChange}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Apellido *
                        </label>
                        <input
                          type="text"
                          name="lastName"
                          required
                          value={formData.lastName}
                          onChange={handleInputChange}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        />
                      </div>
                    </div>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Email *
                        </label>
                        <input
                          type="email"
                          name="email"
                          required
                          value={formData.email}
                          onChange={handleInputChange}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Teléfono *
                        </label>
                        <input
                          type="text"
                          name="phone"
                          required
                          value={formData.phone}
                          onChange={handleInputChange}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        />
                      </div>
                    </div>
                  </div>

                  {/* Dirección */}
                  <div>
                    <h3 className="text-md font-medium text-gray-700 mb-4 flex items-center gap-2">
                      <MapPin size={18} />
                      Dirección de Envío
                    </h3>
                    <div className="space-y-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Dirección *
                        </label>
                        <input
                          type="text"
                          name="address"
                          required
                          value={formData.address}
                          onChange={handleInputChange}
                          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                        />
                      </div>
                      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div>
                          <label className="block text-sm font-medium text-gray-700 mb-1">
                            Ciudad *
                          </label>
                          <input
                            type="text"
                            name="city"
                            required
                            value={formData.city}
                            onChange={handleInputChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                          />
                        </div>
                        <div>
                          <label className="block text-sm font-medium text-gray-700 mb-1">
                            Código Postal *
                          </label>
                          <input
                            type="text"
                            name="zipCode"
                            required
                            value={formData.zipCode}
                            onChange={handleInputChange}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                          />
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Método de Pago */}
                  <div>
                    <h3 className="text-md font-medium text-gray-700 mb-4 flex items-center gap-2">
                      <CreditCard size={18} />
                      Método de Pago
                    </h3>
                    <div className="space-y-3">
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input
                          type="radio"
                          name="paymentMethod"
                          value="credit"
                          checked={formData.paymentMethod === 'credit'}
                          onChange={handleInputChange}
                          className="mr-3"
                        />
                        <CreditCard size={20} className="mr-2 text-gray-600" />
                        <span>Tarjeta de Crédito</span>
                      </label>
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input
                          type="radio"
                          name="paymentMethod"
                          value="debit"
                          checked={formData.paymentMethod === 'debit'}
                          onChange={handleInputChange}
                          className="mr-3"
                        />
                        <CreditCard size={20} className="mr-2 text-gray-600" />
                        <span>Tarjeta Débito</span>
                      </label>
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input
                          type="radio"
                          name="paymentMethod"
                          value="transfer"
                          checked={formData.paymentMethod === 'transfer'}
                          onChange={handleInputChange}
                          className="mr-3"
                        />
                        <CreditCard size={20} className="mr-2 text-gray-600" />
                        <span>Transferencia Bancaria</span>
                      </label>
                    </div>
                  </div>

                  <div className="flex gap-4">
                    <button
                      type="button"
                      onClick={() => navigate(-1)}
                      className="w-full bg-gray-200 text-gray-700 py-3 rounded-lg hover:bg-gray-300 transition-colors font-medium"
                      disabled={isProcessing}
                    >
                      Volver atrás
                    </button>
                    <button
                      type="submit"
                      disabled={isProcessing}
                      className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
                    >
                      {isProcessing ? 'Procesando...' : `Pagar ${formatPrice(cart.total)}`}
                    </button>
                  </div>
                </form>
              </div>

              {/* Resumen del Pedido */}
              <div className="bg-white rounded-lg shadow-sm p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-6">Resumen del Pedido</h2>
                
                <div className="space-y-4">
                  {cart.cartItems.map((item) => (
                    <div key={item.id} className="flex items-center gap-4 p-3 border border-gray-200 rounded-lg">
                      <div className="w-16 h-16 bg-gray-100 rounded-lg flex items-center justify-center">
                        <span className="text-xs text-gray-500">IMG</span>
                      </div>
                      <div className="flex-1">
                        <h3 className="font-medium text-gray-900">{item.productName}</h3>
                        <p className="text-sm text-gray-500">Cantidad: {item.quantity}</p>
                        <p className="text-sm font-medium text-gray-900">
                          {formatPrice(item.price * item.quantity)}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="mt-6 pt-6 border-t border-gray-200">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-gray-600">Subtotal:</span>
                    <span className="font-medium">{formatPrice(cart.total)}</span>
                  </div>
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-gray-600">Envío:</span>
                    <span className="font-medium text-green-600">Gratis</span>
                  </div>
                  <div className="flex justify-between items-center text-lg font-bold pt-2 border-t border-gray-200">
                    <span>Total:</span>
                    <span>{formatPrice(cart.total)}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutPage;
