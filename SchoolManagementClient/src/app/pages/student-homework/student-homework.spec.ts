import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StudentHomework } from './student-homework';

describe('StudentHomework', () => {
  let component: StudentHomework;
  let fixture: ComponentFixture<StudentHomework>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentHomework],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentHomework);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
