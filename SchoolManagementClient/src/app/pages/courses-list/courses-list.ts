import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CourseService } from '../../services/course';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-courses-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './courses-list.html',
  styleUrl: './courses-list.css'
})
export class CoursesList implements OnInit {
  courses: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private courseService: CourseService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void { this.loadCourses(); }

  loadCourses(): void {
    this.loading = true;
    this.courseService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.courses = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load courses', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void { if (this.hasNext) { this.pageNumber++; this.loadCourses(); } }
  previousPage(): void { if (this.hasPrevious) { this.pageNumber--; this.loadCourses(); } }
  goToPage(page: number): void { if (page >= 1 && page <= this.totalPages) { this.pageNumber = page; this.loadCourses(); } }
  getPages(): number[] { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }

  deleteCourse(id: number): void {
    if (confirm('Are you sure you want to delete this course?')) {
      this.courseService.delete(id).subscribe({
        next: () => { this.toastService.success('Course deleted successfully!', 'Success'); this.loadCourses(); },
        error: () => { this.toastService.error('Failed to delete course', 'Error'); }
      });
    }
  }
}