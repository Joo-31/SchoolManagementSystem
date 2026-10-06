import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-teachers-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './teachers-list.html',
  styleUrl: './teachers-list.css'
})
export class TeachersList implements OnInit {
  teachers: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private teacherService: TeacherService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadTeachers();
  }

  loadTeachers(): void {
    this.loading = true;
    this.teacherService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.teachers = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.toastService.error('Failed to load teachers', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void {
    if (this.hasNext) { this.pageNumber++; this.loadTeachers(); }
  }

  previousPage(): void {
    if (this.hasPrevious) { this.pageNumber--; this.loadTeachers(); }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadTeachers();
    }
  }

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  deleteTeacher(id: number): void {
    if (confirm('Are you sure you want to delete this teacher?')) {
      this.teacherService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Teacher deleted successfully!', 'Success');
          this.loadTeachers();
        },
        error: () => {
          this.toastService.error('Failed to delete teacher', 'Error');
        }
      });
    }
  }
}