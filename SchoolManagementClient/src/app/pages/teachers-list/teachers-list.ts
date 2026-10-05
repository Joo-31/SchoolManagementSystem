import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';
import { Teacher } from '../../models/teacher';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-teachers-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './teachers-list.html',
  styleUrl: './teachers-list.css'
})
export class TeachersList implements OnInit {
  teachers: Teacher[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private teacherService: TeacherService,
    private cdr: ChangeDetectorRef,
     private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadTeachers();
  }

  loadTeachers(): void {
    this.teacherService.getAll().subscribe({
      next: (data: Teacher[]) => {
        this.teachers = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load teachers';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }

  deleteTeacher(id: number): void {
  if (confirm('Are you sure you want to delete this teacher?')) {
    this.teacherService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Teacher deleted successfully!', 'Success');
        this.loadTeachers();
      },
      error: (error: any) => {
        this.toastService.error('Failed to delete teacher', 'Error');
        console.error(error);
      }
    });
  }
}
}