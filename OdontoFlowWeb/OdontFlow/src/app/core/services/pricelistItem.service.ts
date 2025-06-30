import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http"; 
import { CreatePriceListItemDTO, PriceListItemViewModel, UpdatePriceListItemDTO } from "../model/pricelistItem-view.model";

@Injectable({
  providedIn: 'root'
})
export class PriceListItemService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);
 
    get(id : any) {
      return this.http.get<PriceListItemViewModel[]>(`${this.baseUrl}/PriceListItem/${id}`);
    }

    post(input : CreatePriceListItemDTO) {
      return this.http.post<PriceListItemViewModel>(`${this.baseUrl}/PriceListItem`, input);
    }

    put(input : UpdatePriceListItemDTO) {
      return this.http.put<PriceListItemViewModel>(`${this.baseUrl}/PriceListItem`, input);
    }

    delete(id : any) {
      return this.http.delete<PriceListItemViewModel>(`${this.baseUrl}/PriceListItem/${id}`);
    }    
 }