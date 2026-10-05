import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, ToastMessage } from '../../services/toast';

interface DisplayToast extends ToastMessage {
  id: number;
  show: boolean;
}

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrl: './toast.css'
})
export class ToastComponent implements OnInit {
  toasts: DisplayToast[] = [];
  private nextId = 1;

  constructor(
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.toastService.toasts$.subscribe((toast: ToastMessage) => {
      const displayToast: DisplayToast = {
        ...toast,
        id: this.nextId++,
        show: true
      };

      this.toasts.push(displayToast);
      this.cdr.detectChanges();

      // ✅ اخفي الـ Toast بعد 3 ثواني
      setTimeout(() => {
        const index = this.toasts.findIndex(t => t.id === displayToast.id);
        if (index > -1) {
          this.toasts[index].show = false;
          this.cdr.detectChanges();

          // ✅ امسحه بعد الأنيميشن
          setTimeout(() => {
            this.toasts = this.toasts.filter(t => t.id !== displayToast.id);
            this.cdr.detectChanges();
          }, 300);
        }
      }, 3000);
    });
  }

  closeToast(id: number): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
    this.cdr.detectChanges();
  }
}