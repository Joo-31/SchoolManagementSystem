import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeacherService } from '../../services/teacher';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-profile.html',
  styleUrl: './my-profile.css'
})
export class MyProfile implements OnInit {
  teacher: any = null;
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private teacherService: TeacherService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.teacherService.getMyProfile().subscribe({
      next: (data: any) => {
        this.teacher = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load profile';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }
}