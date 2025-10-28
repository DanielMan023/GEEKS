export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  productImage?: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface Order {
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

export interface CreateOrderData {
  customerName: string;
  customerEmail: string;
  customerPhone?: string;
  shippingAddress: string;
  city: string;
  zipCode: string;
  paymentMethod: string;
  notes?: string;
}

export interface UpdateOrderStatusData {
  status: string;
  notes?: string;
}
