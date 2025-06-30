import { OrderViewModel } from "./order.model";

export interface CommissionOrdersReportViewModel {
  items: CommissionOrderDetailItem[];
  summary: CommissionEmployeeSummary[];
  totalRecords: number;
}

export interface CommissionOrderDetailItem {
  employeeName: string;
  orderBarcode: string;
  clientName: string;
  creationDate: Date;
  orderTotal: number;
  commissionPercentage: number;
  commissionAmount: number;
  order: OrderViewModel
}

export interface CommissionEmployeeSummary {
  employeeName: string;
  totalOrders: number;
  totalCommission: number;
  totalAmount: number;
}
