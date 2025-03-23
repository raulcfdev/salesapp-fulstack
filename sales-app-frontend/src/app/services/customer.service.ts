// src/app/services/customer.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerDTO } from '../models/order.model';
import { environment } from '../../enviroments/enviroment';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private apiUrl = `${environment.apiUrl}/api/Customers`;
  
  constructor(private http: HttpClient) { }
  
  getCustomers(): Observable<CustomerDTO[]> {
    return this.http.get<CustomerDTO[]>(this.apiUrl);
  }

  getCustomerById(id: number): Observable<CustomerDTO> {
    return this.http.get<CustomerDTO>(`${this.apiUrl}/${id}`);
  }

  createCustomer(customer: any): Observable<number> {
    return this.http.post<number>(this.apiUrl, customer);
  }
}