import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useRole } from '../hooks/useRole';

const RoleBasedRedirect: React.FC = () => {
  const { isAuthenticated, loading } = useAuth();
  const { isAdmin } = useRole();

  console.log('RoleBasedRedirect: isAuthenticated:', isAuthenticated, 'loading:', loading, 'isAdmin:', isAdmin);

  // Mostrar loading mientras se verifica la autenticación
  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-500 mx-auto mb-4"></div>
          <p className="text-gray-600">Verificando autenticación...</p>
        </div>
      </div>
    );
  }

  // Si no está autenticado, ir al login
  if (!isAuthenticated) {
    console.log('RoleBasedRedirect: No autenticado, redirigiendo a login');
    return <Navigate to="/login" replace />;
  }

  // Si es admin, ir a administración de productos
  if (isAdmin) {
    console.log('RoleBasedRedirect: Admin, redirigiendo a admin/products');
    return <Navigate to="/admin/products" replace />;
  }

  // Si es usuario normal, ir al catálogo
  console.log('RoleBasedRedirect: Usuario normal, redirigiendo a catalog');
  return <Navigate to="/catalog" replace />;
};

export default RoleBasedRedirect;
