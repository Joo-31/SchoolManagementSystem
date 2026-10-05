import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyClasses } from './my-classes';

describe('MyClasses', () => {
  let component: MyClasses;
  let fixture: ComponentFixture<MyClasses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyClasses],
    }).compileComponents();

    fixture = TestBed.createComponent(MyClasses);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
