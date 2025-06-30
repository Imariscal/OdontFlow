import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";  
import { StationWorkSummaryViewModel } from "../model/station-work-summary.model";

@Injectable({
  providedIn: 'root'
})
export class LabService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<StationWorkSummaryViewModel>(`${this.baseUrl}/lab`)     
    } 

    getStationDetails(id: any) {
      return this.http.get<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/workstationDetails`)     
    } 

    postProcessOrder(id: any) {
      return this.http.post<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/processOrder`, {})     
    }        

    postCompleteOrder(id: any) {
      return this.http.post<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/completeOrder`, {})     
    } 
    
    rejectOrder(id: any, message: string) {
      return this.http.post<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/rejectOrder`, { message})     
    } 

    blockOrder(id: any, message: string) {
      return this.http.post<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/blockOrder`, { message})     
    } 

    unBlockOrder(id: any) {
      return this.http.post<StationWorkSummaryViewModel>(`${this.baseUrl}/lab/${id}/unBlockOrder`, {})     
    } 
 }