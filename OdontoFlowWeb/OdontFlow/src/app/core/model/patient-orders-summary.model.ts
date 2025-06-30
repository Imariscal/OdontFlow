import { OrderPaymentViewModel } from "./order-payment-view.model";
import { OrderViewModel } from "./order.model";

export interface ClientOrdersSummary {
  clientName: string;
  patients: PatientOrdersSummary[];
}

export interface PatientOrdersSummary {
  patientName: string;
  paidCount: number;
  debtCount: number;
  payments: OrderPaymentViewModel[];
  debtOrders: OrderViewModel[];
}
