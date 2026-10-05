import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';
import { AttendanceService } from '../../services/attendance';

@Component({
  selector: 'app-class-students',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './class-students.html',
  styleUrl: './class-students.css'
})
export class ClassStudents implements OnInit {
  classId: number = 0;
  students: any[] = [];
  loading: boolean = true;
  saving: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  
  // ⚠️ عشان الـ Attendance
  today: string = new Date().toISOString().split('T')[0];
  attendanceDate: string = this.today;
  attendanceRecords: { studentId: number, isPresent: boolean }[] = [];

  constructor(
    private teacherService: TeacherService,
    private attendanceService: AttendanceService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.classId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadStudents();
  }

  loadStudents(): void {
    this.teacherService.getStudentsByClass(this.classId).subscribe({
      next: (data: any[]) => {
        this.students = data;
        this.loadAttendanceForDate();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load students';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }

  loadAttendanceForDate(): void {
    const requestedDate = this.attendanceDate;
    this.errorMessage = '';
    this.loading = true;
    this.attendanceService.getByDate(requestedDate).subscribe({
      next: (attendances) => {
        if (requestedDate !== this.attendanceDate) {
          return;
        }

        const attendanceByStudent = new Map(
          attendances
            .filter(attendance => attendance.classId === this.classId)
            .map(attendance => [attendance.studentId, attendance.isPresent])
        );

        this.attendanceRecords = this.students.map(student => ({
          studentId: student.id,
          isPresent: attendanceByStudent.get(student.id) ?? true
        }));
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        if (requestedDate === this.attendanceDate) {
          this.errorMessage = 'Failed to load attendance';
          this.loading = false;
          this.cdr.detectChanges();
        }
        console.error(error);
      }
    });
  }

  saveAttendance(): void {
  this.saving = true;
  this.attendanceService.takeAttendance({
    classId: this.classId,
    date: this.attendanceDate,
    records: this.attendanceRecords
  }).subscribe({
    next: () => {
      this.saving = false;
      // ✅ Navigate فوراً
      this.router.navigate(['/my-classes']);
    },
    error: (error: any) => {
      this.errorMessage = 'Failed to save attendance';
      this.saving = false;
      console.error(error);
    }
  });
}

  onCancel(): void {
    this.router.navigate(['/my-classes']);
  }
}