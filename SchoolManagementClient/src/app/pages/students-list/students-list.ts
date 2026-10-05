import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';   // ← ضيف ده
import { StudentService } from '../../services/student';
import { Student } from '../../models/student';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-students-list',
  standalone: true,
  imports: [CommonModule, RouterModule],   // ← ضيف RouterModule
  templateUrl: './students-list.html',
  styleUrl: './students-list.css'
})
export class StudentsList implements OnInit {
  students: Student[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.studentService.getAll().subscribe({
      next: (data: Student[]) => {
        this.students = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load students';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }
  deleteStudent(id: number): void {
  if (confirm('Are you sure you want to delete this student?')) {
    this.studentService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Student deleted successfully!', 'Success');
        this.loadStudents();
      },
      error: (error: any) => {
        this.toastService.error('Failed to delete student', 'Error');
        console.error(error);
      }
    });
  }
}
}