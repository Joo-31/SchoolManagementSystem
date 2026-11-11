import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Session } from './models';
import { readSession, store } from './session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  readonly session = signal<Session | null>(readSession());
  readonly role = computed(() => this.session()?.role ?? null);

  home(): string {
    const r = this.session()?.role;
    return r === 'Admin' ? '/dashboard' : r === 'Teacher' ? '/teacher' : '/student';
  }

  async login(username: string, password: string): Promise<Session> {
    const res = await firstValueFrom(this.http.post<{ token: string; username?: string }>('/auth/login', { username, password }));
    store.set('token', res.token);
    store.set('username', res.username || username);
    const s = readSession();
    if (!s) throw new Error('The server returned a token this app could not read.');
    this.session.set(s);
    return s;
  }

  logout(): void {
    store.set('token', null);
    this.session.set(null);
    this.router.navigateByUrl('/login');
  }

  /** Called by the interceptor on 401. */
  expire(): void { this.logout(); }
}
