import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { StudentService } from '../../services/student';

@Component({
  selector: 'app-edit-student',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-student.html',
  styleUrl: './edit-student.css'
})
export class EditStudent implements OnInit {
  id: number = 0;
  firstName: string = '';
  lastName: string = '';
  birthYear: number | null = null;
  birthMonth: number | null = null;
  birthDay: number | null = null;
  gender: number = 0;
  classId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.studentService.getById(this.id).subscribe({
      next: (student: any) => {
        // ✅ الـ API دلوقتي بيرجع firstName, lastName, birthDate
        this.firstName = student.firstName || '';
        this.lastName = student.lastName || '';

        if (student.birthDate) {
          const date = new Date(student.birthDate);
          this.birthYear = date.getFullYear();
          this.birthMonth = date.getMonth() + 1;
          this.birthDay = date.getDate();
        }

        this.gender = student.gender === 'Male' ? 0 : 1;
        this.classId = student.classId;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load student';
        this.loading = false;
        this.cdr.detectChanges();
        console.error(error);
      }
    });
  }

  onSubmit(): void {
    if (!this.firstName || !this.lastName) {
      this.errorMessage = 'First name and last name are required';
      return;
    }

    if (!this.birthYear || !this.birthMonth || !this.birthDay) {
      this.errorMessage = 'Birth date is required';
      return;
    }

    if (!this.classId) {
      this.errorMessage = 'Class ID is required';
      return;
    }

    const birthDate = `${this.birthYear}-${String(this.birthMonth).padStart(2, '0')}-${String(this.birthDay).padStart(2, '0')}`;

    const updatedStudent = {
      id: this.id,
      firstName: this.firstName,
      lastName: this.lastName,
      birthDate: birthDate,
      gender: Number(this.gender),
      classId: Number(this.classId)
    };

    this.studentService.update(this.id, updatedStudent).subscribe({
      next: () => {
        this.successMessage = 'Student updated successfully!';
        setTimeout(() => this.router.navigate(['/students']), 1000);
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to update student';
        console.error(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/students']);
  }
}