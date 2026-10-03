import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CourseService } from '../../services/course';

@Component({
  selector: 'app-add-course',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-course.html',
  styleUrl: './add-course.css'
})
export class AddCourse {
  name: string = '';
  code: string = '';
  credits: number | null = null;
  gradeId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private courseService: CourseService,
    private router: Router
  ) { }

  onSubmit(): void {
    if (!this.name || !this.code || !this.credits || !this.gradeId) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const newCourse = {
      name: this.name,
      code: this.code,
      credits: Number(this.credits),
      gradeId: Number(this.gradeId)
    };

    this.courseService.add(newCourse).subscribe({
      next: () => {
        this.successMessage = 'Course added successfully!';
        setTimeout(() => this.router.navigate(['/courses']), 1000);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to add course';
        console.error(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/courses']);
  }
}