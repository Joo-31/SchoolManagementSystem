import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditClass } from './edit-class';

describe('EditClass', () => {
  let component: EditClass;
  let fixture: ComponentFixture<EditClass>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditClass],
    }).compileComponents();

    fixture = TestBed.createComponent(EditClass);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
