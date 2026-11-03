import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { StudentService } from '../../services/student';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-add-student',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-student.html',
  styleUrl: './add-student.css'
})
export class AddStudent {
  firstName: string = '';
  lastName: string = '';
  birthYear: number | null = null;
  birthMonth: number | null = null;
  birthDay: number | null = null;
  gender: number = 0;
  classId: number | null = null;
  
  // ✅ الحقول الجديدة
  username: string = '';
  password: string = '';
  
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private studentService: StudentService,
    private router: Router,
    private toastService: ToastService 
  ) { }

  onSubmit(): void {
    if (!this.firstName || !this.lastName) {
      this.errorMessage = 'First name and last name are required';
      return;
    }

    if (!this.birthYear || !this.birthMonth || !this.birthDay) {
      this.errorMessage = 'Birth date is required';
      return;
    }

    if (!this.classId) {
      this.errorMessage = 'Class ID is required';
      return;
    }

    // ✅ Validation للـ Username والـ Password
    if (!this.username || !this.password) {
      this.errorMessage = 'Username and Password are required for the account';
      return;
    }

    const birthDate = `${this.birthYear}-${String(this.birthMonth).padStart(2, '0')}-${String(this.birthDay).padStart(2, '0')}`;

    const newStudent = {
      firstName: this.firstName,
      lastName: this.lastName,
      birthDate: birthDate,
      gender: Number(this.gender),
      classId: Number(this.classId),
      username: this.username,
      password: this.password
    };

    this.studentService.add(newStudent).subscribe({
      next: () => {
        this.toastService.success('Student and account created successfully!', 'Success');
        setTimeout(() => this.router.navigate(['/students']), 1000);
      },
      error: (error: any) => {
        this.toastService.error('Failed to add student', 'Error');
        console.error(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/students']);
  }
}