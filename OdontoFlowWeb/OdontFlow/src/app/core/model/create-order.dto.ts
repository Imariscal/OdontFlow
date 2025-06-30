import { CreateOrderItemDto } from "./order-item.model";

export interface CreateOrderDto {
    clientId?: string;  
    workGroupId?: number;
    orderStatusId?: number;
    orderTypeId?: number;
    colorId?: number;
  
    requesterName?: string;
    patientName?: string;
  
    bite?: boolean;
    models?: boolean;
    casts?: boolean;
    spoons?: boolean;
    attachments?: boolean;
    analogs?: boolean;
    screws?: boolean;
  
    others?: string;
    observations?: string;
  
    entryDate?: string;
    deliveryDate?: string;
    commitmentDate?: string;
  
    metalArticulator?: boolean;
    disposableArticulator?: boolean;
  
    cost?: number;
    balance?: number;
    payment?: number;
    tax?: number;
  
    collectionNotes?: string;
    deliveryNotes?: string;
  
    previousOrderId?: string;
    color?: string;
  
    priceListId?: number;
    uncollectible?: boolean;
    applyInvoice?: boolean;
    invoiceNumber?: string;
 
  
    items?: CreateOrderItemDto[];
  }
  