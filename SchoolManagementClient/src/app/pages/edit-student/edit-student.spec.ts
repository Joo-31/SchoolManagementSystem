import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditStudent } from './edit-student';

describe('EditStudent', () => {
  let component: EditStudent;
  let fixture: ComponentFixture<EditStudent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditStudent],
    }).compileComponents();

    fixture = TestBed.createComponent(EditStudent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
