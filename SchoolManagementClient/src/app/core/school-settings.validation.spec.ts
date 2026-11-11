import { gradeBandsError, termsOverlap, validateLogoFile, validateSettingsImport } from './school-settings.validation';
import { DEFAULT_SCHOOL_SETTINGS } from './school-settings.service';

describe('school settings validation', () => {
  it('rejects grade bands with gaps and overlap', () => {
    expect(gradeBandsError([{ label: 'A', minPercent: 90 }, { label: 'D', minPercent: 50 }])).toContain('0%');
    expect(gradeBandsError([{ label: 'A', minPercent: 50 }, { label: 'B', minPercent: 50 }, { label: 'F', minPercent: 0 }])).toContain('overlap');
  });
  it('detects overlapping terms', () => {
    expect(termsOverlap([{ name: 'One', startDate: '2026-01-01', endDate: '2026-02-01' }, { name: 'Two', startDate: '2026-02-01', endDate: '2026-03-01' }])).toBe(true);
  });
  it('validates logo size and MIME type', () => {
    expect(validateLogoFile(new File(['x'], 'logo.png', { type: 'image/png' }))).toBeNull();
    expect(validateLogoFile(new File(['x'], 'logo.gif', { type: 'image/gif' }))).toContain('PNG');
    expect(validateLogoFile(new File([new Uint8Array(512 * 1024 + 1)], 'logo.png', { type: 'image/png' }))).toContain('512 KB');
  });
  it('validates import object shape', () => {
    expect(validateSettingsImport(DEFAULT_SCHOOL_SETTINGS)).toBe(true);
    expect(validateSettingsImport({ schoolName: 'x' })).toBe(false);
  });
});
