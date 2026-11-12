import { ChangeDetectionStrategy, Component, Directive, ElementRef, OnInit, computed, inject, input, output, Pipe, PipeTransform } from '@angular/core';
import { DatePipe, registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import { ICON_PATHS } from './icon-paths';
import { bandVar, gradeBand, initials, letter } from '../core/util';
import { LanguageService } from '../core/language.service';

registerLocaleData(localeAr, 'ar');

@Component({
  selector: 'app-icon',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<svg class="ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.9" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" [attr.width]="size()" [attr.height]="size()"><path [attr.d]="d()" /></svg>`,
})
export class IconComponent {
  name = input.required<string>();
  size = input(18);
  d = computed(() => ICON_PATHS[this.name()] ?? ICON_PATHS['alert']);
}

const HUES = [236, 168, 28, 330, 200, 270, 12, 100];

@Component({
  selector: 'app-avatar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="avatar" aria-hidden="true" [style.--s]="size() + 'px'" [style.--h]="hue()">{{ text() }}</span>`,
})
export class AvatarComponent {
  name = input('');
  size = input(34);
  text = computed(() => initials(this.name()));
  hue = computed(() => {
    let n = 0;
    for (const c of this.name() || '') n = (n * 31 + c.charCodeAt(0)) >>> 0;
    return HUES[n % HUES.length];
  });
}

/** A score drawn like a teacher's red-pen mark. */
@Component({
  selector: 'app-stamp',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="stamp" [class]="cls()" [attr.title]="rounded() + ' / 100'"><b>{{ rounded() }}</b><i>{{ grade() }}</i></span>`,
})
export class StampComponent {
  score = input.required<number>();
  size = input<'' | 'lg'>('');
  rounded = computed(() => Math.round(this.score() * 10) / 10);
  grade = computed(() => letter(this.score()));
  cls = computed(() => `band-${gradeBand(this.score())} ${this.size()}`);
}

@Component({
  selector: 'app-empty',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IconComponent],
  template: `
    <div class="empty" [class.error]="error()">
      <app-icon [name]="error() ? 'alert' : 'inbox'" [size]="28" />
      <h3>{{ title() }}</h3>
      @if (text()) { <p>{{ text() }}</p> }
      <ng-content />
    </div>`,
})
export class EmptyComponent {
  title = input.required<string>();
  text = input('');
  error = input(false);
}

/** Wraps page content: skeleton while loading, an error card with retry, otherwise the projected content. */
@Component({
  selector: 'app-state',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [EmptyComponent],
  template: `
    @if (loading()) {
      <div class="skeleton"><div class="sk-row"></div><div class="sk-row"></div><div class="sk-row"></div><div class="sk-row"></div></div>
    } @else if (error(); as e) {
      <app-empty title="Something went wrong" [text]="e.message" [error]="true"><button class="btn" (click)="retry.emit()">Try again</button></app-empty>
    } @else {
      <ng-content />
    }`,
})
export class StateComponent {
  loading = input(false);
  error = input<Error | null>(null);
  retry = output<void>();
}

@Component({
  selector: '[app-kpi]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IconComponent],
  template: `
    <span class="k-ico"><app-icon [name]="icon()" /></span>
    <b class="num">{{ value() }}</b>
    <span>{{ label() }}</span>`,
  host: { '[class]': "'kpi' + (tone() ? ' t-' + tone() : '')" },
})
export class KpiComponent {
  icon = input.required<string>();
  value = input.required<string | number>();
  label = input.required<string>();
  tone = input('');
}

@Pipe({ name: 'timeAgo', standalone: true, pure: false })
export class TimeAgoPipe implements PipeTransform {
  private readonly language = inject(LanguageService);
  transform(d: string | Date | null | undefined): string {
    if (!d) return '';
    const s = (Date.now() - new Date(d).getTime()) / 1000;
    const isArabic = this.language.language() === 'ar';
    if (s < 60) return isArabic ? 'الآن' : 'just now';
    if (s < 3600) return isArabic ? `منذ ${Math.floor(s / 60)} دقيقة` : Math.floor(s / 60) + ' min ago';
    if (s < 86400) return isArabic ? `منذ ${Math.floor(s / 3600)} ساعة` : Math.floor(s / 3600) + ' h ago';
    if (s < 604800) return isArabic ? `منذ ${Math.floor(s / 86400)} يوم` : Math.floor(s / 86400) + ' d ago';
    return new Date(d).toLocaleDateString(isArabic ? 'ar' : 'en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
  }
}

@Pipe({ name: 'localizedDate', standalone: true, pure: false })
export class LocalizedDatePipe implements PipeTransform {
  private readonly language = inject(LanguageService);
  transform(value: string | number | Date | null | undefined, format = 'mediumDate'): string | null {
    return new DatePipe(this.language.language() === 'ar' ? 'ar' : 'en-GB').transform(value, format);
  }
}

@Pipe({ name: 'bandVar' })
export class BandVarPipe implements PipeTransform {
  transform(score: number): string { return bandVar(score); }
}

/** Focuses the element when it is created (autofocus attributes are ignored for elements added after page load). */
@Directive({ selector: '[appAutofocus]' })
export class AutofocusDirective implements OnInit {
  private el = inject(ElementRef<HTMLElement>);
  ngOnInit(): void { setTimeout(() => (this.el.nativeElement as HTMLElement).focus()); }
}
