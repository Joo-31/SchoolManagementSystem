import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { ApiError } from '../core/models';
import { SettingsService } from '../core/settings.service';
import { SchoolSettingsService } from '../core/school-settings.service';
import { AutofocusDirective, AvatarComponent } from '../shared/primitives';
import { LanguageService } from '../core/language.service';

@Component({
  selector: 'app-login',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AvatarComponent, AutofocusDirective],
  template: `
    <div class="login">
      <section class="login-art">
        <div class="brand">
          @if (school.settings().logo; as logo) { <img class="school-logo" [src]="logo" alt=""> } @else { <span class="logo"><svg viewBox="0 0 32 32" width="22" height="22" fill="none" stroke="currentColor" stroke-width="3.2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M9 23 16 8l7 15M11.5 18.5h9" /></svg></span> }
          {{ school.settings().schoolName }}
        </div>
        <div>
          <h1>Every student, every class, one clear picture.</h1>
          <p>Attendance, marks and homework for admins, teachers and students — without the spreadsheet shuffle.</p>
        </div>
        <div class="foot">School Management System</div>
        <svg class="big-stamp" viewBox="0 0 200 200" aria-hidden="true">
          <ellipse cx="100" cy="100" rx="86" ry="82" fill="none" stroke="currentColor" stroke-width="7" />
          <ellipse cx="102" cy="99" rx="94" ry="90" fill="none" stroke="currentColor" stroke-width="3" opacity=".5" />
          <text x="100" y="128" text-anchor="middle">A+</text>
        </svg>
      </section>

      <section class="login-form">
        <button class="language-btn login-language" type="button" [attr.aria-label]="language.text(language.language() === 'en' ? 'Switch to Arabic' : 'Switch to English')" (click)="language.toggle()">{{ language.language() === 'en' ? 'AR' : 'EN' }}</button>
        <div class="login-card">
          <div><h2>Welcome back</h2><p class="muted" style="margin-top:6px">Sign in with your school account.</p></div>
          <form (submit)="submit($event)">
            <div class="field"><label for="u">Username</label><input id="u" appAutofocus autocomplete="username" required placeholder="admin" [value]="username()" (input)="username.set($any($event.target).value)" /></div>
            <div class="field"><label for="p">Password</label><input id="p" type="password" autocomplete="current-password" required placeholder="••••••••" [value]="password()" (input)="password.set($any($event.target).value)" /></div>
            @if (error(); as e) {
              <div class="callout" role="alert">
                <span>{{ e.status === 401 ? 'That username or password isn’t right.' : e.message }}</span>
                @if (e.network) { <button type="button" class="btn sm" (click)="useDemo()">Try demo data instead</button> }
              </div>
            }
            <button class="btn primary" style="height:46px;justify-content:center" type="submit" [disabled]="busy()">{{ busy() ? 'Signing in…' : 'Sign in' }}</button>
          </form>
          <div class="demo-chips">
            <span class="tiny">Seeded accounts — tap to fill</span>
            @for (c of chips; track c.u) {
              <button type="button" class="chip" (click)="fill(c.u, c.p)"><app-avatar [name]="c.u" [size]="26" /><b>{{ c.u }}</b><span>{{ c.role }}</span></button>
            }
          </div>
          <details class="conn" [open]="settings.mode() === 'demo'">
            <summary>Connection</summary>
            <div class="inner">
              <div class="seg" role="group" aria-label="Data source">
                <button type="button" [attr.aria-pressed]="settings.mode() === 'live'" (click)="settings.setMode('live')">My API</button>
                <button type="button" [attr.aria-pressed]="settings.mode() === 'demo'" (click)="settings.setMode('demo')">Demo data</button>
              </div>
              @if (settings.mode() === 'live') {
                <div class="field"><label for="api">API base URL</label><input id="api" [value]="settings.apiUrl()" (change)="settings.setApiUrl($any($event.target).value)" /></div>
              } @else {
                <p class="tiny">Demo data runs entirely in your browser — nothing is sent anywhere, and changes reset on reload.</p>
              }
            </div>
          </details>
        </div>
      </section>
    </div>`,
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  readonly settings = inject(SettingsService);
  readonly school = inject(SchoolSettingsService);
  readonly language = inject(LanguageService);

  username = signal('');
  password = signal('');
  busy = signal(false);
  error = signal<ApiError | null>(null);
  chips = [
    { u: 'admin', p: 'Admin@123', role: 'Admin' },
    { u: 'teacher_ahmed', p: 'Teacher@123', role: 'Teacher' },
    { u: 'student_omar', p: 'Student@123', role: 'Student' },
  ];

  fill(u: string, p: string): void { this.username.set(u); this.password.set(p); }
  useDemo(): void { this.settings.setMode('demo'); this.error.set(null); }

  async submit(e: Event): Promise<void> {
    e.preventDefault();
    this.busy.set(true);
    this.error.set(null);
    try {
      await this.auth.login(this.username().trim(), this.password());
      await this.router.navigateByUrl(this.auth.home());
    } catch (err) {
      this.error.set(err instanceof ApiError ? err : new ApiError((err as Error).message));
    } finally { this.busy.set(false); }
  }
}
