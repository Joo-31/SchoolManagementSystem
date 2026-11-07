import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { NotificationService } from '../../services/notification'; // ✅ ضيفنا دي

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
  
  // ✅ متغيرات التنبيهات
  notifications: any[] = [];
  showNotifications: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private notificationService: NotificationService // ✅ ضيفنا دي
  ) {
    this.username = localStorage.getItem('username') || '';
  }

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark-theme');
    }

    // ✅ بنجيب التنبيهات أول ما الصفحة تفتح
    if (this.authService.isLoggedIn()) {
      this.loadNotifications();
    }
  }

  // ✅ دالة جلب التنبيهات
  loadNotifications(): void {
    this.notificationService.getMyNotifications().subscribe({
      next: (data) => {
        this.notifications = data;
      },
      error: (err) => console.error('Failed to load notifications', err)
    });
  }

  // ✅ زرار فتح وقفل قائمة التنبيهات
  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
  }

  isAdmin(): boolean { return this.authService.isAdmin(); }
  isTeacher(): boolean { return this.authService.isTeacher(); }
  isStudent(): boolean { return this.authService.isStudent(); }

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

  navigateToHome(): void {
    if (this.isAdmin()) {
      this.router.navigate(['/dashboard']);
    } else if (this.isTeacher()) {
      this.router.navigate(['/my-classes']);
    } else if (this.isStudent()) {
      this.router.navigate(['/my-profile-student']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}