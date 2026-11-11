export const dt = (v: string): string => (v && v.length === 10 ? v + 'T00:00:00' : v);
export const day10 = (v: string | null | undefined): string => (v ? String(v).slice(0, 10) : '');
export const safeUrl = (u: string | null | undefined): string | null => (/^https?:\/\//i.test(u || '') ? (u as string) : null);
export const pctNorm = (v: number): number => { v = +v || 0; return v > 0 && v <= 1 ? v * 100 : v; };
export const avg = (a: number[]): number => (a.length ? a.reduce((x, y) => x + y, 0) / a.length : 0);
export const round1 = (n: number): number => Math.round(n * 10) / 10;
export const cap = (s: string): string => s.charAt(0).toUpperCase() + s.slice(1);

export function isoDay(d: Date = new Date()): string {
  const x = new Date(d);
  x.setMinutes(x.getMinutes() - x.getTimezoneOffset());
  return x.toISOString().slice(0, 10);
}
export const gradeBand = (s: number): 'a' | 'b' | 'c' | 'd' => (s >= 85 ? 'a' : s >= 70 ? 'b' : s >= 50 ? 'c' : 'd');
export const letter = (s: number): string => (s >= 90 ? 'A+' : s >= 85 ? 'A' : s >= 75 ? 'B' : s >= 65 ? 'C' : s >= 50 ? 'D' : 'F');
export const bandVar = (s: number): string => ({ a: 'brand', b: 'ok', c: 'warn', d: 'bad' })[gradeBand(s)];
export const initials = (n: string): string => (n || '?').split(/\s+/).filter(Boolean).slice(0, 2).map((x) => x[0]).join('').toUpperCase();

export const PALETTE = ['#3D4DF5', '#14A38B', '#F2A93B', '#E5484D', '#8B5CF6', '#0EA5E9', '#EC4899', '#84CC16'];
