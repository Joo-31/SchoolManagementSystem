import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyMarks } from './my-marks';

describe('MyMarks', () => {
  let component: MyMarks;
  let fixture: ComponentFixture<MyMarks>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyMarks],
    }).compileComponents();

    fixture = TestBed.createComponent(MyMarks);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
