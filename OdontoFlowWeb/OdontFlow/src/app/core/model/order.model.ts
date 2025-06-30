import { ClientViewModel } from './client-vew.model';
import { OrderItemViewModel } from './order-item.model';
import { OrderPaymentViewModel } from './order-payment-view.model';

export interface StationWorkCurrentViewModel {
  workStationName?: string;
  employeeStartDate?: string; // ISO String
  employeeName?: string;
}

export interface OrderViewModel {
  id?: string;
  clientId?: string;
  clientName?: string;

  barcode?: string;
  workGroup?: string;
  orderStatusId?: number;
  orderStatus?: string;
  orderTypeId?: number;
  orderType?: string;

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

  creationDate?: string;        // 👈 ya lo dejamos como string ISO
  deliveryDate?: string;
  commitmentDate?: string;
  confirmDate?: string;
  processDate?: string;
  paymentDate?: string;
  completeDate?: string;

  paymentComplete?: boolean;
  metalArticulator?: boolean;
  disposableArticulator?: boolean;

  subtotal?: number;            // 👈 nuevo
  tax?: number;
  total?: number;               // 👈 nuevo
  payment?: number;
  balance?: number;

  collectionNotes?: string;
  deliveryNotes?: string;

  previousOrderId?: string;
  color?: string;
  uncollectible?: boolean;
  applyInvoice?: boolean;
  invoiceNumber?: string;

  active?: boolean;

  items?: OrderItemViewModel[];
  payments?: OrderPaymentViewModel[];
  client?: ClientViewModel;

  currentStationWork?: StationWorkCurrentViewModel;
  daysInDebt? : number;
}
