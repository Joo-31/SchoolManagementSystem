import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClassService } from '../../services/class';

@Component({
  selector: 'app-classes-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './classes-list.html',
  styleUrl: './classes-list.css'
})
export class ClassesList implements OnInit {
  classes: any[] = [];
  loading: boolean = true;
  errorMessage: string = '';

  constructor(
    private classService: ClassService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadClasses();
  }

  loadClasses(): void {
    this.classService.getAll().subscribe({
      next: (data: any[]) => {
        this.classes = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load classes';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteClass(id: number): void {
    if (confirm('Are you sure you want to delete this class?')) {
      this.classService.delete(id).subscribe({
        next: () => this.loadClasses(),
        error: () => {
          this.errorMessage = 'Failed to delete class';
        }
      });
    }
  }
}