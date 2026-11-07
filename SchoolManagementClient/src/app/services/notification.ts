import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) { }

  // ✅ جلب التنبيهات الخاصة بالمستخدم الحالي
  getMyNotifications(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // ✅ إنشاء تنبيه جديد (للأدمن فقط)
  createNotification(notification: { title: string, message: string, targetRole: string }): Observable<any> {
    return this.http.post(this.apiUrl, notification);
  }

  // ✅ حذف تنبيه (للأدمن فقط)
  deleteNotification(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}