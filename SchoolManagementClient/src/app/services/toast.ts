import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  type: 'success' | 'error' | 'info' | 'warning';
  message: string;
  title?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private toastSubject = new Subject<ToastMessage>();
  public toasts$ = this.toastSubject.asObservable();

  show(type: 'success' | 'error' | 'info' | 'warning', message: string, title?: string): void {
    this.toastSubject.next({ type, message, title });
  }

  success(message: string, title?: string): void {
    this.show('success', message, title);
  }

  error(message: string, title?: string): void {
    this.show('error', message, title);
  }

  info(message: string, title?: string): void {
    this.show('info', message, title);
  }

  warning(message: string, title?: string): void {
    this.show('warning', message, title);
  }
}