import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";  
import { CreateEmployeeDto, EmployeeViewModel, UpdateEmployeeDto } from "../model/employee-view.model";

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employee`)     
    }

    post(input : CreateEmployeeDto) {
      return this.http.post<EmployeeViewModel>(`${this.baseUrl}/employee`, input);
    }

    put(input : UpdateEmployeeDto) {
      return this.http.put<EmployeeViewModel>(`${this.baseUrl}/employee`, input);
    }

    delete(id : any) {
      return this.http.delete<EmployeeViewModel>(`${this.baseUrl}/employee/${id}`);
    }  

    getActive() {
        return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employee/active`)     
    }

    getSales() {
      return this.http.get<EmployeeViewModel[]>(`${this.baseUrl}/employee/sales`)     
    }
    
 }