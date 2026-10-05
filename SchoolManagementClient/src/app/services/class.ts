import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Class } from '../models/class';

@Injectable({ providedIn: 'root' })
export class ClassService {
  private apiUrl = `${environment.apiUrl}/classes`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Class[]> {
    return this.http.get<Class[]>(this.apiUrl);
  }

  getById(id: number): Observable<Class> {
    return this.http.get<Class>(`${this.apiUrl}/${id}`);
  }

  add(classObj: any): Observable<Class> {
    return this.http.post<Class>(this.apiUrl, classObj);
  }

  update(id: number, classObj: any): Observable<Class> {
    return this.http.put<Class>(`${this.apiUrl}/${id}`, classObj);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  getCount(): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/count`);
}
}