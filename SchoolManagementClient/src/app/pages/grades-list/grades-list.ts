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
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private gradeService: GradeService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void { this.loadGrades(); }

  loadGrades(): void {
    this.loading = true;
    this.gradeService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.grades = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load grades', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void { if (this.hasNext) { this.pageNumber++; this.loadGrades(); } }
  previousPage(): void { if (this.hasPrevious) { this.pageNumber--; this.loadGrades(); } }
  goToPage(page: number): void { if (page >= 1 && page <= this.totalPages) { this.pageNumber = page; this.loadGrades(); } }
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
  deleteGrade(id: number): void {
    if (confirm('Are you sure you want to delete this grade?')) {
      this.gradeService.delete(id).subscribe({
        next: () => { this.toastService.success('Grade deleted successfully!', 'Success'); this.loadGrades(); },
        error: () => { this.toastService.error('Failed to delete grade', 'Error'); }
      });
    }
  }
}