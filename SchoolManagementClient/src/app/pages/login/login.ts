import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast';
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
    private router: Router,
    private toastService: ToastService
  ) { }

 onLogin(): void {
  if (!this.username || !this.password) {
  this.errorMessage = 'Please enter username and password';
  this.toastService.warning('Please fill in all fields', 'Warning');  
  return;
}

 this.authService.login(this.username, this.password).subscribe({
  next: (response: { token: string, role: string, username: string, teacherId: number | null, studentId: number | null }) => {
    this.authService.saveUser(response.token, response.role, response.username, response.teacherId, response.studentId);
    
    // ✅ وجه حسب الـ Role
   if (response.role === 'Admin') {
  this.router.navigate(['/dashboard']);
} else if (response.role === 'Teacher') {
  this.router.navigate(['/my-classes']);
} else if (response.role === 'Student') {
  this.router.navigate(['/my-profile-student']);
}
  },
  error: (error: any) => {
  this.errorMessage = 'Invalid username or password';
  this.toastService.error('Invalid username or password', 'Login Failed');
}
});
}
}