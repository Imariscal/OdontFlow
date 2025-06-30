import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http"; 
import { ClientViewModel, CreateClientDto, UpdateClientDto } from "../model/client-vew.model";

@Injectable({
  providedIn: 'root'
})
export class ClientService {

    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get() {
      return this.http.get<ClientViewModel[]>(`${this.baseUrl}/client`)     
    }

    post(input : CreateClientDto) {
      return this.http.post<ClientViewModel>(`${this.baseUrl}/client`, input);
    }

    put(input : UpdateClientDto) {
      return this.http.put<ClientViewModel>(`${this.baseUrl}/client`, input);
    }

    delete(id : any) {
      return this.http.delete<ClientViewModel>(`${this.baseUrl}/client/${id}`);
    }  

    getActive() {
      return this.http.get<ClientViewModel[]>(`${this.baseUrl}/client/active`)     
    }
    
 }