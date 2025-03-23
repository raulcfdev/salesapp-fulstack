// src/app/services/order.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  OrderDTO, 
  OrderStatus, 
  OrderStatusUpdateDTO,
  CreateOrderDTO
} from '../models/order.model';
import { environment } from '../../enviroments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/api/Orders`;
  
  constructor(private http: HttpClient) { }
  
  getOrders(): Observable<OrderDTO[]> {
    return this.http.get<OrderDTO[]>(this.apiUrl);
  }

  getOrdersByStatus(status: OrderStatus): Observable<OrderDTO[]> {
    return this.http.get<OrderDTO[]>(`${this.apiUrl}?status=${status}`);
  }

  getOrderById(id: number): Observable<OrderDTO> {
    return this.http.get<OrderDTO>(`${this.apiUrl}/${id}`);
  }

  createOrder(orderData: CreateOrderDTO): Observable<number> {
    return this.http.post<number>(this.apiUrl, orderData);
  }

  updateOrderStatus(id: number, statusUpdate: OrderStatusUpdateDTO): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, statusUpdate);
  }
}