import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeworkService } from '../../services/homework';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-student-homework',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './student-homework.html',
  styleUrl: './student-homework.css'
})
export class StudentHomework implements OnInit {
  homeworks: any[] = [];
  loading: boolean = true;

  constructor(private homeworkService: HomeworkService, private toastService: ToastService) { }

  ngOnInit(): void {
    this.homeworkService.getStudentHomework().subscribe({
      next: (data) => {
        this.homeworks = data;
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Failed to load homework', 'Error');
        this.loading = false;
      }
    });
  }
}