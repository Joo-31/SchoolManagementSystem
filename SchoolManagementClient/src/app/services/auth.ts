import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) { }

  // ✅ Register
  register(username: string, password: string, role: string = 'Student'): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, { username, password, role });
  }

  // ✅ Login
  login(username: string, password: string): Observable<{ token: string, role: string, username: string }> {
  return this.http.post<{ token: string, role: string, username: string }>(
    `${this.apiUrl}/login`, { username, password }
  );
}

  // ✅ Save Token
  saveToken(token: string, role: string): void {
  localStorage.setItem('token', token);
  localStorage.setItem('role', role);
}

getRole(): string | null {
  return localStorage.getItem('role');
}

isAdmin(): boolean {
  return this.getRole() === 'Admin';
}

isTeacher(): boolean {
  return this.getRole() === 'Teacher';
}

isStudent(): boolean {
  return this.getRole() === 'Student';
}

  // ✅ Get Token
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // ✅ Logout
  logout(): void {
  localStorage.removeItem('token');
  localStorage.removeItem('role');
  localStorage.removeItem('username');
}

  // ✅ Check if Logged In
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}