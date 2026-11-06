import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {
  username: string = '';
  isDarkMode: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.username = localStorage.getItem('username') || '';
  }

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark-theme');
    }
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isTeacher(): boolean {
    return this.authService.isTeacher();
  }

  isStudent(): boolean {
    return this.authService.isStudent();
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    if (this.isDarkMode) {
      document.body.classList.add('dark-theme');
      localStorage.setItem('theme', 'dark');
    } else {
      document.body.classList.remove('dark-theme');
      localStorage.setItem('theme', 'light');
    }
  }

  // ✅ دالة توجيه الـ Role للصفحة الرئيسية بتاعته
  navigateToHome(): void {
    if (this.isAdmin()) {
      this.router.navigate(['/dashboard']);
    } else if (this.isTeacher()) {
      this.router.navigate(['/my-classes']);
    } else if (this.isStudent()) {
      this.router.navigate(['/my-profile-student']); // أو /dashboard لو عملت داشبورد للطالب
    } else {
      this.router.navigate(['/login']);
    }
  }
}