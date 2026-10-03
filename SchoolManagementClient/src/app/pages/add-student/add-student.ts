import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { StudentService } from '../../services/student';

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
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private studentService: StudentService,
    private router: Router
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

    const birthDate = `${this.birthYear}-${String(this.birthMonth).padStart(2, '0')}-${String(this.birthDay).padStart(2, '0')}`;

    const newStudent = {
      firstName: this.firstName,
      lastName: this.lastName,
      birthDate: birthDate,
      gender: Number(this.gender),
      classId: Number(this.classId)
    };

    this.studentService.add(newStudent).subscribe({
      next: () => {
        this.successMessage = 'Student added successfully!';
        setTimeout(() => this.router.navigate(['/students']), 1000);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to add student';
        console.error(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/students']);
  }
}