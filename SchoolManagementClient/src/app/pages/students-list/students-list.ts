import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StudentService } from '../../services/student';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './students-list.html',
  styleUrl: './students-list.css'
})
export class StudentsList implements OnInit {
  students: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  // ✅ Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.loading = true;
    this.studentService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.students = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.toastService.error('Failed to load students', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ Pagination Methods
  nextPage(): void {
    if (this.hasNext) {
      this.pageNumber++;
      this.loadStudents();
    }
  }

  previousPage(): void {
    if (this.hasPrevious) {
      this.pageNumber--;
      this.loadStudents();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadStudents();
    }
  }

  // ✅ للحصول على قائمة الصفحات
  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  deleteStudent(id: number): void {
    if (confirm('Are you sure you want to delete this student?')) {
      this.studentService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Student deleted successfully!', 'Success');
          this.loadStudents();
        },
        error: (error: any) => {
          this.toastService.error('Failed to delete student', 'Error');
        }
      });
    }
  }
}