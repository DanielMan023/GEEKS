import React, { useState, useEffect } from 'react';
import { Package, Search, Eye, Clock, Truck, CheckCircle, XCircle, RefreshCw } from 'lucide-react';
import Sidebar from '../components/Sidebar';
import OrderStatusProgress from '../components/OrderStatusProgress';
import { useSidebar } from '../contexts/SidebarContext';
import { useAuth } from '../contexts/AuthContext';

interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  productImage?: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

interface Order {
  id: number;
  userId: number;
  orderNumber: string;
  status: string;
  total: number;
  customerName: string;
  customerEmail: string;
  customerPhone?: string;
  shippingAddress: string;
  city: string;
  zipCode: string;
  paymentMethod: string;
  notes?: string;
  createdAtDateTime: string;
  shippedDate?: string;
  deliveredDate?: string;
  totalItems: number;
  orderItems: OrderItem[];
}

const MyOrdersPage: React.FC = () => {
  const { isCollapsed } = useSidebar();
  const { user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [showOrderModal, setShowOrderModal] = useState(false);

  const statusOptions = [
    { value: 'all', label: 'Todos los estados' },
    { value: 'Pending', label: 'Pendiente' },
    { value: 'Processing', label: 'Procesando' },
    { value: 'Shipped', label: 'Enviado' },
    { value: 'Delivered', label: 'Entregado' }
  ];

  const statusColors = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Processing: 'bg-blue-100 text-blue-800',
    Shipped: 'bg-purple-100 text-purple-800',
    Delivered: 'bg-green-100 text-green-800'
  };

  const statusIcons = {
    Pending: Clock,
    Processing: RefreshCw,
    Shipped: Truck,
    Delivered: CheckCircle
  };

  const statusLabels = {
    Pending: 'Pendiente',
    Processing: 'Procesando',
    Shipped: 'Enviado',
    Delivered: 'Entregado'
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/order/my-orders', {
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error('Error al cargar los pedidos');
      }

      const result = await response.json();
      setOrders(result.data);
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  };

  const filteredOrders = orders.filter(order => {
    const matchesSearch = order.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         order.customerName.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || order.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(price);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="flex h-screen bg-gray-50">
      <Sidebar />
      <div className={`flex-1 overflow-y-auto transition-all duration-300 ${isCollapsed ? 'ml-16' : 'ml-64'}`}>
        <div className="p-6">
          <div className="max-w-7xl mx-auto">
            {/* Header */}
            <div className="mb-8">
              <h1 className="text-3xl font-bold text-gray-900 mb-2">Mis Pedidos</h1>
              <p className="text-gray-600">Revisa el estado de tus compras y pedidos</p>
            </div>

            {/* Filtros y búsqueda */}
            <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type="text"
                    placeholder="Buscar por número de pedido..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  />
                </div>
                <div className="relative">
                  <Package className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <select
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  >
                    {statusOptions.map(option => (
                      <option key={option.value} value={option.value}>
                        {option.label}
                      </option>
                    ))}
                  </select>
                </div>
                <button
                  onClick={fetchOrders}
                  className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center gap-2"
                >
                  <RefreshCw className="w-4 h-4" />
                  Actualizar
                </button>
              </div>
            </div>

            {/* Lista de pedidos */}
            <div className="bg-white rounded-lg shadow-sm overflow-hidden">
              {loading ? (
                <div className="p-8 text-center">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-green-600 mx-auto"></div>
                  <p className="mt-2 text-gray-600">Cargando pedidos...</p>
                </div>
              ) : error ? (
                <div className="p-8 text-center">
                  <p className="text-red-600 mb-4">{error}</p>
                  <button
                    onClick={fetchOrders}
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"
                  >
                    Reintentar
                  </button>
                </div>
              ) : filteredOrders.length === 0 ? (
                <div className="p-8 text-center">
                  <Package className="w-12 h-12 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600 mb-4">No tienes pedidos aún</p>
                  <a
                    href="/catalog"
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors inline-block"
                  >
                    Ver Productos
                  </a>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-gray-50 border-b border-gray-200">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Pedido
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Estado
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Total
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Fecha
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Acciones
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {filteredOrders.map((order) => {
                        const StatusIcon = statusIcons[order.status as keyof typeof statusIcons] || Clock;
                        return (
                          <tr key={order.id} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap">
                              <div>
                                <div className="text-sm font-medium text-gray-900">
                                  {order.orderNumber}
                                </div>
                                <div className="text-sm text-gray-500">
                                  {order.totalItems} producto{order.totalItems !== 1 ? 's' : ''}
                                </div>
                              </div>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap">
                              <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[order.status as keyof typeof statusColors]}`}>
                                <StatusIcon className="w-3 h-3 mr-1" />
                                {statusLabels[order.status as keyof typeof statusLabels]}
                              </span>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                              {formatPrice(order.total)}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                              {formatDate(order.createdAtDateTime)}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                              <button
                                onClick={() => {
                                  setSelectedOrder(order);
                                  setShowOrderModal(true);
                                }}
                                className="text-green-600 hover:text-green-900 p-1"
                                title="Ver detalles"
                              >
                                <Eye className="w-4 h-4" />
                              </button>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Modal de detalles del pedido */}
      {showOrderModal && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  Detalles del Pedido {selectedOrder.orderNumber}
                </h2>
                <button
                  onClick={() => {
                    setShowOrderModal(false);
                    setSelectedOrder(null);
                  }}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <XCircle className="w-6 h-6" />
                </button>
              </div>

              {/* Progreso del pedido */}
              <div className="mb-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Estado del Pedido</h3>
                <OrderStatusProgress status={selectedOrder.status} />
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Información del pedido */}
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-4">Información del Pedido</h3>
                  <div className="space-y-3">
                    <div>
                      <span className="font-medium text-gray-700">Estado:</span>
                      <span className={`ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${statusColors[selectedOrder.status as keyof typeof statusColors]}`}>
                        {statusLabels[selectedOrder.status as keyof typeof statusLabels]}
                      </span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-700">Total:</span>
                      <span className="ml-2 text-lg font-bold text-green-600">
                        {formatPrice(selectedOrder.total)}
                      </span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-700">Método de pago:</span>
                      <span className="ml-2">{selectedOrder.paymentMethod}</span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-700">Fecha de creación:</span>
                      <span className="ml-2">{formatDate(selectedOrder.createdAtDateTime)}</span>
                    </div>
                    {selectedOrder.shippedDate && (
                      <div>
                        <span className="font-medium text-gray-700">Fecha de envío:</span>
                        <span className="ml-2">{formatDate(selectedOrder.shippedDate)}</span>
                      </div>
                    )}
                    {selectedOrder.deliveredDate && (
                      <div>
                        <span className="font-medium text-gray-700">Fecha de entrega:</span>
                        <span className="ml-2">{formatDate(selectedOrder.deliveredDate)}</span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Información de envío */}
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-4">Información de Envío</h3>
                  <div className="space-y-3">
                    <div>
                      <span className="font-medium text-gray-700">Nombre:</span>
                      <span className="ml-2">{selectedOrder.customerName}</span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-700">Email:</span>
                      <span className="ml-2">{selectedOrder.customerEmail}</span>
                    </div>
                    {selectedOrder.customerPhone && (
                      <div>
                        <span className="font-medium text-gray-700">Teléfono:</span>
                        <span className="ml-2">{selectedOrder.customerPhone}</span>
                      </div>
                    )}
                    <div>
                      <span className="font-medium text-gray-700">Dirección:</span>
                      <span className="ml-2">{selectedOrder.shippingAddress}</span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-700">Ciudad:</span>
                      <span className="ml-2">{selectedOrder.city}, {selectedOrder.zipCode}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Productos del pedido */}
              <div className="mt-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Productos del Pedido</h3>
                <div className="space-y-4">
                  {selectedOrder.orderItems.map((item) => (
                    <div key={item.id} className="flex items-center p-4 border border-gray-200 rounded-lg">
                      <div className="w-16 h-16 bg-gray-100 rounded-lg flex items-center justify-center mr-4">
                        {item.productImage ? (
                          <img
                            src={item.productImage}
                            alt={item.productName}
                            className="w-full h-full object-cover rounded-lg"
                          />
                        ) : (
                          <Package className="w-8 h-8 text-gray-400" />
                        )}
                      </div>
                      <div className="flex-1">
                        <h4 className="font-medium text-gray-900">{item.productName}</h4>
                        <p className="text-sm text-gray-500">Cantidad: {item.quantity}</p>
                        <p className="text-sm text-gray-500">Precio unitario: {formatPrice(item.unitPrice)}</p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium text-gray-900">{formatPrice(item.subtotal)}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Notas */}
              {selectedOrder.notes && (
                <div className="mt-6">
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">Notas</h3>
                  <p className="text-gray-700 bg-gray-50 p-3 rounded-lg">{selectedOrder.notes}</p>
                </div>
              )}

              {/* Acciones */}
              <div className="mt-6 flex justify-end">
                <button
                  onClick={() => {
                    setShowOrderModal(false);
                    setSelectedOrder(null);
                  }}
                  className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
                >
                  Cerrar
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default MyOrdersPage;
