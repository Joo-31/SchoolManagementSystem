import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { NotificationService } from '../../services/notification';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-add-notification',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-notification.html',
  styleUrl: './add-notification.css'
})
export class AddNotification {
  title: string = '';
  message: string = '';
  targetRole: string = 'All'; // Default

  constructor(
    private notificationService: NotificationService,
    private toastService: ToastService,
    private router: Router
  ) { }

  onSubmit(): void {
    if (!this.title || !this.message) {
      this.toastService.error('Title and Message are required', 'Error');
      return;
    }

    const payload = {
      title: this.title,
      message: this.message,
      targetRole: this.targetRole
    };

    this.notificationService.createNotification(payload).subscribe({
      next: () => {
        this.toastService.success('Notification sent successfully!', 'Success');
        this.router.navigate(['/dashboard']); // أو ترجع للصفحة اللي إنت جاي منها
      },
      error: (err) => {
        this.toastService.error('Failed to send notification', 'Error');
        console.error(err);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/dashboard']);
  }
}