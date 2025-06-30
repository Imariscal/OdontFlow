export interface SupplierViewModel {
    id?: string;                       
    name?: string;
    contact?: string;
    email?: string;
    phone1?: string;
    phone2?: string;
    bankDetails?: string;
    invoiceContact?: string;
    invoiceEmail?: string;
    invoicePhone?: string;
    account?: string;
    credit?: number;               
    creditFormatted?: string;
    comments?: string;
    product?: string;
    address?: string;
    active?: boolean;
  }

  export interface CreateSupplierDTO {
    name?: string;
    contact?: string;
    email?: string;
    phone1?: string;
    phone2?: string;
    bankDetails?: string;
    invoiceContact?: string;
    invoiceEmail?: string;
    invoicePhone?: string;
    account?: string;
    credit?: number;               
    creditFormatted?: string;
    comments?: string;
    product?: string;
    address?: string;
    active?: boolean;
  }
  
  export interface UpdateSupplierDTO extends CreateSupplierDTO {
    id: string | undefined;  
  }
  