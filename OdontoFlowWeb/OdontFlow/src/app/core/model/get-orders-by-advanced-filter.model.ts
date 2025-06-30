export interface GetOrdersByAdvancedFilterQuery {
    search?: string;
    orderStatusId?: number;
    orderTypeId?: number;
    groupId?: string;
    paymentTypeId? : string;
    clientName?: string;
    patientName?: string;
    requesterName?: string;
    creationDateStart?: string | Date;
    creationDateEnd?: string | Date;
    commitmentDateStart?: string;
    commitmentDateEnd?: string;
    paymentComplete?: boolean;
    applyInvoice?: boolean;
    minBalance?: number;
    maxBalance?: number;
    page: number;
    pageSize: number;
    productIds?: string[];
  }
  