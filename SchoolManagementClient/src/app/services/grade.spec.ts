import { TestBed } from '@angular/core/testing';
import { Grade } from './grade';

describe('Grade', () => {
  let service: Grade;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Grade);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
