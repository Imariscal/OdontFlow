export interface OrderPaymentViewModel {
    id: string;
    paymentTypeId: number;
    paymentType: string;
    amount: number;
    barCode? :string;
    patientName? :string;
    clientName? :string;
    reference?: string;
    creationDate?: Date;
  }

  export interface CreateOrderPaymentDto {
    orderId: string;
    paymentTypeId: number;
    amount: number;
    reference?: string;
  }

  export interface UpdateOrderPaymentDto  extends CreateOrderPaymentDto {
    id: string; 
  }

