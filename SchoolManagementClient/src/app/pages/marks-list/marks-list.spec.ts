import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MarksList } from './marks-list';

describe('MarksList', () => {
  let component: MarksList;
  let fixture: ComponentFixture<MarksList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarksList],
    }).compileComponents();

    fixture = TestBed.createComponent(MarksList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
