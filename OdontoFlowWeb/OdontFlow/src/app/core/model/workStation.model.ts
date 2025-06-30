export interface WorkStationViewModel {
    id?: string;
    name?: string;
    days?: number; 
    active?: boolean;
  }
  
  export interface CreateWorkStationDto {
    name?: string;
    days?: number; 
    active?: boolean;
  }
    
  export interface UpdateWorkStationDto extends CreateWorkStationDto {
    id?: string;
  }
 