import { CreateOrderDto } from './create-order.dto';

export interface UpdateOrderDto extends CreateOrderDto {
  id?: string;
}
