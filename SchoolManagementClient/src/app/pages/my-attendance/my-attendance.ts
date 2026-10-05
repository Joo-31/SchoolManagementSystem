import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../../services/student';

@Component({
  selector: 'app-my-attendance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-attendance.html',
  styleUrl: './my-attendance.css'
})
export class MyAttendance implements OnInit {
  attendances: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadAttendances();
  }

  loadAttendances(): void {
    this.studentService.getMyAttendances().subscribe({
      next: (data: any[]) => {
        this.attendances = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load attendances';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}