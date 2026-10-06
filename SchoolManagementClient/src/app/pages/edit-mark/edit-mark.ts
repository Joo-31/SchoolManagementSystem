import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MarkService } from '../../services/mark';
import { StudentService } from '../../services/student';
import { CourseService } from '../../services/course';
import { TeacherService } from '../../services/teacher';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-edit-mark',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-mark.html',
  styleUrl: './edit-mark.css'
})
export class EditMark implements OnInit {
  id: number = 0;
  students: any[] = [];
  courses: any[] = [];
  teachers: any[] = [];

  studentId: number | null = null;
  courseId: number | null = null;
  teacherId: number | null = null;
  score: number | null = null;
  date: string = '';
  notes: string = '';

  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private markService: MarkService,
    private studentService: StudentService,
    private courseService: CourseService,
    private teacherService: TeacherService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData();
    this.loadMark();
  }

  loadData(): void {
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

  loadMark(): void {
    this.markService.getById(this.id).subscribe({
      next: (mark: any) => {
        this.studentId = mark.studentId;
        this.courseId = mark.courseId;
        this.teacherId = mark.teacherId;
        this.score = mark.score;
        this.date = mark.date.split('T')[0];
        this.notes = mark.notes || '';
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.toastService.error('Failed to load mark', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (!this.studentId || !this.courseId || !this.teacherId || this.score === null) {
      this.errorMessage = 'All fields are required';
      return;
    }

    if (this.score < 0 || this.score > 100) {
      this.errorMessage = 'Score must be between 0 and 100';
      return;
    }

    const updatedMark = {
      studentId: Number(this.studentId),
      courseId: Number(this.courseId),
      teacherId: Number(this.teacherId),
      score: Number(this.score),
      date: this.date,
      notes: this.notes
    };

    this.markService.update(this.id, updatedMark).subscribe({
      next: () => {
        this.toastService.success('Mark updated successfully!', 'Success');
        setTimeout(() => this.router.navigate(['/marks']), 1000);
      },
      error: (error: any) => {
        this.toastService.error('Failed to update mark', 'Error');
        this.errorMessage = 'Failed to update mark';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/marks']);
  }
}