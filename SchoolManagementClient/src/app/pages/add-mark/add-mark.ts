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
    // ✅ Validation واضحة
    if (!this.studentId || this.studentId <= 0) {
      this.errorMessage = 'Please select a student';
      this.toastService.warning('Please select a student', 'Warning');
      return;
    }

    if (!this.courseId || this.courseId <= 0) {
      this.errorMessage = 'Please select a course';
      this.toastService.warning('Please select a course', 'Warning');
      return;
    }

    if (this.score === null || this.score < 0 || this.score > 100) {
      this.errorMessage = 'Score must be between 0 and 100';
      this.toastService.warning('Score must be between 0 and 100', 'Warning');
      return;
    }

    if (!this.isTeacher && (!this.teacherId || this.teacherId <= 0)) {
      this.errorMessage = 'Please select a teacher';
      this.toastService.warning('Please select a teacher', 'Warning');
      return;
    }

    const newMark = {
      studentId: Number(this.studentId),
      courseId: Number(this.courseId),
      teacherId: this.isTeacher ? 0 : Number(this.teacherId),   // ← 0 بدل null
      score: Number(this.score),
      date: this.date,
      notes: this.notes || ''
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