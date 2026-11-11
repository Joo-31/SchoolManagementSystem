import { Injectable, inject, signal } from '@angular/core';
import { ApiService } from './api.service';
import { ApiError, SchoolSettings } from './models';
import { SettingsService } from './settings.service';

export const DEFAULT_SCHOOL_SETTINGS: SchoolSettings = {
  schoolName: 'Chalkline', shortName: 'Chalkline', logo: null, tagline: '', address: '', phone: '', email: '', website: '',
  academicYear: new Date().getFullYear() + '/' + (new Date().getFullYear() + 1), terms: [
    { name: 'Term 1', startDate: '', endDate: '' }, { name: 'Term 2', startDate: '', endDate: '' },
  ], weekStartsOn: 0, passMark: 50,
  gradeBands: [{ label: 'A', minPercent: 90 }, { label: 'B', minPercent: 80 }, { label: 'C', minPercent: 70 }, { label: 'D', minPercent: 50 }, { label: 'F', minPercent: 0 }],
  lateThresholdMinutes: 10, countExcusedAbsences: false, language: 'English', dateFormat: 'd MMM y', timezone: 'Africa/Cairo',
  defaultTheme: 'system', accentColor: '#3D4DF5', apiMode: 'live', apiUrl: '',
};
const KEY = 'cl.schoolSettings';

@Injectable({ providedIn: 'root' })
export class SchoolSettingsService {
  private api = inject(ApiService);
  private connection = inject(SettingsService);
  private state = signal<SchoolSettings>(DEFAULT_SCHOOL_SETTINGS);
  readonly settings = this.state.asReadonly();
  readonly localOnly = signal(false);
  private loaded = false;

  async load(): Promise<void> {
    if (this.loaded) return;
    try {
      const value = await this.api.get<SchoolSettings>('/settings/school');
      if (isSchoolSettings(value)) this.apply(value);
    } catch (error) {
      if (this.connection.mode() === 'live' && error instanceof ApiError && error.status === 404) this.localOnly.set(true);
      const local = safeRead();
      if (local) this.apply(local);
    }
    this.loaded = true;
  }

  async save(value: SchoolSettings): Promise<void> {
    this.apply(value);
    try {
      const response = await this.api.put<SchoolSettings>('/settings/school', value);
      if (isSchoolSettings(response)) this.apply(response);
    } catch (error) {
      if (this.connection.mode() === 'live' && error instanceof ApiError && error.status === 404) this.localOnly.set(true);
      if (!(this.connection.mode() === 'live' && error instanceof ApiError && error.status === 404)) throw error;
      safeWrite(value);
    }
  }

  replace(value: SchoolSettings): void { this.apply(value); safeWrite(value); }
  private apply(value: SchoolSettings): void {
    this.state.set(value);
    this.connection.setMode(value.apiMode);
    this.connection.setApiUrl(value.apiUrl);
    document.title = `${value.schoolName || 'Chalkline'} — School Management`;
    document.documentElement.lang = value.language === 'Arabic' ? 'ar' : 'en';
    document.documentElement.dir = value.language === 'Arabic' ? 'rtl' : 'ltr';
    if (value.accentColor) document.documentElement.style.setProperty('--brand', value.accentColor);
    if (value.defaultTheme !== 'system') {
      document.documentElement.dataset['theme'] = value.defaultTheme;
      this.connection.theme.set(value.defaultTheme);
    }
    if (value.logo) {
      let icon = document.querySelector<HTMLLinkElement>('link[rel="icon"]');
      if (!icon) { icon = document.createElement('link'); icon.rel = 'icon'; document.head.append(icon); }
      icon.href = value.logo;
    }
    safeWrite(value);
  }
}

export function isSchoolSettings(value: unknown): value is SchoolSettings {
  if (!value || typeof value !== 'object') return false;
  const v = value as Partial<SchoolSettings>;
  return typeof v.schoolName === 'string' && typeof v.shortName === 'string' && Array.isArray(v.terms) && Array.isArray(v.gradeBands) &&
    typeof v.passMark === 'number' && (v.apiMode === 'demo' || v.apiMode === 'live');
}
function safeRead(): SchoolSettings | null { try { const raw = localStorage.getItem(KEY); if (!raw) return null; const v: unknown = JSON.parse(raw); return isSchoolSettings(v) ? v : null; } catch { return null; } }
function safeWrite(value: SchoolSettings): void { try { localStorage.setItem(KEY, JSON.stringify(value)); } catch { /* storage can be unavailable or full */ } }
