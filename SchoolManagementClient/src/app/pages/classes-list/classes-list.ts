import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClassService } from '../../services/class';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-classes-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './classes-list.html',
  styleUrl: './classes-list.css'
})
export class ClassesList implements OnInit {
  classes: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPrevious: boolean = false;
  hasNext: boolean = false;

  constructor(
    private classService: ClassService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void { this.loadClasses(); }

  loadClasses(): void {
    this.loading = true;
    this.classService.getPaged(this.pageNumber, this.pageSize).subscribe({
      next: (response: any) => {
        this.classes = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.hasPrevious = response.hasPrevious;
        this.hasNext = response.hasNext;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load classes', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void { if (this.hasNext) { this.pageNumber++; this.loadClasses(); } }
  previousPage(): void { if (this.hasPrevious) { this.pageNumber--; this.loadClasses(); } }
  goToPage(page: number): void { if (page >= 1 && page <= this.totalPages) { this.pageNumber = page; this.loadClasses(); } }
  getPages(): number[] { return Array.from({ length: this.totalPages }, (_, i) => i + 1); }

  deleteClass(id: number): void {
    if (confirm('Are you sure you want to delete this class?')) {
      this.classService.delete(id).subscribe({
        next: () => { this.toastService.success('Class deleted successfully!', 'Success'); this.loadClasses(); },
        error: () => { this.toastService.error('Failed to delete class', 'Error'); }
      });
    }
  }
}