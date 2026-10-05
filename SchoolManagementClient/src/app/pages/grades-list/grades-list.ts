import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GradeService } from '../../services/grade';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-grades-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './grades-list.html',
  styleUrl: './grades-list.css'
})
export class GradesList implements OnInit {
  grades: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private gradeService: GradeService,
    private cdr: ChangeDetectorRef,
      private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadGrades();
  }

  loadGrades(): void {
    this.gradeService.getAll().subscribe({
      next: (data: any[]) => {
        this.grades = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load grades';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

 deleteGrade(id: number): void {
  if (confirm('Are you sure you want to delete this grade?')) {
    this.gradeService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Grade deleted successfully!', 'Success');
        this.loadGrades();
      },
      error: (error: any) => {
        this.toastService.error('Failed to delete grade', 'Error');
      }
    });
    }
  }
}