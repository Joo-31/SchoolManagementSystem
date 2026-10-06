import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Teacher } from '../models/teacher';

@Injectable({ providedIn: 'root' })
export class TeacherService {
  private apiUrl = `${environment.apiUrl}/teachers`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(this.apiUrl);
  }

  getById(id: number): Observable<Teacher> {
    return this.http.get<Teacher>(`${this.apiUrl}/${id}`);
  }

  add(teacher: any): Observable<Teacher> {
    return this.http.post<Teacher>(this.apiUrl, teacher);
  }

  update(id: number, teacher: any): Observable<Teacher> {
    return this.http.put<Teacher>(`${this.apiUrl}/${id}`, teacher);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  search(keyword: string): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(`${this.apiUrl}/search?keyword=${keyword}`);
  }

  getMyClasses(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/me/classes`);
  }
  getStudentsByClass(classId: number): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/me/classes/${classId}/students`);
}
getMyProfile(): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/me`);
}
getCount(): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/count`);
}
getPaged(pageNumber: number, pageSize: number): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`);
}
}