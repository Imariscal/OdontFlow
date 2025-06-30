import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserDTO, UpdateUserDTO, UserViewModel } from '../model/user-view.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private baseUrl = environment.apiBaseUrl;
    private http = inject(HttpClient);

    get(): Observable<UserViewModel[]> {
        return this.http.get<UserViewModel[]>(`${this.baseUrl}/auth`);
    }

    post(input: CreateUserDTO): Observable<UserViewModel> {
        return this.http.post<UserViewModel>(`${this.baseUrl}/auth/RegisterLogin`, input);
    }

    put(input: UpdateUserDTO): Observable<UserViewModel> {
        return this.http.put<UserViewModel>(`${this.baseUrl}/auth/UpdateLogin`, input);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/auth/${id}`);
    }
}
