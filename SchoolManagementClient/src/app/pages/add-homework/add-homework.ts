import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HomeworkService } from '../../services/homework';
import { MarkService } from '../../services/mark'; // عشان نجيب كورسات المدرس
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-add-homework',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-homework.html',
  styleUrl: './add-homework.css'
})
export class AddHomework implements OnInit {
  title: string = '';
  description: string = '';
  link: string = '';
  courseId: number | null = null;
  
  courses: any[] = []; // كورسات المدرس

  constructor(
    private homeworkService: HomeworkService,
    private markService: MarkService, // بنستخدمه عشان يجيب الكورسات
    private toastService: ToastService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // بنجيب الكورسات اللي المدرس بيدرسها
    this.markService.getMyCourses().subscribe(data => {
      this.courses = data;
    });
  }

  onSubmit(): void {
    if (!this.title || !this.link || !this.courseId) {
      this.toastService.error('Title, Link, and Course are required', 'Error');
      return;
    }

    const payload = {
      title: this.title,
      description: this.description,
      link: this.link,
      courseId: this.courseId
    };

    this.homeworkService.addHomework(payload).subscribe({
      next: () => {
        this.toastService.success('Homework added successfully!', 'Success');
        this.router.navigate(['/my-homework']);
      },
      error: () => this.toastService.error('Failed to add homework', 'Error')
    });
  }

  onCancel(): void {
    this.router.navigate(['/my-homework']);
  }
}