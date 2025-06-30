import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ProductivityReportViewModel } from '../model/productivity-report.model';
import { GetOrdersByAdvancedFilterQuery } from '../model/get-orders-by-advanced-filter.model';
import { OrderViewModel } from '../model/order.model';
import { PagedResult } from '../model/paged-result.model';
import { WorkedPiecesReportViewModel, WorkedPieceViewModel } from '../model/worked-piece-view.model';
import { ClientOrdersSummary, PatientOrdersSummary } from '../model/patient-orders-summary.model';
import { CommissionOrdersReportViewModel } from '../model/comission-view.model';
import { Answer } from '../model/answer.model';


@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiBaseUrl}/Report`;

  getResume(startDate: Date, endDate: Date): Observable<ProductivityReportViewModel> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());

    return this.http.get<ProductivityReportViewModel>(`${this.baseUrl}/resume`, { params });
  }

  getStationDetail(startDate: Date, endDate: Date): Observable<ProductivityReportViewModel> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());

    return this.http.get<ProductivityReportViewModel>(`${this.baseUrl}/stationDetail`, { params });
  }

  getEmploeeDetail(startDate: Date, endDate: Date): Observable<ProductivityReportViewModel> {
    const params = new HttpParams()
      .set('startDate', startDate.toISOString())
      .set('endDate', endDate.toISOString());

    return this.http.get<ProductivityReportViewModel>(`${this.baseUrl}/employeeDetail`, { params });
  }

  getCommission(orderNumber: string): Observable<[]> {
    const params = new HttpParams()
      .set('orderNumber', orderNumber);

    return this.http.get<[]>(`${this.baseUrl}/getComission`, { params });
  }

  getOrdersByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<OrderViewModel>> {
    return this.http.post<PagedResult<OrderViewModel>>(`${this.baseUrl}/ordersByAdvancedFilter`, query);
  }

  getPaymentsByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<OrderViewModel>> {
    return this.http.post<PagedResult<OrderViewModel>>(`${this.baseUrl}/paymentsByAdvancedFilter`, query);
  }

  // getDebtsByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<OrderViewModel>> {
  //   return this.http.post<PagedResult<OrderViewModel>>(`${this.baseUrl}/debtsByAdvancedFilter`, query);
  // }

  getWorkPiecesByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<WorkedPiecesReportViewModel>> {
    return this.http.post<PagedResult<WorkedPiecesReportViewModel>>(`${this.baseUrl}/workedPiecesByAdvancedFilter`, query);
  }

  getPaymentsAndDebtsByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<ClientOrdersSummary>> {
    return this.http.post<PagedResult<ClientOrdersSummary>>(`${this.baseUrl}/paymentAndDebtsByAdvancedFilter`, query);
  }

  getComissionByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<PagedResult<CommissionOrdersReportViewModel>> {
    return this.http.post<PagedResult<CommissionOrdersReportViewModel>>(`${this.baseUrl}/commisionByAdvancedFilter`, query);
  }

  getDebtsByAdvancedFilter(query: GetOrdersByAdvancedFilterQuery): Observable<Answer<PagedResult<OrderViewModel>>> {
    return this.http.post<Answer<PagedResult<OrderViewModel>>>(`${this.baseUrl}/debtOrdersWithSummary`, query);
  }

  exportDebtOrders(query: GetOrdersByAdvancedFilterQuery): Observable<Answer<OrderViewModel[]>> {
  return this.http.post<Answer<OrderViewModel[]>>(`${this.baseUrl}/debt/export`, query);
}
}
