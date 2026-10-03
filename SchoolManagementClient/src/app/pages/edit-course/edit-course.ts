import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { CourseService } from '../../services/course';

@Component({
  selector: 'app-edit-course',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-course.html',
  styleUrl: './edit-course.css'
})
export class EditCourse implements OnInit {
  id: number = 0;
  name: string = '';
  code: string = '';
  credits: number | null = null;
  gradeId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private courseService: CourseService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.courseService.getById(this.id).subscribe({
      next: (course: any) => {
        this.name = course.name || '';
        this.code = course.code || '';
        this.credits = course.credits;
        this.gradeId = course.gradeId;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load course';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (!this.name || !this.code || !this.credits || !this.gradeId) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const updatedCourse = {
      id: this.id,
      name: this.name,
      code: this.code,
      credits: Number(this.credits),
      gradeId: Number(this.gradeId)
    };

    this.courseService.update(this.id, updatedCourse).subscribe({
      next: () => {
        this.successMessage = 'Course updated successfully!';
        setTimeout(() => this.router.navigate(['/courses']), 1000);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to update course';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/courses']);
  }
}