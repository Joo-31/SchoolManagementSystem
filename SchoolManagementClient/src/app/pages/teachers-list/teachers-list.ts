import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';
import { Teacher } from '../../models/teacher';

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
    private cdr: ChangeDetectorRef
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
          this.loadTeachers();
        },
        error: (error: any) => {
          this.errorMessage = 'Failed to delete teacher';
          console.error(error);
        }
      });
    }
  }
}