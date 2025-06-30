import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http"; 
import { CreateSupplierDTO, SupplierViewModel, UpdateSupplierDTO } from "../model/supplier-view.model";

@Injectable({
  providedIn: 'root'
})
export class SupplierService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<SupplierViewModel[]>(`${this.baseUrl}/supplier`)     
    }

    post(input : CreateSupplierDTO) {
      return this.http.post<SupplierViewModel>(`${this.baseUrl}/supplier`, input);
    }

    put(input : UpdateSupplierDTO) {
      return this.http.put<SupplierViewModel>(`${this.baseUrl}/supplier`, input);
    }

    delete(id : any) {
      return this.http.delete<SupplierViewModel>(`${this.baseUrl}/supplier/${id}`);
    }

    
 }