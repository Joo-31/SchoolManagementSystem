import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClassesList } from './classes-list';

describe('ClassesList', () => {
  let component: ClassesList;
  let fixture: ComponentFixture<ClassesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassesList],
    }).compileComponents();

    fixture = TestBed.createComponent(ClassesList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
