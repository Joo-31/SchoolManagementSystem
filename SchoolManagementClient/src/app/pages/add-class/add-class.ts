import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ClassService } from '../../services/class';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-add-class',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-class.html',
  styleUrl: './add-class.css'
})
export class AddClass {
  name: string = '';
  gradeId: number | null = null;
  classTeacherId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private classService: ClassService,
    private router: Router,
    private toastService: ToastService
  ) { }

  onSubmit(): void {
    if (!this.name || !this.gradeId || !this.classTeacherId) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const newClass = {
      name: this.name,
      gradeId: Number(this.gradeId),
      classTeacherId: Number(this.classTeacherId)
    };

    this.classService.add(newClass).subscribe({
     next: () => {
  this.toastService.success('Class added successfully!', 'Success');
  setTimeout(() => this.router.navigate(['/classes']), 1000);
},
error: (error: any) => {
  this.toastService.error('Failed to add class', 'Error');
  console.error(error);
}
    });
  }

  onCancel(): void {
    this.router.navigate(['/classes']);
  }
}