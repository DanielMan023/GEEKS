import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useRole } from '../../hooks/useRole';

interface RouteGuardProps {
  children: React.ReactNode;
  requiredRole?: 'Admin' | 'User' | 'Any';
  requiredPermission?: string;
  fallbackPath?: string;
}

const RouteGuard: React.FC<RouteGuardProps> = ({
  children,
  requiredRole = 'Any',
  requiredPermission,
  fallbackPath = '/dashboard'
}) => {
  const { isAuthenticated, loading } = useAuth();
  const { userRole, hasPermission, canAccessRoute } = useRole();
  const location = useLocation();

  // Mostrar loading mientras se verifica la autenticación
  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-500 mx-auto mb-4"></div>
          <p className="text-gray-600">Verificando permisos...</p>
        </div>
      </div>
    );
  }

  // Si no está autenticado, redirigir al login
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Verificar rol requerido
  if (requiredRole !== 'Any') {
    if (requiredRole === 'Admin' && userRole !== 'Admin') {
      return <Navigate to={fallbackPath} replace />;
    }
    if (requiredRole === 'User' && userRole !== 'User' && userRole !== 'Admin') {
      return <Navigate to={fallbackPath} replace />;
    }
  }

  // Verificar permiso específico
  if (requiredPermission && !hasPermission(requiredPermission as any)) {
    return <Navigate to={fallbackPath} replace />;
  }

  // Verificar acceso a la ruta actual
  if (!canAccessRoute(location.pathname)) {
    return <Navigate to={fallbackPath} replace />;
  }

  // Si pasa todas las verificaciones, mostrar el contenido
  return <>{children}</>;
};

export default RouteGuard;
