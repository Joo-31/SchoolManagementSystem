import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddNotification } from './add-notification';

describe('AddNotification', () => {
  let component: AddNotification;
  let fixture: ComponentFixture<AddNotification>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNotification],
    }).compileComponents();

    fixture = TestBed.createComponent(AddNotification);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
