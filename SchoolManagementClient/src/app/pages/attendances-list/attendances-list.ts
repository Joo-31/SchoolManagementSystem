import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AttendanceService } from '../../services/attendance';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-attendances-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './attendances-list.html',
  styleUrl: './attendances-list.css'
})
export class AttendancesList implements OnInit {
  attendances: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private attendanceService: AttendanceService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void { this.loadAttendances(); }

  loadAttendances(): void {
    this.loading = true;
    this.attendanceService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.attendances = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
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

  nextPage(): void { if (this.hasNext) { this.pageNumber++; this.loadAttendances(); } }
  previousPage(): void { if (this.hasPrevious) { this.pageNumber--; this.loadAttendances(); } }
  goToPage(page: number): void { if (page >= 1 && page <= this.totalPages) { this.pageNumber = page; this.loadAttendances(); } }
  getPages(): number[] { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }

  deleteAttendance(id: number): void {
    if (confirm('Are you sure you want to delete this attendance?')) {
      this.attendanceService.delete(id).subscribe({
        next: () => { this.toastService.success('Attendance deleted successfully!', 'Success'); this.loadAttendances(); },
        error: () => { this.toastService.error('Failed to delete attendance', 'Error'); }
      });
    }
  }
}