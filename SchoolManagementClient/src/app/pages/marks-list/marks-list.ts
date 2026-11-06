import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ ضيفنا دي عشان البحث
import { RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-marks-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // ✅ ضيفنا FormsModule
  templateUrl: './marks-list.html',
  styleUrl: './marks-list.css'
})
export class MarksList implements OnInit {
  marks: any[] = [];
  allMarks: any[] = []; // ✅ عشان نحتفظ بالداتا الأصلية للفلترة
  loading: boolean = true;
  errorMessage: string = '';
  
  searchKeyword: string = ''; // ✅ متغير البحث

  constructor(
    private markService: MarkService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadMarks();
  }

  // ✅ دالة البحث في الفرونت إند
  filterMarks(): void {
    if (!this.searchKeyword || this.searchKeyword.trim() === '') {
      this.marks = this.allMarks;
      return;
    }
    
    const term = this.searchKeyword.toLowerCase();
    this.marks = this.allMarks.filter(m => 
      (m.studentName && m.studentName.toLowerCase().includes(term)) || 
      (m.courseName && m.courseName.toLowerCase().includes(term))
    );
  }

  clearSearch(): void {
    this.searchKeyword = '';
    this.marks = this.allMarks;
  }

  loadMarks(): void {
    this.loading = true;
    this.markService.getAll().subscribe({
      next: (data: any[]) => {
        this.marks = data;
        this.allMarks = data; // ✅ بنحفظ نسخة للفلترة
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