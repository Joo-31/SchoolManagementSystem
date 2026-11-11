import { ChangeDetectionStrategy, Component, HostListener, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { DialogService } from '../core/dialog.service';
import { AppNotification } from '../core/models';
import { NAV, NavItem } from '../core/nav';
import { OptionsService } from '../core/options.service';
import { store } from '../core/session';
import { SettingsService } from '../core/settings.service';
import { SchoolSettingsService } from '../core/school-settings.service';
import { AutofocusDirective, AvatarComponent, IconComponent, TimeAgoPipe } from '../shared/primitives';

@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, IconComponent, AvatarComponent, TimeAgoPipe, AutofocusDirective],
  template: `
    <div class="shell" [class.nav-open]="mobileNav()">
      <aside class="side">
        <div class="brand">
          @if (school.settings().logo; as logo) { <img class="school-logo" [src]="logo" alt=""> } @else { <span class="logo"><svg viewBox="0 0 32 32" width="22" height="22" fill="none" stroke="currentColor" stroke-width="3.2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M9 23 16 8l7 15M11.5 18.5h9" /></svg></span> }
          {{ school.settings().shortName || school.settings().schoolName }}
        </div>
        @if (settings.mode() === 'demo') { <span class="demo-pill">Demo data</span> }
        <nav class="nav" aria-label="Main">
          @for (g of groups(); track g.name) {
            <h4>{{ g.name }}</h4>
            @for (n of g.items; track n.path) {
              <a [routerLink]="'/' + n.path" routerLinkActive ariaCurrentWhenActive="page"><app-icon [name]="n.icon" />{{ n.title }}</a>
            }
          }
        </nav>
        <span class="spacer"></span>
        <div class="me">
          <app-avatar [name]="auth.session()?.username ?? ''" [size]="36" />
          <div class="who"><b>{{ auth.session()?.username }}</b><small>{{ auth.session()?.role }}</small></div>
          <button class="icon-btn" aria-label="Sign out" title="Sign out" (click)="logout()"><app-icon name="logout" /></button>
        </div>
      </aside>
      <div class="scrim" (click)="mobileNav.set(false)"></div>

      <div class="main">
        <header class="top" [class.scrolled]="scrolled()">
          <button class="icon-btn menu" aria-label="Open menu" (click)="mobileNav.set(true)"><app-icon name="menu" [size]="20" /></button>
          <h1>@if (school.settings().logo; as logo) { <img class="top-school-logo" [src]="logo" alt=""> }<span class="top-school-name">{{ school.settings().shortName || school.settings().schoolName }}</span><span aria-hidden="true"> · </span>{{ title() }}</h1>
          <span class="grow"></span>
          <button class="kbd-btn" aria-label="Jump to page" (click)="openPalette()"><app-icon name="search" [size]="16" /><span>Jump to…</span><kbd>Ctrl K</kbd></button>
          <button class="icon-btn" aria-label="Toggle dark mode" (click)="settings.toggleTheme()"><app-icon [name]="settings.theme() === 'dark' ? 'sun' : 'moon'" [size]="19" /></button>
          <button class="icon-btn" aria-label="Notifications" (click)="openNotes()">
            <app-icon name="bell" [size]="19" />
            @if (unread() > 0) { <span class="badge-dot">{{ unread() > 9 ? '9+' : unread() }}</span> }
          </button>
        </header>
        <main class="content" id="content"><router-outlet /></main>
      </div>
    </div>

    @if (showNotes()) {
      <div style="position:fixed;inset:0;z-index:80;background:rgba(9,11,30,.35)" (click)="showNotes.set(false)"></div>
      <aside class="drawer" role="dialog" aria-label="Announcements">
        <header><h2>Announcements</h2><button class="icon-btn" aria-label="Close" (click)="showNotes.set(false)"><app-icon name="x" /></button></header>
        <div class="body">
          @for (n of notes(); track n.id) {
            <div class="note"><b>{{ n.title }}</b><p>{{ n.message }}</p><span class="tiny">{{ n.createdAt | timeAgo }}</span></div>
          } @empty { <p class="muted" style="padding:24px 0">No announcements yet.</p> }
        </div>
      </aside>
    }

    @if (showPalette()) {
      <div class="overlay" style="align-items:start" (mousedown)="closePalette($event)">
        <div class="pal" role="dialog" aria-label="Jump to page">
          <input appAutofocus placeholder="Jump to a page…" aria-label="Jump to a page" autocomplete="off" [value]="paletteQuery()"
            (input)="paletteQuery.set($any($event.target).value); sel.set(0)" (keydown)="onPaletteKey($event)" />
          <ul role="listbox">
            @for (i of shown(); track i.path; let idx = $index) {
              <li role="option" [attr.aria-selected]="idx === sel()" (click)="go(i)" (mousemove)="sel.set(idx)"><app-icon [name]="i.icon" />{{ i.title }}<small>{{ i.group }}</small></li>
            } @empty { <li>No matching page</li> }
          </ul>
        </div>
      </div>
    }`,
})
export class ShellComponent {
  readonly auth = inject(AuthService);
  readonly settings = inject(SettingsService);
  readonly school = inject(SchoolSettingsService);
  private router = inject(Router);
  private api = inject(ApiService);
  private dialogs = inject(DialogService);
  private opts = inject(OptionsService);

