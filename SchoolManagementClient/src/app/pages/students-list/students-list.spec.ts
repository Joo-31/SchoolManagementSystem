import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StudentsList } from './students-list';

describe('StudentsList', () => {
  let component: StudentsList;
  let fixture: ComponentFixture<StudentsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StudentsList],
    }).compileComponents();

    fixture = TestBed.createComponent(StudentsList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
