import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { SidebarProvider } from './contexts/SidebarContext';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import ProductCatalogPage from './pages/ProductCatalogPage';
import PublicRoute from './components/PublicRoute';
import RouteGuard from './components/guards/RouteGuard';

const AppContent: React.FC = () => {
  return (
    <Routes>
      {/* Ruta raíz - redirigir según autenticación */}
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      
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
      
      {/* Ruta por defecto - redirigir a dashboard */}
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <SidebarProvider>
        <AppContent />
      </SidebarProvider>
    </AuthProvider>
  );
};

export default App;

