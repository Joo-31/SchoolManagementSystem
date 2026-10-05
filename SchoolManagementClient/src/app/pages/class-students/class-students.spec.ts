import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClassStudents } from './class-students';

describe('ClassStudents', () => {
  let component: ClassStudents;
  let fixture: ComponentFixture<ClassStudents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassStudents],
    }).compileComponents();

    fixture = TestBed.createComponent(ClassStudents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
