import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-marks-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './marks-list.html',
  styleUrl: './marks-list.css'
})
export class MarksList implements OnInit {
  marks: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private markService: MarkService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadMarks();
  }

  loadMarks(): void {
    this.markService.getAll().subscribe({
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