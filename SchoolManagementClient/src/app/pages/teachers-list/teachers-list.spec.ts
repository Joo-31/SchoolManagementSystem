import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TeachersList } from './teachers-list';

describe('TeachersList', () => {
  let component: TeachersList;
  let fixture: ComponentFixture<TeachersList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeachersList],
    }).compileComponents();

    fixture = TestBed.createComponent(TeachersList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
