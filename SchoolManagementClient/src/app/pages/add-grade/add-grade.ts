import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { GradeService } from '../../services/grade';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-add-grade',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-grade.html',
  styleUrl: './add-grade.css'
})
export class AddGrade {
  name: string = '';
  level: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private gradeService: GradeService,
    private router: Router,
    private toastService: ToastService
  ) { }

  onSubmit(): void {
    if (!this.name || !this.level) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const newGrade = {
      name: this.name,
      level: Number(this.level)
    };

    this.gradeService.add(newGrade).subscribe({
     next: () => {
  this.toastService.success('Grade added successfully!', 'Success');
  setTimeout(() => this.router.navigate(['/grades']), 1000);
},
error: (error: any) => {
  this.toastService.error('Failed to add grade', 'Error');
}
    });
  }

  onCancel(): void {
    this.router.navigate(['/grades']);
  }
}