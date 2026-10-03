import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditAttendance } from './edit-attendance';

describe('EditAttendance', () => {
  let component: EditAttendance;
  let fixture: ComponentFixture<EditAttendance>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditAttendance],
    }).compileComponents();

    fixture = TestBed.createComponent(EditAttendance);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
