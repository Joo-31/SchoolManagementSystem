import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddClass } from './add-class';

describe('AddClass', () => {
  let component: AddClass;
  let fixture: ComponentFixture<AddClass>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddClass],
    }).compileComponents();

    fixture = TestBed.createComponent(AddClass);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
