import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ ضيفنا دي عشان الـ ngModel
import { RouterModule } from '@angular/router';
import { StudentService } from '../../services/student';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // ✅ ضيفنا FormsModule
  templateUrl: './students-list.html',
  styleUrl: './students-list.css'
})
export class StudentsList implements OnInit {
  students: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  // ✅ متغير الـ Search
  searchKeyword: string = '';
  isSearching: boolean = false;

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

  // ✅ دالة الـ Search
  onSearch(): void {
    if (!this.searchKeyword || this.searchKeyword.trim() === '') {
      this.isSearching = false;
      this.pageNumber = 1;
      this.loadStudents();
      return;
    }

    this.isSearching = true;
    this.loading = true;
    this.studentService.search(this.searchKeyword).subscribe({
      next: (data) => {
        this.students = data;
        this.totalCount = data.length; // في البحث بنعرض العدد الكلي للنتايج
        this.totalPages = 1; // مفيش Pagination في البحث
        this.hasPrevious = false;
        this.hasNext = false;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to search students', 'Error');
        this.loading = false;
      }
    });
  }

  clearSearch(): void {
    this.searchKeyword = '';
    this.isSearching = false;
    this.pageNumber = 1;
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

    getPages(): number[] {
    const total = this.totalPages;
    const current = this.pageNumber;
    const maxVisible = 5; // عرض 5 أزرار بس

    if (total <= maxVisible) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    let start = Math.max(current - Math.floor(maxVisible / 2), 1);
    let end = start + maxVisible - 1;

    if (end > total) {
      end = total;
      start = Math.max(end - maxVisible + 1, 1);
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
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