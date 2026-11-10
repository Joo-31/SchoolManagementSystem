import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Navbar } from './components/navbar/navbar';
import { ToastComponent } from './components/toast/toast';
import { TranslateService } from '@ngx-translate/core'; // ✅ ضيف ده

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, Navbar, ToastComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private translate: TranslateService) {
    // ✅ هنقرا اللغة المحفوظة أول ما البرنامج يفتح
    this.translate.setDefaultLang('en');
    const savedLang = localStorage.getItem('lang') || 'en';
    this.translate.use(savedLang);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
}