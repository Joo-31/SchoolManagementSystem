import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { GradeService } from '../../services/grade';

@Component({
  selector: 'app-edit-grade',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-grade.html',
  styleUrl: './edit-grade.css'
})
export class EditGrade implements OnInit {
  id: number = 0;
  name: string = '';
  level: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = true;

  constructor(
    private gradeService: GradeService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.gradeService.getById(this.id).subscribe({
      next: (data: any) => {
        this.name = data.name || '';
        this.level = data.level;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load grade';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (!this.name || !this.level) {
      this.errorMessage = 'All fields are required';
      return;
    }

    const updatedGrade = {
      id: this.id,
      name: this.name,
      level: Number(this.level)
    };

    this.gradeService.update(this.id, updatedGrade).subscribe({
      next: () => {
        this.successMessage = 'Grade updated successfully!';
        setTimeout(() => this.router.navigate(['/grades']), 1000);
      },
      error: () => {
        this.errorMessage = 'Failed to update grade';
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/grades']);
  }
}