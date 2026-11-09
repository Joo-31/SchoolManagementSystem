import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth'; 
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users-list.html',
  styleUrl: './users-list.css'
})
export class UsersList implements OnInit {
  users: any[] = [];
  allUsers: any[] = []; // ✅ بنحفظ الداتا الأصلية
  loading: boolean = true;
  
  // ✅ متغيرات الـ Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;

  selectedUser: any = null;
  newPassword: string = '';
  isResetModalOpen: boolean = false;

  constructor(
    private authService: AuthService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.authService.getAllUsers().subscribe({
      next: (data) => {
        this.allUsers = data;
        this.totalPages = Math.ceil(this.allUsers.length / this.pageSize) || 1;
        this.updatePageData();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load users', 'Error');
        this.loading = false;
      }
    });
  }

  // ✅ تقسيم الداتا
  updatePageData(): void {
    const startIndex = (this.pageNumber - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.users = this.allUsers.slice(startIndex, endIndex);
  }

  // ✅ دوال التنقل
  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.updatePageData();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.updatePageData();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.updatePageData();
    }
  }

  // ✅ ذكاء عرض الصفحات (5 بس)
  getPages(): number[] {
    const total = this.totalPages;
    const current = this.pageNumber;
    const maxVisible = 5;

    if (total <= maxVisible) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    let start = Math.max(current - Math.floor(maxVisible / 2), 1);
    let end = start + maxVisible - 1;

    if (end > total) {
      end = total;
      start = Math.max(end - maxVisible + 1, 1);
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }

  openResetModal(user: any): void {
    if (user.role === 'Admin') {
      this.toastService.error('Cannot reset password for Admin accounts.', 'Security Alert');
      return;
    }
    this.selectedUser = user;
    this.newPassword = '';
    this.isResetModalOpen = true;
  }

  closeResetModal(): void {
    this.isResetModalOpen = false;
    this.selectedUser = null;
  }

  generatePassword(): void {
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$";
    let password = "";
    for (let i = 0; i < 8; i++) {
      password += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    this.newPassword = password;
  }

  confirmReset(): void {
    if (!this.newPassword || this.newPassword.length < 6) {
      this.toastService.error('Password must be at least 6 characters', 'Error');
      return;
    }

    this.authService.resetPassword(this.selectedUser.id, this.newPassword).subscribe({
      next: () => {
        this.toastService.success(`Password reset for ${this.selectedUser.username}! New Password: ${this.newPassword}`, 'Success');
        this.closeResetModal();
      },
      error: (err) => {
        this.toastService.error(err.error || 'Failed to reset password', 'Error');
      }
    });
  }
}