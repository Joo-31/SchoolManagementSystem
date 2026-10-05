import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';

@Component({
  selector: 'app-my-classes',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-classes.html',
  styleUrl: './my-classes.css'
})
export class MyClasses implements OnInit {
  classes: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private teacherService: TeacherService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadClasses();
  }

  loadClasses(): void {
    this.teacherService.getMyClasses().subscribe({
      next: (data: any[]) => {
        this.classes = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load your classes';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }
}