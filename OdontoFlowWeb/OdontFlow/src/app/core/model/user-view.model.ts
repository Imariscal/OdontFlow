export interface UserViewModel {
    id?: string;
    email?: string;
    roleId?: number; 
    roleName?: string; 
    changePassword?: boolean;
    employeeId?: string | null;
    employeeName?: string | null;
    clientId?: string | null;
    clientName?: string | null;
    createdAt?: string;
    updatedAt?: string;
    active?: boolean
  }

  
  export interface CreateUserDTO {
    email: string; 
    role?: number;
    changePassword?: boolean;   
    employeeId?: string | null;
    clientId?: string | null;
    active? : boolean
  }

  export interface UpdateUserDTO {
    id: any;
    email?: string;
    role?: number; 
    changePassword?: boolean;
    employeeId?: string | null;
    clientId?: string | null;
    active? : boolean
  }
  
  