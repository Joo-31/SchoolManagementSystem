import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';

@Component({
  selector: 'app-add-teacher',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-teacher.html',
  styleUrl: './add-teacher.css'
})
export class AddTeacher {
  name: string = '';
  email: string = '';
  phone: string = '';
  specialization: string = '';
  hireYear: number | null = null;
  hireMonth: number | null = null;
  hireDay: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private teacherService: TeacherService,
    private router: Router
  ) { }

  onSubmit(): void {
    if (!this.name || !this.email || !this.phone || !this.specialization) {
      this.errorMessage = 'All fields are required';
      return;
    }

    if (!this.hireYear || !this.hireMonth || !this.hireDay) {
      this.errorMessage = 'Hire date is required';
      return;
    }

    const hireDate = `${this.hireYear}-${String(this.hireMonth).padStart(2, '0')}-${String(this.hireDay).padStart(2, '0')}`;

    const newTeacher = {
      name: this.name,
      email: this.email,
      phone: this.phone,
      specialization: this.specialization,
      hireDate: hireDate
    };

    this.teacherService.add(newTeacher).subscribe({
      next: () => {
        this.successMessage = 'Teacher added successfully!';
        setTimeout(() => this.router.navigate(['/teachers']), 1000);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to add teacher';
        console.error(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/teachers']);
  }
}