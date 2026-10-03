import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GradesList } from './grades-list';

describe('GradesList', () => {
  let component: GradesList;
  let fixture: ComponentFixture<GradesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GradesList],
    }).compileComponents();

    fixture = TestBed.createComponent(GradesList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
