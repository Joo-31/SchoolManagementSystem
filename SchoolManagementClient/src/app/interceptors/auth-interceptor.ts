import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  // ✅ ضيف الـ Token لو موجود
  let clonedRequest = req;
  if (token) {
    clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // ✅ استقبل الـ Response
  return next(clonedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      // ✅ لو 401 — Token انتهى
      if (error.status === 401) {
        // ✅ امسح الـ Storage
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('username');
        localStorage.removeItem('teacherId');
        localStorage.removeItem('studentId');

        // ✅ وجه للـ Login
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};