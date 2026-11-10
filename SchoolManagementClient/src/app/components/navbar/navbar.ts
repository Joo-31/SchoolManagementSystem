import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule], // ✅ ضيفنا TranslateModule
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {
  username: string = '';
  isDarkMode: boolean = false;
  isSidebarOpen: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    public translate: TranslateService // ✅ ضيفنا TranslateService
  ) {
    this.username = localStorage.getItem('username') || '';
    
    // ✅ إعداد اللغة الافتراضية
    this.translate.setDefaultLang('en');
    const savedLang = localStorage.getItem('lang') || 'en';
    this.translate.use(savedLang);
  }

  // ✅ دالة تغيير اللغة
   switchLanguage(lang: string) {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    // ✅ لو عايز نعكس اتجاه الصفحة للعربي
    if (lang === 'ar') {
      document.documentElement.dir = 'rtl';
    } else {
      document.documentElement.dir = 'ltr';
    }
  }

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode = true;
      document.body.classList.add('dark-theme');
    }
  }

  toggleSidebar(): void { this.isSidebarOpen = !this.isSidebarOpen; }
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
    if (this.isAdmin()) this.router.navigate(['/dashboard']);
    else if (this.isTeacher()) this.router.navigate(['/my-classes']);
    else if (this.isStudent()) this.router.navigate(['/my-profile-student']);
    else this.router.navigate(['/login']);
  }
}