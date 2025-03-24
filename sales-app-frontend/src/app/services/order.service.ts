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
    return this.http.get<OrderDTO[]>(`${this.apiUrl}/list`);
  }

  getOrdersByStatus(status: OrderStatus): Observable<OrderDTO[]> {
    return this.http.get<OrderDTO[]>(`${this.apiUrl}/list?status=${status}`);
  }

  getOrderById(id: number): Observable<OrderDTO> {
    return this.http.get<OrderDTO>(`${this.apiUrl}/details/${id}`);
  }

  createOrder(orderData: CreateOrderDTO): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/create`, orderData);
  }

  updateOrderStatus(id: number, statusUpdate: OrderStatusUpdateDTO): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-status/${id}`, statusUpdate);
  }
}