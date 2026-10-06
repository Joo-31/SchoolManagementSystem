import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditMark } from './edit-mark';

describe('EditMark', () => {
  let component: EditMark;
  let fixture: ComponentFixture<EditMark>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditMark],
    }).compileComponents();

    fixture = TestBed.createComponent(EditMark);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
