import { HttpClient } from '@angular/common/http';
import { Injectable, effect, signal } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiBaseUrl;
  private tokenKey = 'token';
  private _token = signal<string | null>(localStorage.getItem(this.tokenKey)); 

  constructor(private http: HttpClient, private router: Router) {
    effect(() => {
      const token = this._token();
      if (token && this.isTokenExpired()) {
        this.logout();
      }
    });
  }

  login(email: string, password: string) {
    return this.http.post<{ token: string }>(`${this.baseUrl}/auth/login`, { email, password })     
  }

  resetPassword(NewPassword: string) {
    return this.http.post<{ token: string }>(`${this.baseUrl}/auth/changePassword`, { NewPassword })     
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this._token.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return this._token();
  }

  setToken(token :string): void {
    this._token.set(token);
  }
 
 
  getUserFromToken(): any {
    const token = this._token();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload;
    } catch {
      return null;
    }
  } 
  
  isTokenExpired(): boolean {
    const payload = this.getUserFromToken();
    if (!payload || !payload.exp) return true;
  
    const now = Math.floor(Date.now() / 1000); // Tiempo actual en segundos
    return now >= payload.exp;
  }

  isAuthenticated() {
    return !!this._token();
  }

  getUserRole(): string | null {
    const user = this.getUserFromToken();
    return user?.Role || null;
  }
 
  
  hasRole(role: string): boolean {
    return this.getUserRole() === role;
  }
  
  hasAnyRole(roles: string[]): boolean {
    return roles.includes(this.getUserRole() || '');
  }
  
}
