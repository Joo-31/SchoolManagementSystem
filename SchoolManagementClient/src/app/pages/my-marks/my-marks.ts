import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';   // ← جديد
import { MarkService } from '../../services/mark';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-my-marks',
  standalone: true,
  imports: [CommonModule, RouterModule],   // ← RouterModule
  templateUrl: './my-marks.html',
  styleUrl: './my-marks.css'
})
export class MyMarks implements OnInit {
  marks: any[] = [];
  loading: boolean = true;
  isTeacher: boolean = false;

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
        this.marks = data;
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

  // ✅ جديد
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