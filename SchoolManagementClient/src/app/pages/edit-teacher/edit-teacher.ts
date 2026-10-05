import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { TeacherService } from '../../services/teacher';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-edit-teacher',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-teacher.html',
  styleUrl: './edit-teacher.css'
})
export class EditTeacher implements OnInit {
  id: number = 0;
  name: string = '';
  email: string = '';
  phone: string = '';
  specialization: string = '';
  hireYear: number | null = null;
  hireMonth: number | null = null;
  hireDay: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private teacherService: TeacherService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
     private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.teacherService.getById(this.id).subscribe({
      next: (teacher: any) => {
        this.name = teacher.name || '';
        this.email = teacher.email || '';
        this.phone = teacher.phone || '';
        this.specialization = teacher.specialization || '';

        if (teacher.hireDate) {
          const date = new Date(teacher.hireDate);
          this.hireYear = date.getFullYear();
          this.hireMonth = date.getMonth() + 1;
          this.hireDay = date.getDate();
        }

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
  this.toastService.error('Failed to load teacher', 'Error');
  this.loading = false;
  this.cdr.detectChanges();
}
    });
  }

  onSubmit(): void {
    if (!this.name || !this.email || !this.phone || !this.specialization) {
      this.errorMessage = 'All fields are required';
      return;
    }

    if (!this.hireYear || !this.hireMonth || !this.hireDay) {
      this.errorMessage = 'Hire date is required';
      return;
    }

    const hireDate = `${this.hireYear}-${String(this.hireMonth).padStart(2, '0')}-${String(this.hireDay).padStart(2, '0')}`;

    const updatedTeacher = {
      id: this.id,
      name: this.name,
      email: this.email,
      phone: this.phone,
      specialization: this.specialization,
      hireDate: hireDate
    };

    this.teacherService.update(this.id, updatedTeacher).subscribe({
      next: () => {
  this.toastService.success('Teacher updated successfully!', 'Success');
  setTimeout(() => this.router.navigate(['/teachers']), 1000);
},
error: (error: any) => {
  this.toastService.error('Failed to update teacher', 'Error');
  console.error(error);
}
    });
  }

  onCancel(): void {
    this.router.navigate(['/teachers']);
  }
}