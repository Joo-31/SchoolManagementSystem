import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { ClassService } from '../../services/class';

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
    private cdr: ChangeDetectorRef
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
        this.errorMessage = 'Failed to load class';
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
        this.successMessage = 'Class updated successfully!';
        setTimeout(() => this.router.navigate(['/classes']), 1000);
      },
      error: () => {
        this.errorMessage = 'Failed to update class';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/classes']);
  }
}