import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { StudentService } from '../../services/student';
import { CourseService } from '../../services/course';
import { TeacherService } from '../../services/teacher';
import { AuthService } from '../../services/auth';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-add-mark',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-mark.html',
  styleUrl: './add-mark.css'
})
export class AddMark implements OnInit {
  students: any[] = [];
  courses: any[] = [];
  teachers: any[] = [];

  studentId: number | null = null;
  courseId: number | null = null;
  teacherId: number | null = null;
  score: number | null = null;
  date: string = new Date().toISOString().split('T')[0];
  notes: string = '';

  errorMessage: string = '';
  successMessage: string = '';
  isTeacher: boolean = false;

  constructor(
    private markService: MarkService,
    private studentService: StudentService,
    private courseService: CourseService,
    private teacherService: TeacherService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.isTeacher = this.authService.isTeacher();
    this.loadData();
  }

  loadData(): void {
    if (this.isTeacher) {
      // ✅ Teacher — بس المواد والطلبة بتاعته
      this.teacherId = this.authService.getTeacherId();

      this.markService.getMyCourses().subscribe({
        next: (data: any[]) => { this.courses = data; this.cdr.detectChanges(); }
      });

      this.markService.getMyStudents().subscribe({
        next: (data: any[]) => { this.students = data; this.cdr.detectChanges(); }
      });
    } else {
      // ✅ Admin — كل حاجة
      this.studentService.getAll().subscribe({
        next: (data: any[]) => { this.students = data; this.cdr.detectChanges(); }
      });
      this.courseService.getAll().subscribe({
        next: (data: any[]) => { this.courses = data; this.cdr.detectChanges(); }
      });
      this.teacherService.getAll().subscribe({
        next: (data: any[]) => { this.teachers = data; this.cdr.detectChanges(); }
      });
    }
  }

  onSubmit(): void {
    if (!this.studentId || !this.courseId || this.score === null) {
      this.errorMessage = 'All fields are required';
      return;
    }

    if (!this.isTeacher && !this.teacherId) {
      this.errorMessage = 'Teacher is required';
      return;
    }

    if (this.score < 0 || this.score > 100) {
      this.errorMessage = 'Score must be between 0 and 100';
      return;
    }

    const newMark = {
      studentId: Number(this.studentId),
      courseId: Number(this.courseId),
      teacherId: this.isTeacher ? null : Number(this.teacherId),
      score: Number(this.score),
      date: this.date,
      notes: this.notes
    };

    this.markService.add(newMark).subscribe({
      next: () => {
        this.toastService.success('Mark added successfully!', 'Success');
        setTimeout(() => this.router.navigate(['/my-marks']), 1000);
      },
      error: (error: any) => {
        this.toastService.error(error.error?.message || 'Failed to add mark', 'Error');
        this.errorMessage = error.error?.message || 'Failed to add mark';
      }
    });
  }

  onCancel(): void {
    this.router.navigate([this.isTeacher ? '/my-marks' : '/marks']);
  }
}