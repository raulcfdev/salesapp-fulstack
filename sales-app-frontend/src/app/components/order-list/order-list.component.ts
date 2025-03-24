import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { OrderDTO, OrderStatus } from '../../models/order.model';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './order-list.component.html',
  styles: [] 
})
export class OrderListComponent implements OnInit {
  orders: OrderDTO[] = [];
  loading = false;
  statusFilter: OrderStatus | 'all' = 'all';
  orderStatus = OrderStatus;
  
  selectedOrder: OrderDTO | null = null;
  showDetailsModal = false;

  constructor(private orderService: OrderService) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    
    const request = this.statusFilter === 'all' 
      ? this.orderService.getOrders()
      : this.orderService.getOrdersByStatus(this.statusFilter);
      
    request.subscribe({
      next: (data) => {
        this.orders = data;
        this.loading = false;
      },
      error: (error) => {
        console.error(`Error loading orders${this.statusFilter !== 'all' ? ' by status' : ''}`, error);
        this.loading = false;
      }
    });
  }

  updateOrderStatus(orderId: number, status: OrderStatus): void {
    this.orderService.updateOrderStatus(orderId, { status })
      .subscribe({
        next: () => {
          this.loadOrders();
        },
        error: (error) => {
          console.error('Error updating order status', error);
        }
      });
  }

  applyFilter(): void {
    this.loadOrders();
  }

  openDetailsModal(orderId: number): void {
    this.loading = true;
    this.orderService.getOrderById(orderId).subscribe({
      next: (order) => {
        this.selectedOrder = order;
        this.showDetailsModal = true;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading order details', error);
        this.loading = false;
      }
    });
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedOrder = null;
  }
}