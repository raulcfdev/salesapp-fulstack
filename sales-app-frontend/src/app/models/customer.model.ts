export interface CustomerDTO {
    customerId: number;
    customerName: string | null;
  }
  
  export interface CreateCustomerDTO {
    customerName: string | null;
  }