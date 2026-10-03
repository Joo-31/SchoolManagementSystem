import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.html',
styleUrl: './login.css'
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

 onLogin(): void {
  if (!this.username || !this.password) {
    this.errorMessage = 'Please enter username and password';
    return;
  }

  this.authService.login(this.username, this.password).subscribe({
    next: (response: { token: string, role: string, username: string }) => {
      this.authService.saveToken(response.token, response.role);
      localStorage.setItem('username', response.username);
      this.router.navigate(['/students']);
    },
    error: (error: any) => {
      this.errorMessage = 'Invalid username or password';
    }
  });
}
}