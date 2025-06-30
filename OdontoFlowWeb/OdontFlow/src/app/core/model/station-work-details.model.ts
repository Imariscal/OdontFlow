export interface StationWorkDetailViewModel {
    orderId? : string;
    orderNumber?: string;
    barcode?: string;
    stationWorkId?: string;
    productName?: string;
    orderColor?: string;
    clientName?: string;
    teeth?: string[];
    teethDetails?: string;
    previousStationName?: string;
    previousEmployeeName?: string;
    previousEndDate?: string;  
    stationStardDate?: string;
    stationEndDate?: string;
    workStatusIndicator?:number
    workStationName?: string;
    workStationId?: string;
    workStatus?: number;
    inProgress?: boolean;
    employeeEndDate?: string;  
    employeeStartDate?: string;
    step? : number;
    workedOnTime?: boolean;
    productivityPercent?:number;
    realTime?: string;
    estimatedTime?:string;
    currentEmployee?:string;
    delayMinutes?:number;
  }
  