import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";   
import { CreateWorkPlanDto, UpdateWorkPlanDto, WorkPlanViewModel } from "../model/workplan-view.mode";

@Injectable({
  providedIn: 'root'
})
export class WorkPlanService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<WorkPlanViewModel[]>(`${this.baseUrl}/workPlan`)     
    }

    post(input : CreateWorkPlanDto) {
      return this.http.post<WorkPlanViewModel>(`${this.baseUrl}/workPlan`, input);
    }

    put(input : UpdateWorkPlanDto) {
      return this.http.put<WorkPlanViewModel>(`${this.baseUrl}/workPlan`, input);
    }

    delete(id : any) {
      return this.http.delete<WorkPlanViewModel>(`${this.baseUrl}/workPlan/${id}`);
    }  
    
 }