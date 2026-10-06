import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Mark } from '../models/mark';

@Injectable({ providedIn: 'root' })
export class MarkService {
  private apiUrl = `${environment.apiUrl}/marks`;

  constructor(private http: HttpClient) { }

  // Admin + Teacher
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

  // Student
  getMyMarks(): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/me`);
  }

  // Teacher
  getMarksByTeacher(): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/my-marks`);
  }

  // By Student (Admin/Teacher)
  getByStudent(studentId: number): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/student/${studentId}`);
  }

  // By Course
  getByCourse(courseId: number): Observable<Mark[]> {
    return this.http.get<Mark[]>(`${this.apiUrl}/course/${courseId}`);
  }

  getMyCourses(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/my-courses`);
}

getMyStudents(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/my-students`);
}
}