import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ClassService } from '../../services/class';
import { ToastService } from '../../services/toast';
@Component({
  selector: 'app-edit-class',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-class.html',
  styleUrl: './edit-class.css'
})
export class EditClass implements OnInit {
  id: number = 0;
  name: string = '';
  gradeId: number | null = null;
  classTeacherId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private classService: ClassService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
      private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.classService.getById(this.id).subscribe({
      next: (data: any) => {
        this.name = data.name || '';
        this.gradeId = data.gradeId;
        this.classTeacherId = data.classTeacherId;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.toastService.error('Failed to load class', 'Error');
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (!this.name || !this.gradeId || !this.classTeacherId) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const updatedClass = {
      id: this.id,
      name: this.name,
      gradeId: Number(this.gradeId),
      classTeacherId: Number(this.classTeacherId)
    };

    this.classService.update(this.id, updatedClass).subscribe({
      next: () => {
  this.toastService.success('Class updated successfully!', 'Success');
  setTimeout(() => this.router.navigate(['/classes']), 1000);
},
error: (error: any) => {
  this.toastService.error('Failed to update class', 'Error');
}
    });
  }

  onCancel(): void {
    this.router.navigate(['/classes']);
  }
}