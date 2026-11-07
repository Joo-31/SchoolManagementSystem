import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyHomework } from './my-homework';

describe('MyHomework', () => {
  let component: MyHomework;
  let fixture: ComponentFixture<MyHomework>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyHomework],
    }).compileComponents();

    fixture = TestBed.createComponent(MyHomework);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
