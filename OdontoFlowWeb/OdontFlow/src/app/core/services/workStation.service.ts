import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";  
import { CreateWorkStationDto, UpdateWorkStationDto, WorkStationViewModel } from "../model/workStation.model";

@Injectable({
  providedIn: 'root'
})
export class WorkStationService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<WorkStationViewModel[]>(`${this.baseUrl}/workStation`)     
    }

    
    getActive() {
      return this.http.get<WorkStationViewModel[]>(`${this.baseUrl}/workStation/active`)     
    }

    post(input : CreateWorkStationDto) {
      return this.http.post<WorkStationViewModel>(`${this.baseUrl}/workStation`, input);
    }

    put(input : UpdateWorkStationDto) {
      return this.http.put<WorkStationViewModel>(`${this.baseUrl}/workStation`, input);
    }

    delete(id : any) {
      return this.http.delete<WorkStationViewModel>(`${this.baseUrl}/workStation/${id}`);
    }  
    
 }