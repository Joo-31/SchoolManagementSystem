import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Grade } from '../models/grade';

@Injectable({ providedIn: 'root' })
export class GradeService {
  private apiUrl = `${environment.apiUrl}/grades`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Grade[]> {
    return this.http.get<Grade[]>(this.apiUrl);
  }

  getById(id: number): Observable<Grade> {
    return this.http.get<Grade>(`${this.apiUrl}/${id}`);
  }

  add(grade: any): Observable<Grade> {
    return this.http.post<Grade>(this.apiUrl, grade);
  }

  update(id: number, grade: any): Observable<Grade> {
    return this.http.put<Grade>(`${this.apiUrl}/${id}`, grade);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}