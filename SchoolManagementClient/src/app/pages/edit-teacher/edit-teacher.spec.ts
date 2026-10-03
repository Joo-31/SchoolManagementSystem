import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditTeacher } from './edit-teacher';

describe('EditTeacher', () => {
  let component: EditTeacher;
  let fixture: ComponentFixture<EditTeacher>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditTeacher],
    }).compileComponents();

    fixture = TestBed.createComponent(EditTeacher);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
