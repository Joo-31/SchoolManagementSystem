import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AttendanceService } from '../../services/attendance';

@Component({
  selector: 'app-add-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-attendance.html',
  styleUrl: './add-attendance.css'
})
export class AddAttendance {
  studentId: number | null = null;
  classId: number | null = null;
  dateYear: number | null = null;
  dateMonth: number | null = null;
  dateDay: number | null = null;
  isPresent: boolean = true;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private attendanceService: AttendanceService,
    private router: Router
  ) { }

  onSubmit(): void {
    if (!this.studentId || !this.classId || !this.dateYear || !this.dateMonth || !this.dateDay) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const date = `${this.dateYear}-${String(this.dateMonth).padStart(2, '0')}-${String(this.dateDay).padStart(2, '0')}`;

    const newAttendance = {
      studentId: Number(this.studentId),
      classId: Number(this.classId),
      date: date,
      isPresent: this.isPresent
    };

    this.attendanceService.add(newAttendance).subscribe({
      next: () => {
        this.successMessage = 'Attendance added successfully!';
        setTimeout(() => this.router.navigate(['/attendances']), 1000);
      },
      error: () => {
        this.errorMessage = 'Failed to add attendance';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/attendances']);
  }
}