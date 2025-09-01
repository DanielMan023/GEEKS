import { LayoutDashboard, Building, Folder, ClipboardList, CalendarCheck, Users, Headphones, Sun, Settings, LayoutPanelLeft, ShoppingBag } from 'lucide-react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { twMerge } from '../utils';
import { useSidebar } from '../contexts/SidebarContext';
import { useAuth } from '../contexts/AuthContext';
import { useRole } from '../hooks/useRole';

const Sidebar = () => {
  const { isCollapsed, toggleSidebar } = useSidebar();
  const { user, logout } = useAuth();
  const { permissions, userRole, isAdmin } = useRole();
  const location = useLocation();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Error al cerrar sesión:', error);
    }
  };

  // Menú base que todos pueden ver
  const baseMenuItems = [
    { 
      icon: LayoutDashboard, 
      label: 'Dashboard', 
      to: '/dashboard', 
      permission: 'canViewDashboard',
      show: isAdmin // Solo admin ve el dashboard
    },
    { 
      icon: Building, 
      label: 'Inventarios', 
      to: '/dashboard/inventarios/lista', 
      permission: 'canViewInventories',
      show: permissions.canViewInventories && isAdmin
    },
    { 
      icon: Folder, 
      label: 'Productos', 
      to: '/admin/products', 
      permission: 'canViewProducts',
      show: permissions.canViewProducts && isAdmin
    },
    { 
      icon: ClipboardList, 
      label: 'Pedidos', 
      to: '/dashboard/pedidos/lista', 
      permission: 'canViewOrders',
      show: permissions.canViewOrders && isAdmin
    },
    { 
      icon: CalendarCheck, 
      label: 'Compras', 
      to: '/dashboard/compras/lista', 
      permission: 'canViewPurchases',
      show: permissions.canViewPurchases && isAdmin
    },
    { 
      icon: ShoppingBag, 
      label: 'Catálogo', 
      to: '/catalog', 
      permission: 'canViewCatalog',
      show: true // Siempre visible para usuarios autenticados
    },
  ];

  // Menú solo para administradores
  const adminMenuItems = [
    { 
      icon: Users, 
      label: 'Usuarios', 
      to: '/dashboard/usuarios/lista', 
      permission: 'canViewUsers',
      show: permissions.canViewUsers
    },
    { 
      icon: Headphones, 
      label: 'Usuarios Administrativos', 
      to: '/dashboard/usuarios-administrativos/lista', 
      permission: 'canViewAdminUsers',
      show: permissions.canViewAdminUsers
    },
  ];

  // Combinar menús y filtrar por permisos
  const allMenuItems = [...baseMenuItems, ...adminMenuItems].filter(item => item.show);

  return (
    <div className={twMerge(
      "fixed left-0 top-0 z-40 h-screen transition-transform duration-300 ease-in-out",
      isCollapsed ? "w-16" : "w-64"
    )}>
      <div className="flex h-full flex-col bg-[#1E1E1E] border-r border-gray-700">
        {/* Header */}
        <div className="flex h-16 items-center justify-between px-4 border-b border-gray-700">
          {!isCollapsed && (
            <div className="flex items-center space-x-2">
              <img src="/geeks.png" alt="GEEKS" className="w-8 h-8 rounded" />
              <span className="text-white font-semibold text-lg">GEEKS</span>
            </div>
          )}
          <button
            onClick={toggleSidebar}
            className="p-2 text-gray-400 hover:text-white hover:bg-gray-700 rounded-lg transition-colors"
          >
            <LayoutPanelLeft className="w-5 h-5" />
          </button>
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 space-y-2">
          {allMenuItems.map((item) => {
            const Icon = item.icon;
            const isActive = location.pathname === item.to;
            
            return (
              <Link
                key={item.to}
                to={item.to}
                className={twMerge(
                  "flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors",
                  isActive
                    ? "bg-green-600 text-white"
                    : "text-gray-300 hover:bg-gray-700 hover:text-white"
                )}
              >
                <Icon className="w-5 h-5 flex-shrink-0" />
                {!isCollapsed && (
                  <span className="ml-3">{item.label}</span>
                )}
              </Link>
            );
          })}
        </nav>

        {/* User Section */}
        <div className="border-t border-gray-700 p-4">
          {!isCollapsed && (
            <div className="mb-4">
              <div className="text-sm text-gray-400">Usuario</div>
              <div className="text-white font-medium truncate">
                {user?.firstName} {user?.lastName}
              </div>
              <div className="text-xs text-gray-500 truncate">{user?.email}</div>
              <div className="text-xs text-green-400 font-medium mt-1">
                Rol: {userRole}
              </div>
            </div>
          )}
          
          <div className="flex items-center space-x-2">
            <button
              onClick={handleLogout}
              className={twMerge(
                "flex items-center px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-700 hover:text-white rounded-lg transition-colors",
                isCollapsed ? "justify-center w-full" : ""
              )}
            >
              <Settings className="w-5 h-5" />
              {!isCollapsed && <span className="ml-3">Cerrar Sesión</span>}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Sidebar;
