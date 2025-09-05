export interface CartDTO {
  id: number;
  userId: number;
  cartItems: CartItemDTO[];
  total: number;
  totalItems: number;
}

export interface CartItemDTO {
  id: number;
  productId: number;
  productName: string;
  productImage: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface AddToCartDTO {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemDTO {
  cartItemId: number;
  quantity: number;
}

export interface RemoveFromCartDTO {
  cartItemId: number;
}

export interface ServiceResponse<T> {
  success: boolean;
  data?: T;
  message: string;
}
