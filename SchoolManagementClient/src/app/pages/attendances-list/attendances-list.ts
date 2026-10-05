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

  constructor(
    private attendanceService: AttendanceService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadAttendances();
  }

  loadAttendances(): void {
    this.attendanceService.getAll().subscribe({
      next: (data: any[]) => {
        this.attendances = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load attendances';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteAttendance(id: number): void {
  if (confirm('Are you sure you want to delete this attendance?')) {
    this.attendanceService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Attendance deleted successfully!', 'Success');
        this.loadAttendances();
      },
      error: (error: any) => {
        this.toastService.error('Failed to delete attendance', 'Error');
      }
    });
    }
  }
}