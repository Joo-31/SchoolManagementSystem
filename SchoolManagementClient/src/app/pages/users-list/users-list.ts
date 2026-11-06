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
  loading: boolean = true;
  
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
        this.users = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load users', 'Error');
        this.loading = false;
      }
    });
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