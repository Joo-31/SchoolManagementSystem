import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AttendancesList } from './attendances-list';

describe('AttendancesList', () => {
  let component: AttendancesList;
  let fixture: ComponentFixture<AttendancesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AttendancesList],
    }).compileComponents();

    fixture = TestBed.createComponent(AttendancesList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
