import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HomeworkService } from '../../services/homework';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-my-homework',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-homework.html',
  styleUrl: './my-homework.css'
})
export class MyHomework implements OnInit {
  homeworks: any[] = [];
  loading: boolean = true;

  constructor(
    private homeworkService: HomeworkService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadHomeworks();
  }

  loadHomeworks(): void {
    this.loading = true;
    this.homeworkService.getTeacherHomework().subscribe({
      next: (data) => {
        this.homeworks = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load homework', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteHomework(id: number): void {
    if (confirm('Are you sure you want to delete this homework?')) {
      this.homeworkService.deleteHomework(id).subscribe({
        next: () => {
          this.toastService.success('Homework deleted successfully!', 'Success');
          this.loadHomeworks();
        },
        error: () => this.toastService.error('Failed to delete homework', 'Error')
      });
    }
  }
}