import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class HomeworkService {
  private apiUrl = `${environment.apiUrl}/homework`;

  constructor(private http: HttpClient) { }

  // ✅ للمدرس: إضافة واجب
  addHomework(homework: any): Observable<any> {
    return this.http.post(this.apiUrl, homework);
  }

  // ✅ للمدرس: عرض واجباته
  getTeacherHomework(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-homework`);
  }

  // ✅ للطالب: عرض الواجبات اللي تخصه
  getStudentHomework(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/student`);
  }

  // ✅ حذف واجب (للمدرس أو الأدمن)
  deleteHomework(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}