import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}Auth`; // koristi environment URL
  private tokenKey = 'token';
  private userKey = 'user';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password });
  }

  register(email: string, password: string, displayName: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, { email, password, displayName });
  }

  saveUser(user: { displayName: string; token: string }) {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  getUser(): { displayName: string; token: string } | null {
    const data = localStorage.getItem(this.userKey);
    return data ? JSON.parse(data) : null;
  }

  getDisplayName(): string | null {
    const user = this.getUser();
    return user ? user.displayName : null;
  }

  isLoggedIn(): boolean {
    return !!this.getUser();
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }
}