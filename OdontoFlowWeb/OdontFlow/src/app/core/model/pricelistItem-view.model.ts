import { ProductViewModel } from "./product-view.model";

export interface PriceListItemViewModel {
  id?: string;
  productId?: string;
  productName?: string;
  priceListId?: string;
  priceListName?: string;
  price?: number;
  product?: ProductViewModel, 
  comments?: string
}

export interface CreatePriceListItemDTO {
  productId: string;
  priceListId: string;
  price: number;
  comments?: string;
}


export interface UpdatePriceListItemDTO {
  id: any;
  price: number;
  comments?: string;
}