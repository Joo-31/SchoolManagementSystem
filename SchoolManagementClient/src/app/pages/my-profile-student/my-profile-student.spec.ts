import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyProfileStudent } from './my-profile-student';

describe('MyProfileStudent', () => {
  let component: MyProfileStudent;
  let fixture: ComponentFixture<MyProfileStudent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyProfileStudent],
    }).compileComponents();

    fixture = TestBed.createComponent(MyProfileStudent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
