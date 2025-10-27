import React from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { SidebarProvider } from './contexts/SidebarContext';
import { CartProvider } from './contexts/CartContext';
import Login from './components/Login';
import Register from './components/Register';
import ProductCatalogPage from './pages/ProductCatalogPage';
import ProductManagementPage from './pages/ProductManagementPage';
import OrdersManagementPage from './pages/OrdersManagementPage';
import CheckoutPage from './pages/CheckoutPage';
import CheckoutSuccessPage from './pages/CheckoutSuccessPage';
import PublicRoute from './components/PublicRoute';
import RouteGuard from './components/guards/RouteGuard';
import RoleBasedRedirect from './components/RoleBasedRedirect';
import { CartWidget } from './components/cart';
import ChatbotWidget from './components/chatbot/ChatbotWidget';
import ResetPasswordPage from './pages/ResetPasswordPage';

const AppContent: React.FC = () => {
  const location = useLocation();
  
  // Rutas donde NO debe aparecer el carrito ni el chatbot
  const hideWidgetRoutes = ['/login', '/register', '/reset-password'];
  const shouldShowWidgets = !hideWidgetRoutes.includes(location.pathname);

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
        
        <Route path="/reset-password" element={<ResetPasswordPage />} />
        
        {/* Rutas protegidas - solo accesibles si estás autenticado */}
        
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
        
        <Route path="/admin/orders" element={
          <RouteGuard>
            <OrdersManagementPage />
          </RouteGuard>
        } />
        
        <Route path="/checkout" element={
          <RouteGuard>
            <CheckoutPage />
          </RouteGuard>
        } />
        
        <Route path="/checkout/success" element={
          <RouteGuard>
            <CheckoutSuccessPage />
          </RouteGuard>
        } />
        
        {/* Ruta por defecto - redirigir según rol */}
        <Route path="*" element={<RoleBasedRedirect />} />
      </Routes>
      
      {/* Widgets - solo visibles en rutas protegidas */}
      {shouldShowWidgets && (
        <>
          <CartWidget />
          <ChatbotWidget />
        </>
      )}
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

