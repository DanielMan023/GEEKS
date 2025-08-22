import React from 'react';
import { useAuth } from '../contexts/AuthContext';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();

  if (!user) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center">
              <h1 className="text-2xl font-bold text-gray-900">
                GEEKS Dashboard
              </h1>
            </div>
            <div className="flex items-center space-x-4">
              <div className="text-right">
                <p className="text-sm font-medium text-gray-900">
                  {user.firstName} {user.lastName}
                </p>
                <p className="text-xs text-gray-500">{user.email}</p>
              </div>
              <button
                onClick={logout}
                className="btn-secondary"
              >
                Cerrar sesión
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">
            Bienvenido, {user.firstName}!
          </h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {/* User Info Card */}
            <div className="bg-blue-50 rounded-lg p-6">
              <h3 className="text-lg font-medium text-blue-900 mb-3">
                Información del Usuario
              </h3>
              <div className="space-y-2">
                <p className="text-sm text-blue-700">
                  <span className="font-medium">Nombre:</span> {user.firstName} {user.lastName}
                </p>
                <p className="text-sm text-blue-700">
                  <span className="font-medium">Email:</span> {user.email}
                </p>
                <p className="text-sm text-blue-700">
                  <span className="font-medium">Rol:</span> {user.role}
                </p>
                <p className="text-sm text-blue-700">
                  <span className="font-medium">ID:</span> {user.id}
                </p>
              </div>
            </div>

            {/* Quick Actions Card */}
            <div className="bg-green-50 rounded-lg p-6">
              <h3 className="text-lg font-medium text-green-900 mb-3">
                Acciones Rápidas
              </h3>
              <div className="space-y-3">
                <button className="w-full btn-primary text-sm">
                  Editar Perfil
                </button>
                <button className="w-full btn-secondary text-sm">
                  Cambiar Contraseña
                </button>
              </div>
            </div>

            {/* Stats Card */}
            <div className="bg-purple-50 rounded-lg p-6">
              <h3 className="text-lg font-medium text-purple-900 mb-3">
                Estadísticas
              </h3>
              <div className="space-y-2">
                <p className="text-sm text-purple-700">
                  <span className="font-medium">Sesiones:</span> 1
                </p>
                <p className="text-sm text-purple-700">
                  <span className="font-medium">Último acceso:</span> Hoy
                </p>
              </div>
            </div>
          </div>

          {/* Welcome Message */}
          <div className="mt-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg p-6 text-white">
            <h3 className="text-xl font-semibold mb-2">
              ¡Gracias por unirte a GEEKS!
            </h3>
            <p className="text-blue-100">
              Tu cuenta ha sido creada exitosamente. Ahora puedes acceder a todas las funcionalidades de nuestra plataforma.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;
