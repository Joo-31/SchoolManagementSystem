import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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

  constructor(
    private homeworkService: HomeworkService, 
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.homeworkService.getStudentHomework().subscribe({
      next: (data) => {
        this.homeworks = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        // ✅ لو حصل خطأ أو مفيش داتا، بنقفل الـ Loading عشان تظهر الرسالة
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}