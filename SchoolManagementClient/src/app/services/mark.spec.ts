import { TestBed } from '@angular/core/testing';
import { Mark } from './mark';

describe('Mark', () => {
  let service: Mark;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Mark);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
