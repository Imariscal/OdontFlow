import { OrderViewModel } from "./order.model";

export interface WorkedPieceViewModel {
  id: string;
  employeeName: string;
  order: OrderViewModel;
  productName: string;
  teethDetails: string[];
  employeeStartDate: Date;
  employeeEndDate: Date;
  stationName: string;
  quantity: number;
  unitCost: number;
  subtotal: number;
  totalTax: number;
  total: number;
}

export interface ProductPiecesChart {
  productName: string;
  pieces: number;
}

export interface LabTechnicianSummaryViewModel {
  laboratorista: string;
  ordenes: number;
  productos: {
    [productName: string]: number;
  };
}

export interface WorkedPiecesReportViewModel {
  items: WorkedPieceViewModel[];
  topProducts: ProductPiecesChart[];
  allProducts: ProductPiecesChart[];
  consolidadoPorLaboratorista: LabTechnicianSummaryViewModel[]; 
}
