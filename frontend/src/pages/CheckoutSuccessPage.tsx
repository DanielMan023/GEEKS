import React from 'react';
import { useNavigate } from 'react-router-dom';
import { CheckCircle, ArrowLeft, ShoppingBag } from 'lucide-react';
import Sidebar from '../components/Sidebar';
import { useSidebar } from '../contexts/SidebarContext';

const CheckoutSuccessPage: React.FC = () => {
  const { isCollapsed } = useSidebar();
  const navigate = useNavigate();

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className={`flex-1 overflow-y-auto transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <div className="p-6">
          <div className="max-w-2xl mx-auto text-center">
            {/* Icono de éxito */}
            <div className="mb-6">
              <div className="mx-auto w-20 h-20 bg-green-100 rounded-full flex items-center justify-center">
                <CheckCircle className="w-12 h-12 text-green-600" />
              </div>
            </div>

            {/* Mensaje principal */}
            <h1 className="text-3xl font-bold text-gray-900 mb-4">
              ¡Compra Realizada con Éxito!
            </h1>
            
            <p className="text-lg text-gray-600 mb-8">
              Tu pedido ha sido procesado correctamente. Recibirás un email de confirmación pronto.
            </p>

            {/* Información adicional */}
            <div className="bg-white rounded-lg shadow-sm p-6 mb-8 text-left">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">¿Qué sigue?</h2>
              <div className="space-y-3 text-gray-600">
                <div className="flex items-start gap-3">
                  <div className="w-2 h-2 bg-green-500 rounded-full mt-2 flex-shrink-0"></div>
                  <p>Recibirás un email de confirmación con los detalles de tu pedido</p>
                </div>
                <div className="flex items-start gap-3">
                  <div className="w-2 h-2 bg-green-500 rounded-full mt-2 flex-shrink-0"></div>
                  <p>Procesaremos tu pedido y te notificaremos cuando esté listo para envío</p>
                </div>
                <div className="flex items-start gap-3">
                  <div className="w-2 h-2 bg-green-500 rounded-full mt-2 flex-shrink-0"></div>
                  <p>El tiempo de entrega estimado es de 3-5 días hábiles</p>
                </div>
              </div>
            </div>

            {/* Botones de acción */}
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <button
                onClick={() => navigate('/catalog')}
                className="bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center gap-2"
              >
                <ShoppingBag size={20} />
                Seguir Comprando
              </button>
              
              <button
                onClick={() => navigate('/')}
                className="bg-gray-600 text-white px-6 py-3 rounded-lg hover:bg-gray-700 transition-colors flex items-center justify-center gap-2"
              >
                <ArrowLeft size={20} />
                Volver al Inicio
              </button>
            </div>

            {/* Información de contacto */}
            <div className="mt-8 p-4 bg-blue-50 rounded-lg">
              <p className="text-sm text-blue-800">
                ¿Tienes alguna pregunta? Contáctanos en{' '}
                <a href="mailto:support@geeks.com" className="font-medium underline">
                  support@geeks.com
                </a>
                {' '}o al{' '}
                <a href="tel:+573001234567" className="font-medium underline">
                  +57 300 123 4567
                </a>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutSuccessPage;
