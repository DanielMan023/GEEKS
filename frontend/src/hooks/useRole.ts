import { useAuth } from '../contexts/AuthContext';

export interface UserPermissions {
  canViewDashboard: boolean;
  canViewInventories: boolean;
  canViewProducts: boolean;
  canViewOrders: boolean;
  canViewPurchases: boolean;
  canViewUsers: boolean;
  canViewAdminUsers: boolean;
  canManageUsers: boolean;
  canManageSystem: boolean;
}

export const useRole = () => {
  const { user } = useAuth();

  const isAdmin = user?.role?.name === 'Admin';
  const isUser = user?.role?.name === 'User';

  const permissions: UserPermissions = {
    canViewDashboard: false,
    canViewInventories: isAdmin || isUser, // Ambos roles pueden ver inventarios
    canViewProducts: isAdmin || isUser, // Ambos roles pueden ver productos
    canViewOrders: isAdmin || isUser, // Ambos roles pueden ver pedidos
    canViewPurchases: isAdmin || isUser, // Ambos roles pueden ver compras
    canViewUsers: isAdmin, // Solo admin puede ver usuarios
    canViewAdminUsers: isAdmin, // Solo admin puede ver usuarios administrativos
    canManageUsers: isAdmin, // Solo admin puede gestionar usuarios
    canManageSystem: isAdmin, // Solo admin puede gestionar el sistema
  };

  const hasPermission = (permission: keyof UserPermissions): boolean => {
    return permissions[permission];
  };

  const canAccessRoute = (route: string): boolean => {
    switch (route) {
      case '/dashboard':
        return false;
      case '/dashboard/inventarios':
      case '/dashboard/inventarios/lista':
        return permissions.canViewInventories;
      case '/dashboard/productos':
      case '/dashboard/productos/lista':
        return permissions.canViewProducts;
      case '/dashboard/pedidos':
      case '/dashboard/pedidos/lista':
        return permissions.canViewOrders;
      case '/dashboard/compras':
      case '/dashboard/compras/lista':
        return permissions.canViewPurchases;
      case '/dashboard/usuarios':
      case '/dashboard/usuarios/lista':
        return permissions.canViewUsers;
      case '/dashboard/usuarios-administrativos':
      case '/dashboard/usuarios-administrativos/lista':
        return permissions.canViewAdminUsers;
      default:
        return true; // Rutas no especificadas son accesibles
    }
  };

  return {
    isAdmin,
    isUser,
    permissions,
    hasPermission,
    canAccessRoute,
    userRole: user?.role?.name || 'Guest'
  };
};
