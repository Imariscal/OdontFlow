export interface ProductViewModel {
    id?: string;  
    name?: string;
    price?: Number;
    productCategory?: string;
    productCategoryId?: Number;
    priceFormatted?: string;
    isLabelRequired?: boolean;
    applyDiscount?: boolean;
    active?: boolean;
  }

  export interface CreateProductDTO {
    name?: string;
    productCategory?: Number;
    price?: string;
    isLabelRequired?: boolean;
    applyDiscount?: boolean;
    active?: boolean;
  }

  export interface UpdateProductDTO {
    id?: string;  
    name?: string;
    productCategory?: Number;
    price?: string;
    isLabelRequired?: boolean;
    applyDiscount?: boolean;
    active?: boolean;
  }