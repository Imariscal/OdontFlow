export interface WorkPlanViewModel {
    id?: string;
    name?: string;
    stations?: WorkPlanStationViewModel[];
    products?: WorkPlanProductViewModel[];
    active? : boolean;
  }
  
  export interface WorkPlanStationViewModel {
    workStationId?: string;
    workStationName?: string;
    order: number;
  }
  
  export interface WorkPlanProductViewModel {
    productId: string;
    productName?: string;
  }

  
  export interface CreateWorkPlanDto {
    name?: string;
    stations: WorkPlanStationInput[];
    productIds: string[];
    active? : boolean;
  }
  
  export interface WorkPlanStationInput {
    workStationId: string;
    order: number;
  }

  
  export interface UpdateWorkPlanDto extends CreateWorkPlanDto {
    id?: string;
  }