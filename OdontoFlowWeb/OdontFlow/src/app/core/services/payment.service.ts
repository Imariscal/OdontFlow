import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";  
import { CreateOrderPaymentDto, OrderPaymentViewModel, UpdateOrderPaymentDto } from "../model/order-payment-view.model";

@Injectable({
  providedIn: 'root'
})
export class OrderPaymentService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get(id : any) {
      return this.http.get<OrderPaymentViewModel[]>(`${this.baseUrl}/OrderPayment/${id}`)     
    }

    post(input : CreateOrderPaymentDto) {
      return this.http.post<OrderPaymentViewModel>(`${this.baseUrl}/OrderPayment`, input);
    }

    put(input : UpdateOrderPaymentDto) {
      return this.http.put<OrderPaymentViewModel>(`${this.baseUrl}/OrderPayment`, input);
    }

    delete(id : any) {
      return this.http.delete<OrderPaymentViewModel>(`${this.baseUrl}/OrderPayment/${id}`);
    }  
    
 }