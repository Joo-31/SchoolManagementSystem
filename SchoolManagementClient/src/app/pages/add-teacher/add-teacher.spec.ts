import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddTeacher } from './add-teacher';

describe('AddTeacher', () => {
  let component: AddTeacher;
  let fixture: ComponentFixture<AddTeacher>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddTeacher],
    }).compileComponents();

    fixture = TestBed.createComponent(AddTeacher);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
