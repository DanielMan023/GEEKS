import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { SidebarProvider } from './contexts/SidebarContext';
import { CartProvider } from './contexts/CartContext';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import ProductCatalogPage from './pages/ProductCatalogPage';
import ProductManagementPage from './pages/ProductManagementPage';
import PublicRoute from './components/PublicRoute';
import RouteGuard from './components/guards/RouteGuard';
import RoleBasedRedirect from './components/RoleBasedRedirect';
import { CartWidget } from './components/cart';

const AppContent: React.FC = () => {
  return (
    <>
      <Routes>
        {/* Ruta raíz - redirigir según autenticación y rol */}
        <Route path="/" element={<RoleBasedRedirect />} />
        
        {/* Rutas públicas - solo accesibles si NO estás autenticado */}
        <Route path="/login" element={
          <PublicRoute>
            <Login />
          </PublicRoute>
        } />
        
        <Route path="/register" element={
          <PublicRoute>
            <Register />
          </PublicRoute>
        } />
        
        {/* Rutas protegidas - solo accesibles si estás autenticado */}
        <Route path="/dashboard/*" element={
          <RouteGuard>
            <Dashboard />
          </RouteGuard>
        } />
        
        <Route path="/catalog" element={
          <RouteGuard>
            <ProductCatalogPage />
          </RouteGuard>
        } />
        
        <Route path="/admin/products" element={
          <RouteGuard>
            <ProductManagementPage />
          </RouteGuard>
        } />
        
        {/* Ruta por defecto - redirigir según rol */}
        <Route path="*" element={<RoleBasedRedirect />} />
      </Routes>
      
      {/* Widget del carrito - visible en todas las rutas protegidas */}
      <CartWidget />
    </>
  );
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <SidebarProvider>
        <CartProvider>
          <AppContent />
        </CartProvider>
      </SidebarProvider>
    </AuthProvider>
  );
};

export default App;

