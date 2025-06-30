import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";  
import { CreatePriceListDTO, PriceListViewModel, UpdatePriceListDTO } from "../model/pricelist-view.model";

@Injectable({
  providedIn: 'root'
})
export class PriceListService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<PriceListViewModel[]>(`${this.baseUrl}/pricelist`)     
    }

    getOnlyActive() {
      return this.http.get<PriceListViewModel[]>(`${this.baseUrl}/pricelist/GetOnlyActive`)     
    }

    post(input : CreatePriceListDTO) {
      return this.http.post<PriceListViewModel>(`${this.baseUrl}/pricelist`, input);
    }

    put(input : UpdatePriceListDTO) {
      return this.http.put<PriceListViewModel>(`${this.baseUrl}/pricelist`, input);
    }

    delete(id : any) {
      return this.http.delete<PriceListViewModel>(`${this.baseUrl}/pricelist/${id}`);
    }

    
 }