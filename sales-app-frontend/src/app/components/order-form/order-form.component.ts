import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { OrderService } from '../../services/order.service';
import { CustomerDTO, CreateOrderDTO, CreateOrderItemDTO } from '../../models/order.model';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './order-form.component.html',
  styles: []
})
export class OrderFormComponent implements OnInit {
  orderForm: FormGroup;
  newCustomerForm: FormGroup;
  customers: CustomerDTO[] = [];
  loading = false;
  submitting = false;
  errorMessage = '';
  selectedCustomerId?: number;
  showNewCustomerForm = false;
  creatingCustomer = false;

  constructor(
    private fb: FormBuilder,
    private customerService: CustomerService,
    private orderService: OrderService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.orderForm = this.fb.group({
      customerRefId: ['', Validators.required],
      orderItems: this.fb.array([])
    });

    this.newCustomerForm = this.fb.group({
      customerName: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.loadCustomers();
    
    this.route.queryParams.subscribe(params => {
      const customerId = params['customerId'];
      if (customerId) {
        this.selectedCustomerId = +customerId;
        this.orderForm.get('customerRefId')?.setValue(this.selectedCustomerId);
      }
    });
    
    this.addOrderItem();
  }

  get orderItems(): FormArray {
    return this.orderForm.get('orderItems') as FormArray;
  }

  createOrderItem(): FormGroup {
    return this.fb.group({
      productName: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  addOrderItem(): void {
    this.orderItems.push(this.createOrderItem());
  }

  removeOrderItem(index: number): void {
    if (this.orderItems.length > 1) {
      this.orderItems.removeAt(index);
    }
  }

  calculateSubtotal(index: number): number {
    const item = this.orderItems.at(index).value;
    return item.quantity * item.unitPrice;
  }

  calculateTotal(): number {
    return this.orderItems.controls.reduce((total, itemControl) => {
      const item = itemControl.value;
      return total + (item.quantity * item.unitPrice);
    }, 0);
  }

  loadCustomers(): void {
    this.loading = true;
    this.customerService.getCustomers()
      .subscribe({
        next: (data) => {
          this.customers = data;
          this.loading = false;
          
          if (this.customers.length === 0) {
            this.showNewCustomerForm = true;
          }
        },
        error: (error: any) => {
          console.error('Error loading customers', error);
          this.loading = false;
          this.errorMessage = 'Erro ao carregar lista de clientes.';
        }
      });
  }

  toggleNewCustomerForm(): void {
    this.showNewCustomerForm = !this.showNewCustomerForm;
    if (!this.showNewCustomerForm) {
      this.newCustomerForm.reset();
    }
  }

  createNewCustomer(): void {
    if (this.newCustomerForm.invalid) {
      this.markFormGroupTouched(this.newCustomerForm);
      return;
    }

    this.creatingCustomer = true;
    this.customerService.createCustomer(this.newCustomerForm.value)
      .subscribe({
        next: (customerId) => {
          const newCustomer: CustomerDTO = {
            customerId: customerId,
            customerName: this.newCustomerForm.value.customerName
          };
          this.customers.push(newCustomer);
          
          this.orderForm.get('customerRefId')?.setValue(customerId);
          
          this.newCustomerForm.reset();
          this.showNewCustomerForm = false;
          this.creatingCustomer = false;
        },
        error: (error) => {
          console.error('Error creating customer', error);
          this.errorMessage = 'Erro ao criar cliente. Tente novamente.';
          this.creatingCustomer = false;
        }
      });
  }

  onSubmit(): void {
    if (this.orderForm.invalid) {
      this.markFormGroupTouched(this.orderForm);
      return;
    }
  
    this.submitting = true;
    
    const formValue = this.orderForm.value;
    const newOrder: CreateOrderDTO = {
      customerRefId: Number(formValue.customerRefId),
      orderItems: formValue.orderItems
    };
  
    this.orderService.createOrder(newOrder)
      .subscribe({
        next: () => {
          this.router.navigate(['/orders']);
        },
        error: (error: any) => {
          console.error('Error creating order', error);
          this.errorMessage = 'Erro ao criar pedido. Tente novamente.';
          this.submitting = false;
        }
      });
  }

  markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach(item => {
          if (item instanceof FormGroup) {
            this.markFormGroupTouched(item);
          }
        });
      }
    });
  }
}