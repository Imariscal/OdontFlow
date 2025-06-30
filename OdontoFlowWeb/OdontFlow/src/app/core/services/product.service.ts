import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateProductDTO, ProductViewModel, UpdateProductDTO } from "../model/product-view.model";

@Injectable({
  providedIn: 'root'
})
export class ProductService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<ProductViewModel[]>(`${this.baseUrl}/product`)     
    }

    post(input : CreateProductDTO) {
      return this.http.post<ProductViewModel>(`${this.baseUrl}/product`, input);
    }

    put(input : UpdateProductDTO) {
      return this.http.put<ProductViewModel>(`${this.baseUrl}/product`, input);
    }

    delete(id : any) {
      return this.http.delete<ProductViewModel>(`${this.baseUrl}/product/${id}`);
    }

    getByCategory(id : any) {
      return this.http.get<any>(`${this.baseUrl}/product/ByCategory/${id}`)     
    }

    getWithoutPlan() {
      return this.http.get<ProductViewModel[]>(`${this.baseUrl}/product/WithoutPlan`)     
    }

    getByPlanId(id : any) {
      return this.http.get<ProductViewModel[]>(`${this.baseUrl}/product/${id}/byPlanId`)     
    }

    
 }