import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditGrade } from './edit-grade';

describe('EditGrade', () => {
  let component: EditGrade;
  let fixture: ComponentFixture<EditGrade>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditGrade],
    }).compileComponents();

    fixture = TestBed.createComponent(EditGrade);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
