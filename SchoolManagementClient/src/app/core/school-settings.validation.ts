import { GradeBand, SchoolSettings, SchoolTerm } from './models';

export function gradeBandsError(bands: GradeBand[]): string | null {
  if (!bands.length || bands.some((b) => !b.label.trim() || b.minPercent < 0 || b.minPercent > 100)) return 'Bands must have a label and minimum from 0 to 100.';
  const sorted = [...bands].sort((a, b) => b.minPercent - a.minPercent);
  if (new Set(sorted.map((b) => b.minPercent)).size !== sorted.length) return 'Band minimums cannot overlap.';
  if (sorted[sorted.length - 1].minPercent !== 0) return 'Bands must include a 0% minimum.';
  return null;
}
export function termsOverlap(terms: SchoolTerm[]): boolean {
  const dated = terms.filter((t) => t.startDate && t.endDate).slice().sort((a, b) => a.startDate.localeCompare(b.startDate));
  return dated.some((term, i) => term.startDate > term.endDate || (i > 0 && dated[i - 1].endDate >= term.startDate));
}
export function validateLogoFile(file: File): string | null {
  if (file.size > 512 * 1024) return 'Logo must be 512 KB or smaller.';
  if (!['image/png', 'image/jpeg', 'image/svg+xml', 'image/webp'].includes(file.type)) return 'Choose a PNG, JPG, SVG, or WEBP image.';
  return null;
}
export function validateSettingsImport(value: unknown): value is SchoolSettings {
  if (!value || typeof value !== 'object') return false;
  const v = value as Partial<SchoolSettings>;
  return typeof v.schoolName === 'string' && v.schoolName.length >= 2 && v.schoolName.length <= 80 && typeof v.shortName === 'string' && v.shortName.length <= 20 &&
    (v.logo === null || typeof v.logo === 'string') && Array.isArray(v.terms) && Array.isArray(v.gradeBands) && !termsOverlap(v.terms) && !gradeBandsError(v.gradeBands) &&
    typeof v.passMark === 'number' && v.passMark >= 0 && v.passMark <= 100 && (v.apiMode === 'demo' || v.apiMode === 'live');
}
