import { decodeJwt } from './session';
import { dt, gradeBand, letter, pctNorm, safeUrl } from './util';

describe('util', () => {
  it('adds a time part so System.Text.Json can parse plain dates', () => {
    expect(dt('2026-11-09')).toBe('2026-11-09T00:00:00');
    expect(dt('2026-11-09T10:00:00')).toBe('2026-11-09T10:00:00');
  });
  it('bands and letters scores', () => {
    expect(gradeBand(90)).toBe('a');
    expect(gradeBand(49)).toBe('d');
    expect(letter(92)).toBe('A+');
    expect(letter(40)).toBe('F');
  });
  it('only allows http(s) links', () => {
    expect(safeUrl('https://example.com')).toBe('https://example.com');
    expect(safeUrl('javascript:alert(1)')).toBeNull();
  });
  it('normalises fractions to percentages', () => {
    expect(pctNorm(0.8)).toBeCloseTo(80);
    expect(pctNorm(92)).toBe(92);
  });
});

describe('decodeJwt', () => {
  it('reads the payload of a token', () => {
    const b = (o: object) => btoa(JSON.stringify(o)).replace(/=/g, '');
    const token = `${b({ alg: 'none' })}.${b({ TeacherId: '7', role: 'Teacher' })}.sig`;
    expect(decodeJwt(token)).toEqual({ TeacherId: '7', role: 'Teacher' });
  });
  it('returns null for garbage', () => {
    expect(decodeJwt('nope')).toBeNull();
  });
});
