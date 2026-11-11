import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { store } from './session';

export type DataMode = 'live' | 'demo';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  readonly defaultUrl = environment.apiUrl;
  readonly mode = signal<DataMode>(store.get('mode') === 'demo' ? 'demo' : 'live');
  readonly apiUrl = signal<string>((store.get('url') || environment.apiUrl).replace(/\/+$/, ''));
  readonly theme = signal<'light' | 'dark'>(document.documentElement.dataset['theme'] === 'dark' ? 'dark' : 'light');

  setMode(m: DataMode): void { this.mode.set(m); store.set('mode', m); }

  setApiUrl(url: string): void {
    const v = url.trim().replace(/\/+$/, '');
    this.apiUrl.set(v || environment.apiUrl);
    store.set('url', !v || v === environment.apiUrl ? null : v);
  }

  toggleTheme(): void {
    const t = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(t);
    document.documentElement.dataset['theme'] = t;
    store.set('theme', t);
  }
}
