import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AttendanceService } from '../../services/attendance';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-edit-attendance',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-attendance.html',
  styleUrl: './edit-attendance.css'
})
export class EditAttendance implements OnInit {
  id: number = 0;
  studentId: number | null = null;
  classId: number | null = null;
  dateYear: number | null = null;
  dateMonth: number | null = null;
  dateDay: number | null = null;
  isPresent: boolean = true;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private attendanceService: AttendanceService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.attendanceService.getById(this.id).subscribe({
      next: (data: any) => {
        this.studentId = data.studentId;
        this.classId = data.classId;
        
        if (data.date) {
          const d = new Date(data.date);
          this.dateYear = d.getFullYear();
          this.dateMonth = d.getMonth() + 1;
          this.dateDay = d.getDate();
        }

        this.isPresent = data.isPresent;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load attendance', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (!this.studentId || !this.classId || !this.dateYear || !this.dateMonth || !this.dateDay) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const date = `${this.dateYear}-${String(this.dateMonth).padStart(2, '0')}-${String(this.dateDay).padStart(2, '0')}`;

    const updatedAttendance = {
      id: this.id,
      studentId: Number(this.studentId),
      classId: Number(this.classId),
      date: date,
      isPresent: this.isPresent
    };

    this.attendanceService.update(this.id, updatedAttendance).subscribe({
      next: () => {
  this.toastService.success('Attendance updated successfully!', 'Success');
  setTimeout(() => this.router.navigate(['/attendances']), 1000);
},
error: (error: any) => {
  this.toastService.error('Failed to update attendance', 'Error');
}
    });
  }

  onCancel(): void {
    this.router.navigate(['/attendances']);
  }
}