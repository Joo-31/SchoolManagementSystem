import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../../services/student';

@Component({
  selector: 'app-my-profile-student',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-profile-student.html',
  styleUrl: './my-profile-student.css'
})
export class MyProfileStudent implements OnInit {
  student: any = null;
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private studentService: StudentService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.studentService.getMyProfile().subscribe({
      next: (data: any) => {
        this.student = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load profile';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}