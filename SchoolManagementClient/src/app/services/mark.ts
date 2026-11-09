import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Mark } from '../models/mark';

@Injectable({ providedIn: 'root' })
export class MarkService {
  private apiUrl = `${environment.apiUrl}/marks`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Mark[]> {
    return this.http.get<Mark[]>(this.apiUrl);
  }

  getById(id: number): Observable<Mark> {
    return this.http.get<Mark>(`${this.apiUrl}/${id}`);
  }

  add(mark: any): Observable<Mark> {
    return this.http.post<Mark>(this.apiUrl, mark);
  }

  update(id: number, mark: any): Observable<Mark> {
    return this.http.put<Mark>(`${this.apiUrl}/${id}`, mark);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getMyMarks(): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/me`);
  }

  getMarksByTeacher(): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/my-marks`);
  }

  getByStudent(studentId: number): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/student/${studentId}`);
  }

  getByCourse(courseId: number): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/course/${courseId}`);
  }

  getMyCourses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-courses`);
  }

  getMyStudents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-students`);
  }

  // ✅ الدالة دي بس اللي محتاجينها هنا
  getPaged(pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}