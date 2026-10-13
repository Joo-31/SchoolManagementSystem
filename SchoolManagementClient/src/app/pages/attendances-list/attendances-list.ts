import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AttendanceService } from '../../services/attendance';
import { ClassService } from '../../services/class';
import { ToastService } from '../../services/toast';
import { Class } from '../../models/class';

@Component({
  selector: 'app-attendances-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './attendances-list.html',
  styleUrl: './attendances-list.css'
})
export class AttendancesList implements OnInit {
  attendances: any[] = [];
  classes: Class[] = [];

  loading: boolean = true;
  errorMessage: string = '';

  // Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  // Filters
  filterDate: string = this.getTodayDate();
  filterClassId: number | null = null;
  filterStatus: string = '';

  // Statistics (computed from current page)
  statsPresent: number = 0;
  statsAbsent: number = 0;

  constructor(
    private attendanceService: AttendanceService,
    private classService: ClassService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadClasses();
    this.loadAttendances();
  }

  // ✅ Get today's date in "yyyy-MM-dd" format
  getTodayDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  loadClasses(): void {
    this.classService.getAll().subscribe({
      next: (classes: Class[]) => {
        this.classes = classes;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load classes', 'Error');
      }
    });
  }

  loadAttendances(): void {
    this.loading = true;
    this.pageNumber = 1; // reset to first page when filters change
    this.fetchData();
  }

  fetchData(): void {
    this.loading = true;
    this.attendanceService.getFiltered(this.pageNumber, this.pageSize, {
      date: this.filterDate || undefined,
      classId: this.filterClassId || undefined,
      status: this.filterStatus || undefined
    }).subscribe({
      next: (response: any) => {
        this.attendances = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;

        // ✅ Compute stats from current page
        this.computeStats();

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load attendances', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  computeStats(): void {
    this.statsPresent = this.attendances.filter(a => a.isPresent).length;
    this.statsAbsent = this.attendances.filter(a => !a.isPresent).length;
  }

  get attendanceRate(): number {
    if (this.totalCount === 0) return 0;
    // For stats over the current page
    const total = this.statsPresent + this.statsAbsent;
    if (total === 0) return 0;
    return Math.round((this.statsPresent / total) * 100);
  }

  applyFilters(): void {
    this.loadAttendances();
  }

  clearFilters(): void {
    this.filterDate = this.getTodayDate();
    this.filterClassId = null;
    this.filterStatus = '';
    this.loadAttendances();
  }

  nextPage(): void {
    if (this.hasNext) {
      this.pageNumber++;
      this.fetchData();
    }
  }

  previousPage(): void {
    if (this.hasPrevious) {
      this.pageNumber--;
      this.fetchData();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.fetchData();
    }
  }

  getPages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  deleteAttendance(id: number): void {
    if (confirm('Are you sure you want to delete this attendance?')) {
      this.attendanceService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Attendance deleted successfully!', 'Success');
          this.fetchData();
        },
        error: () => {
          this.toastService.error('Failed to delete attendance', 'Error');
        }
      });
    }
  }
}