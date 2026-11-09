import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-marks-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './marks-list.html',
  styleUrl: './marks-list.css'
})
export class MarksList implements OnInit {
  marks: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  searchKeyword: string = '';

  // ✅ Pagination
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private markService: MarkService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadMarks();
  }

  loadMarks(): void {
    this.loading = true;
    this.markService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.marks = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load marks', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  applySearch(): void {
    if (!this.searchKeyword || this.searchKeyword.trim() === '') {
      this.loadMarks();
      return;
    }

    this.loading = true;
    this.markService.getAll().subscribe({
      next: (data: any[]) => {
        const term = this.searchKeyword.toLowerCase();
        this.marks = data.filter(m => 
          (m.studentName && m.studentName.toLowerCase().includes(term)) || 
          (m.courseName && m.courseName.toLowerCase().includes(term))
        );
        this.totalCount = this.marks.length;
        this.totalPages = 1;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to search marks', 'Error');
        this.loading = false;
      }
    });
  }

  clearSearch(): void {
    this.searchKeyword = '';
    this.loadMarks();
  }

  nextPage(): void { if (this.hasNext) { this.pageNumber++; this.loadMarks(); } }
  previousPage(): void { if (this.hasPrevious) { this.pageNumber--; this.loadMarks(); } }
  goToPage(page: number): void { if (page >= 1 && page <= this.totalPages) { this.pageNumber = page; this.loadMarks(); } }

  // ✅ الدالة دي هي اللي بتظبط شكل الصفحات (5 بس)
  getPages(): number[] {
    const total = this.totalPages;
    const current = this.pageNumber;
    const maxVisible = 5;

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

  deleteMark(id: number): void {
    if (confirm('Are you sure you want to delete this mark?')) {
      this.markService.delete(id).subscribe({
        next: () => {
          this.toastService.success('Mark deleted successfully!', 'Success');
          this.loadMarks();
        },
        error: () => this.toastService.error('Failed to delete mark', 'Error')
      });
    }
  }
}