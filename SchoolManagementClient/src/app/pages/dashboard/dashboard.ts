import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentService } from '../../services/student';
import { TeacherService } from '../../services/teacher';
import { CourseService } from '../../services/course';
import { ClassService } from '../../services/class';
import { GradeService } from '../../services/grade';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  studentCount: number = 0;
  teacherCount: number = 0;
  courseCount: number = 0;
  classCount: number = 0;
  gradeCount: number = 0;
  loading: boolean = true;

  constructor(
    private studentService: StudentService,
    private teacherService: TeacherService,
    private courseService: CourseService,
    private classService: ClassService,
    private gradeService: GradeService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCounts();
  }

  loadCounts(): void {
    this.studentService.getCount().subscribe({
      next: (res: any) => { this.studentCount = res.count; this.checkLoading(); }
    });
    this.teacherService.getCount().subscribe({
      next: (res: any) => { this.teacherCount = res.count; this.checkLoading(); }
    });
    this.courseService.getCount().subscribe({
      next: (res: any) => { this.courseCount = res.count; this.checkLoading(); }
    });
    this.classService.getCount().subscribe({
      next: (res: any) => { this.classCount = res.count; this.checkLoading(); }
    });
    this.gradeService.getCount().subscribe({
      next: (res: any) => { this.gradeCount = res.count; this.checkLoading(); }
    });
  }

  private checkLoading(): void {
    this.loading = false;
    this.cdr.detectChanges();
  }
}