import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../services/student';

@Component({
  selector: 'app-my-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './my-attendance.html',
  styleUrl: './my-attendance.css'
})
export class MyAttendance implements OnInit {
  allAttendances: any[] = [];
  filteredAttendances: any[] = [];
  attendances: any[] = [];
  
  loading: boolean = true;
  errorMessage: string = '';

  // ✅ متغيرات الإحصائيات
  statsPresent: number = 0;
  statsAbsent: number = 0;
  statsRate: number = 0;

  filterDate: string = '';

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
        this.filteredAttendances = data;
        this.calculateStats();
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

  // ✅ حساب الإحصائيات
  calculateStats(): void {
    // بنحسب الإحصائيات من الداتا الأصلية (كل الغياب) عشان النسبة متتأثرش بالفلترة
    this.statsPresent = this.allAttendances.filter(a => a.isPresent).length;
    this.statsAbsent = this.allAttendances.filter(a => !a.isPresent).length;
    
    const total = this.allAttendances.length;
    this.statsRate = total > 0 ? Math.round((this.statsPresent / total) * 100) : 0;
  }

  applyDateFilter(): void {
    this.pageNumber = 1;
    
    if (!this.filterDate) {
      this.filteredAttendances = this.allAttendances;
    } else {
      this.filteredAttendances = this.allAttendances.filter(a => {
        const attendanceDate = new Date(a.date).toISOString().split('T')[0];
        return attendanceDate === this.filterDate;
      });
    }
    
    this.calculatePagination();
  }

  clearFilter(): void {
    this.filterDate = '';
    this.filteredAttendances = this.allAttendances;
    this.pageNumber = 1;
    this.calculatePagination();
  }

  calculatePagination(): void {
    this.totalPages = Math.ceil(this.filteredAttendances.length / this.pageSize) || 1;
    const startIndex = (this.pageNumber - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.attendances = this.filteredAttendances.slice(startIndex, endIndex);
  }

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