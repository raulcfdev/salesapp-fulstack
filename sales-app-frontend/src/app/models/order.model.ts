// src/app/models/order.model.ts

export interface OrderItemDTO {
  productName: string | null;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface OrderDTO {
  orderId: number;
  customerName: string | null;
  orderDate: string;
  orderTotal: number;
  orderStatus: string | null;
  orderItems: OrderItemDTO[] | null;
}

export enum OrderStatus {
  Pending = "Pending",
  Processed = "Processed"
}

export interface OrderStatusUpdateDTO {
  status: string | null;
}

export interface CustomerDTO {
  customerId: number;
  customerName: string | null;
}

export interface CreateOrderItemDTO {
  productName: string | null;
  quantity: number;
  unitPrice: number;
}

export interface CreateOrderDTO {
  customerRefId: number;
  orderItems: CreateOrderItemDTO[] | null;
}