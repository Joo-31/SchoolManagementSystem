import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ ضيفنا دي عشان البحث
import { RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-my-marks',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // ✅ ضيفنا FormsModule
  templateUrl: './my-marks.html',
  styleUrl: './my-marks.css'
})
export class MyMarks implements OnInit {
  allMarks: any[] = [];        // كل الداتا من السيرفر
  filteredMarks: any[] = [];   // الداتا بعد البحث
  marks: any[] = [];           // الداتا اللي بتتعرض في الصفحة الحالية
  
  loading: boolean = true;
  isTeacher: boolean = false;
  searchKeyword: string = '';

  // ✅ متغيرات الـ Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;

  constructor(
    private markService: MarkService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.isTeacher = this.authService.isTeacher();
    this.loadMarks();
  }

  loadMarks(): void {
    const obs = this.isTeacher
      ? this.markService.getMarksByTeacher()
      : this.markService.getMyMarks();

    obs.subscribe({
      next: (data: any[]) => {
        this.allMarks = data;
        this.filteredMarks = data;
        this.calculatePagination();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.toastService.error('Failed to load marks', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ دالة البحث
  applySearch(): void {
    this.pageNumber = 1; // نرجع لأول صفحة
    
    if (!this.searchKeyword || this.searchKeyword.trim() === '') {
      this.filteredMarks = this.allMarks;
    } else {
      const term = this.searchKeyword.toLowerCase();
      this.filteredMarks = this.allMarks.filter(m => 
        (m.courseName && m.courseName.toLowerCase().includes(term)) ||
        (this.isTeacher && m.studentName && m.studentName.toLowerCase().includes(term))
      );
    }
    
    this.calculatePagination();
  }

  clearSearch(): void {
    this.searchKeyword = '';
    this.applySearch();
  }

  // ✅ دالة تقسيم الصفحات
  calculatePagination(): void {
    this.totalPages = Math.ceil(this.filteredMarks.length / this.pageSize) || 1;
    const startIndex = (this.pageNumber - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.marks = this.filteredMarks.slice(startIndex, endIndex);
  }

  // ✅ دوال التنقل
  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;
      this.calculatePagination();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.calculatePagination();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.calculatePagination();
    }
  }

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  deleteMark(id: number): void {
    if (confirm('Are you sure you want to delete this mark?')) {
      this.markService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Mark deleted successfully!', 'Success');
          this.loadMarks();
        },
        error: (error: any) => {
          this.toastService.error('Failed to delete mark', 'Error');
        }
      });
    }
  }
}