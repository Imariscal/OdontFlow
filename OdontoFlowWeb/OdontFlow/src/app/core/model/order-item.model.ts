export interface OrderItemViewModel {
  id?: string;
  productId: string;
  productName?: string;
  
  quantity: number;
  unitCost: number;
  unitTax: number;        
  totalCost: number;  

  teeth: number[];   
  teethNames?:   string
}

export interface CreateOrderItemDto {
  productId: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
  teeth: number[];
}

export interface UpdateOrderItemDto {
  id: string;          
  productId: string;
  quantity: number;
  unitCost: number;
  totalCost: number;
  teeth: number[];
}