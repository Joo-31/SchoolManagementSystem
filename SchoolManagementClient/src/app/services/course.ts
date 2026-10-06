import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Course } from '../models/course';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private apiUrl = `${environment.apiUrl}/courses`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Course[]> {
    return this.http.get<Course[]>(this.apiUrl);
  }

  getById(id: number): Observable<Course> {
    return this.http.get<Course>(`${this.apiUrl}/${id}`);
  }

  add(course: any): Observable<Course> {
    return this.http.post<Course>(this.apiUrl, course);
  }

  update(id: number, course: any): Observable<Course> {
    return this.http.put<Course>(`${this.apiUrl}/${id}`, course);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  search(keyword: string): Observable<Course[]> {
    return this.http.get<Course[]>(`${this.apiUrl}/search?keyword=${keyword}`);
  }
  getCount(): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/count`);
}
getPaged(pageNumber: number, pageSize: number): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/paged?pageNumber=${pageNumber}&pageSize=${pageSize}`);
}
}