import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Student } from '../models/student';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private apiUrl = `${environment.apiUrl}/students`;

  constructor(private http: HttpClient) { }

  // ✅ Get All
  getAll(): Observable<Student[]> {
    return this.http.get<Student[]>(this.apiUrl);
  }

  // ✅ Get By ID
  getById(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }

  // ✅ Add
  add(student: any): Observable<Student> {
    return this.http.post<Student>(this.apiUrl, student);
  }

  // ✅ Update
  update(id: number, student: any): Observable<Student> {
    return this.http.put<Student>(`${this.apiUrl}/${id}`, student);
  }

  // ✅ Delete
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // ✅ Search
  search(keyword: string): Observable<Student[]> {
    return this.http.get<Student[]>(`${this.apiUrl}/search?keyword=${keyword}`);
  }

  // ✅ Pagination
  getPaged(pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  getMyProfile(): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/me`);
}

getMyAttendances(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/me/attendances`);
}
}