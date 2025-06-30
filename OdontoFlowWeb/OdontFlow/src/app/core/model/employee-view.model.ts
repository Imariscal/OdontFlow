export interface EmployeeViewModel {
    id?: string;
    name?: string;
    email?: string;
    applyCommission?: boolean;
    isSalesRepresentative?: boolean;
    commissionPercentage?: number;
    active?: boolean; 
  }
  
  export interface EmployeeClientSummaryViewModel {
    id: string;
    name: string;
    commissionPercentage: number; // Comisión personalizada
  }

  export interface CreateEmployeeDto {
    name?: string;
    email?: string;
    applyCommission: boolean;
    isSalesRepresentative: boolean;
    commissionPercentage: number;
    active?: boolean; 
  }

  export interface UpdateEmployeeDto extends CreateEmployeeDto {
    id: string | undefined;
  } 

  export interface CreateClientCommissionDto {
    clientId: string;
    commissionPercentage: number;
  }
  