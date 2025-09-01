import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useRole } from '../hooks/useRole';
import Sidebar from './Sidebar';
import { useSidebar } from '../contexts/SidebarContext';
import SessionTimer from './SessionTimer';

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const { isAdmin } = useRole();
  const { isCollapsed } = useSidebar();
  const navigate = useNavigate();

  // Redirigir usuarios normales al catálogo
  useEffect(() => {
    if (!isAdmin) {
      navigate('/catalog');
    }
  }, [isAdmin, navigate]);

  // Si no es admin, no mostrar nada (se redirigirá)
  if (!isAdmin) {
    return null;
  }

  return (
    <div className="flex h-screen bg-gray-100">
      <Sidebar />
      <SessionTimer />
      
      <div className={`flex-1 transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <div className="p-8">
          <div className="max-w-7xl mx-auto">
            <div className="bg-white rounded-lg shadow-lg p-8">
              <h1 className="text-3xl font-bold text-gray-900 mb-8">
                Dashboard Administrativo
              </h1>
              
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {/* Información del Usuario */}
                <div className="bg-blue-50 p-6 rounded-lg border border-blue-200">
                  <h3 className="text-lg font-semibold text-blue-900 mb-4">
                    Información del Usuario
                  </h3>
                  <div className="space-y-2">
                    <p><span className="font-medium">Nombre:</span> {String(user?.firstName)} {String(user?.lastName)}</p>
                    <p><span className="font-medium">Email:</span> {String(user?.email)}</p>
                    <p><span className="font-medium">Rol:</span> {String(user?.role)}</p>
                    <p><span className="font-medium">ID:</span> {String(user?.id)}</p>
                  </div>
                </div>

                {/* Estadísticas Rápidas */}
                <div className="bg-green-50 p-6 rounded-lg border border-green-200">
                  <h3 className="text-lg font-semibold text-green-900 mb-4">
                    Estadísticas Rápidas
                  </h3>
                  <div className="space-y-2">
                    <p><span className="font-medium">Inventarios:</span> 0</p>
                    <p><span className="font-medium">Productos:</span> 0</p>
                    <p><span className="font-medium">Pedidos:</span> 0</p>
                    <p><span className="font-medium">Compras:</span> 0</p>
                  </div>
                </div>

                {/* Acciones Rápidas */}
                <div className="bg-purple-50 p-6 rounded-lg border border-purple-200">
                  <h3 className="text-lg font-semibold text-purple-900 mb-4">
                    Acciones Rápidas
                  </h3>
                  <div className="space-y-3">
                    <button className="w-full bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors">
                      Ver Inventarios
                    </button>
                    <button className="w-full bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors">
                      Crear Producto
                    </button>
                    <button className="w-full bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors">
                      Nuevo Pedido
                    </button>
                  </div>
                </div>
              </div>

              {/* Mensaje de Bienvenida */}
              <div className="mt-8 bg-gradient-to-r from-blue-500 to-purple-600 text-white p-6 rounded-lg">
                <h2 className="text-2xl font-bold mb-2">
                  ¡Bienvenido al Panel Administrativo!
                </h2>
                <p className="text-blue-100">
                  Como administrador, tienes acceso completo a todas las funcionalidades del sistema.
                  Utiliza el menú lateral para navegar por las diferentes secciones administrativas.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;

