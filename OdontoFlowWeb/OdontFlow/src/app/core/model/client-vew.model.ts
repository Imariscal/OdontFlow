export interface ClientViewModel {
  id?: string;
  name?: string;
  address?: string;
  contact?: string;
  phone1?: string;
  phone2?: string;
  mobile?: string;
  generalEmail?: string;
  collectionEmail?: string;
  credit?: number;
  appliesInvoice?: boolean;
  groupId?: number;
  workGroup?: string;
  stateId?: number;
  remarks?: string;
  priceListId?: string;
  priceList?: string;
  employeeId?: number;
  employeeName?: string;
  account?: string;
  active?: boolean;
  clientInvoice?: ClientInvoiceDto;
  commissionPercentage? : number;
  
}

export interface CreateClientDto {
  name?: string;
  address?: string;
  contact?: string;
  phone1?: string;
  phone2?: string;
  mobile?: string;
  generalEmail?: string;
  collectionEmail?: string;
  credit?: number;
  appliesInvoice?: boolean;
  groupId?: number;
  stateId?: number;
  remarks?: string;
  priceListId?: string;
  salesEmployeeId?: number;
  account?: string;
  active?: boolean;
  clientInvoice?: ClientInvoiceDto | null;
  employeeId?: string;
  employeeName?: string;
  commissionPercentage?: number;
}
  
export interface UpdateClientDto extends CreateClientDto {
  id?: string;
}

  export interface ClientInvoiceDto {
    id?: string;
    invoiceName?: string;
    invoiceRFC?: string;
    invoiceEmail?: string;
    phone?: string;
    cfdiUse?: string;
    regimen?: string;
    street?: string;
    exteriorNumber?: string;
    interiorNumber?: string;
    city?: string;
    zipCode?: string;
    municipality?: string;
    state?: string;
    country?: string;
  }