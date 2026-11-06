import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ ضيفنا دي عشان الفلتر
import { StudentService } from '../../services/student';

@Component({
  selector: 'app-my-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule], // ✅ ضيفنا FormsModule
  templateUrl: './my-attendance.html',
  styleUrl: './my-attendance.css'
})
export class MyAttendance implements OnInit {
  allAttendances: any[] = [];       // كل الداتا من السيرفر
  filteredAttendances: any[] = [];  // الداتا بعد الفلترة بالتاريخ
  attendances: any[] = [];          // الداتا اللي بتتعرض في الصفحة الحالية
  
  loading: boolean = true;
  errorMessage: string = '';

  // ✅ متغير الفلتر
  filterDate: string = '';

  // ✅ متغيرات الـ Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadAttendances();
  }

  loadAttendances(): void {
    this.studentService.getMyAttendances().subscribe({
      next: (data: any[]) => {
        this.allAttendances = data;
        this.filteredAttendances = data; // في الأول مفيش فلتر
        this.calculatePagination();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load attendances';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ✅ دالة الفلترة بالتاريخ
  applyDateFilter(): void {
    this.pageNumber = 1; // نرجع لأول صفحة لما نفلتر
    
    if (!this.filterDate) {
      this.filteredAttendances = this.allAttendances;
    } else {
      this.filteredAttendances = this.allAttendances.filter(a => {
        // بنقارن التاريخ (سنة-شهر-يوم) بس
        const attendanceDate = new Date(a.date).toISOString().split('T')[0];
        return attendanceDate === this.filterDate;
      });
    }
    
    this.calculatePagination();
  }

  // ✅ مسح الفلتر
  clearFilter(): void {
    this.filterDate = '';
    this.filteredAttendances = this.allAttendances;
    this.pageNumber = 1;
    this.calculatePagination();
  }

  // ✅ حساب الصفحات وتقسيم الداتا
  calculatePagination(): void {
    this.totalPages = Math.ceil(this.filteredAttendances.length / this.pageSize) || 1;
    const startIndex = (this.pageNumber - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.attendances = this.filteredAttendances.slice(startIndex, endIndex);
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
}