  title = signal('');
  mobileNav = signal(false);
  scrolled = signal(false);
  notes = signal<AppNotification[]>([]);
  seen = signal(0);
  showNotes = signal(false);
  showPalette = signal(false);
  paletteQuery = signal('');
  sel = signal(0);

  private items = computed<NavItem[]>(() => NAV.filter((n) => n.roles.includes(this.auth.session()?.role as any)));
  groups = computed(() => {
    const items = this.items();
    return [...new Set(items.map((i) => i.group))].map((name) => ({ name, items: items.filter((i) => i.group === name) }));
  });
  unread = computed(() => this.notes().filter((n) => n.id > this.seen()).length);
  shown = computed(() => {
    const k = this.paletteQuery().toLowerCase();
    return this.items().filter((i) => i.title.toLowerCase().includes(k) || i.group.toLowerCase().includes(k));
  });

  constructor() {
    this.router.events.pipe(filter((e) => e instanceof NavigationEnd), takeUntilDestroyed()).subscribe(() => {
      this.title.set(this.leafTitle());
      this.mobileNav.set(false);
      this.showNotes.set(false);
      this.dialogs.closeAll();
      window.scrollTo(0, 0);
    });
    this.title.set(this.leafTitle());
    void this.loadNotes();
  }

  private leafTitle(): string {
    let r = this.router.routerState.snapshot.root;
    while (r.firstChild) r = r.firstChild;
    return (r.data['title'] as string) ?? '';
  }

  private seenKey = () => 'seen.' + (this.auth.session()?.username ?? '');
  private async loadNotes(): Promise<void> {
    this.seen.set(+(store.get(this.seenKey()) || 0));
    try { this.notes.set(await this.api.get<AppNotification[]>('/notifications')); } catch { this.notes.set([]); }
  }

  openNotes(): void {
    this.showNotes.set(true);
    const ids = this.notes().map((n) => n.id);
    if (ids.length) { const max = Math.max(...ids); store.set(this.seenKey(), String(max)); this.seen.set(max); }
  }

  logout(): void { this.opts.invalidate(); this.auth.logout(); }

  openPalette(): void { this.paletteQuery.set(''); this.sel.set(0); this.showPalette.set(true); }
  closePalette(e?: MouseEvent): void { if (!e || e.target === e.currentTarget) this.showPalette.set(false); }
  go(i: NavItem): void { this.showPalette.set(false); void this.router.navigateByUrl('/' + i.path); }
  onPaletteKey(e: KeyboardEvent): void {
    const n = this.shown().length;
    if (e.key === 'ArrowDown') { this.sel.set(Math.min(this.sel() + 1, n - 1)); e.preventDefault(); }
    else if (e.key === 'ArrowUp') { this.sel.set(Math.max(this.sel() - 1, 0)); e.preventDefault(); }
    else if (e.key === 'Enter') { const i = this.shown()[this.sel()]; if (i) this.go(i); }
    else if (e.key === 'Escape') this.showPalette.set(false);
  }

  @HostListener('window:keydown', ['$event'])
  onKey(e: KeyboardEvent): void {
    if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') { e.preventDefault(); this.openPalette(); }
  }
  @HostListener('window:scroll')
  onScroll(): void { this.scrolled.set(window.scrollY > 4); }
}
