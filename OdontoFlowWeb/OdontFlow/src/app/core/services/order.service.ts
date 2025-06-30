import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { OrderViewModel } from "../model/order.model";
import { CreateOrderDto } from "../model/create-order.dto";
import { UpdateOrderDto } from "../model/update-order.dto";

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private baseUrl = environment.apiBaseUrl;
  private http = inject(HttpClient);

  getPaged(query: {
    page: number;
    pageSize: number;
    sortField?: any;
    sortOrder?: any;
    global?: string;
    filters?: { [key: string]: { value: string; matchMode: string } };
  }) {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize);

    if (query.sortField)
      params = params.set('sortField', query.sortField);
    if (query.sortOrder != null)
      params = params.set('sortOrder', query.sortOrder.toString());
    if (query.global)
      params = params.set('global', query.global);

    if (query.filters) {
      // Codificamos el objeto como string JSON para enviarlo en query string
      params = params.set('filters', JSON.stringify(query.filters));
    }

    return this.http.get<OrderViewModel[]>(`${this.baseUrl}/order`, { params })
  }



  get() {

    return this.http.get<OrderViewModel[]>(`${this.baseUrl}/order`)
  }

  post(input: CreateOrderDto) {
    return this.http.post<OrderViewModel>(`${this.baseUrl}/order`, input);
  }

  put(input: UpdateOrderDto) {
    return this.http.put<OrderViewModel>(`${this.baseUrl}/order`, input);
  }

  delete(id: any) {
    return this.http.delete<OrderViewModel>(`${this.baseUrl}/order/${id}`);
  }

  confirm(id: any) {
    return this.http.put<OrderViewModel>(`${this.baseUrl}/order/${id}/confirm`, {});
  }

  deleteItem(id: any) {
    return this.http.delete<OrderViewModel>(`${this.baseUrl}/order/${id}/deleteOrderItem`);
  }

  deliver(id: any) {
    return this.http.delete<OrderViewModel>(`${this.baseUrl}/order/${id}/deliver`);
  }

  getById(id: any) {
    return this.http.get<OrderViewModel[]>(`${this.baseUrl}/order/${id}/byId`)
  }


  printOrderPdf(id: any): void {
    const url = `${this.baseUrl}/order/${id}/printOrder`;

    this.http.get(url, { responseType: 'blob' }).subscribe(blob => {
      const blobUrl = URL.createObjectURL(blob);
      const printWindow = window.open(blobUrl, '_blank');

      const interval = setInterval(() => {
        if (printWindow?.document?.readyState === 'complete') {
          printWindow.focus();
          printWindow.print();
          clearInterval(interval);
        }
      }, 500);
    });
  }

  printNoteGamma(id: any): void {
    const url = `${this.baseUrl}/order/${id}/printGamma`;

    this.http.get(url, { responseType: 'blob' }).subscribe(blob => {
      const blobUrl = URL.createObjectURL(blob);
      const printWindow = window.open(blobUrl, '_blank');

      const interval = setInterval(() => {
        if (printWindow?.document?.readyState === 'complete') {
          printWindow.focus();
          printWindow.print();
          clearInterval(interval);
        }
      }, 500);
    });
  }

  printNoteZirconia(id: any): void {
    const url = `${this.baseUrl}/order/${id}/printZirconia`;

    this.http.get(url, { responseType: 'blob' }).subscribe(blob => {
      const blobUrl = URL.createObjectURL(blob);
      const printWindow = window.open(blobUrl, '_blank');

      const interval = setInterval(() => {
        if (printWindow?.document?.readyState === 'complete') {
          printWindow.focus();
          printWindow.print();
          clearInterval(interval);
        }
      }, 500);
    });
  }

}