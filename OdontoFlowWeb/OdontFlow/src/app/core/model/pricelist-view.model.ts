export interface PriceListViewModel {
    id?: string;                       
    name?: string; 
    category?: string;
    categoryId?: Number;
    discount?: number;
    active?:boolean;
  }
  
  export interface CreatePriceListDTO {
    name?: string;
    contact?: string;
    category?: number;
    categoryId?: Number;
    discount?: Number;   
    active?:boolean; 
  }
  
  export interface UpdatePriceListDTO extends CreatePriceListDTO {
    id: string | undefined;  
  }
  
