import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { AppNotification, Homework, Mark } from '../core/models';
import { bandVar, safeUrl } from '../core/util';
import { EmptyComponent, IconComponent, StampComponent, TimeAgoPipe } from './primitives';

@Component({
  selector: 'app-note-feed',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [EmptyComponent, TimeAgoPipe],
  template: `
    @if (notes().length) {
      <div class="feed">
        @for (n of notes().slice(0, 4); track n.id) {
          <div class="item"><b>{{ n.title }}</b><p class="muted">{{ n.message }}</p><div class="meta">{{ n.createdAt | timeAgo }}</div></div>
        }
      </div>
    } @else {
      <app-empty title="No announcements" text="Messages from the school office show up here." />
    }`,
})
export class NoteFeedComponent { notes = input.required<AppNotification[]>(); }

@Component({
  selector: 'app-hw-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IconComponent, TimeAgoPipe],
  template: `
    <div class="hw">
      <div class="meta"><span class="badge brand">{{ hw().courseName }}</span><span class="badge">{{ hw().teacherName }}</span></div>
      <h3>{{ hw().title }}</h3>
      @if (hw().description) { <p class="muted">{{ hw().description }}</p> }
      <div class="foot">
        <span class="tiny">{{ hw().createdAt | timeAgo }}</span><span class="grow"></span>
        @if (url(); as u) {
          <a class="btn sm primary" [href]="u" target="_blank" rel="noopener noreferrer"><app-icon name="ext" [size]="14" />Open</a>
        } @else { <span class="tiny">No valid link</span> }
        @if (deletable()) { <button class="icon-btn danger" aria-label="Delete homework" (click)="remove.emit()"><app-icon name="trash" [size]="16" /></button> }
      </div>
    </div>`,
})
export class HwCardComponent {
  hw = input.required<Homework>();
  deletable = input(false);
  remove = output<void>();
  /** Homework links are teacher-entered text, so only http(s) URLs are ever rendered as links. */
  url = computed(() => safeUrl(this.hw().link));
}

@Component({
  selector: 'app-mark-row',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [StampComponent, DatePipe],
  template: `
    <div class="mark-row" [style.--c]="'var(--' + band() + ')'">
      <app-stamp [score]="mark().score" />
      <div class="grow">
        <b>{{ mark().courseName }}</b>
        <span class="tiny">{{ mark().teacherName }} · {{ mark().date | date: 'd MMM y' }}</span>
        <div class="meter"><i [style.width.%]="min100(mark().score)"></i></div>
        @if (mark().notes) { <div class="tiny" style="margin-top:6px">{{ mark().notes }}</div> }
      </div>
    </div>`,
})
export class MarkRowComponent {
  mark = input.required<Mark>();
  band = computed(() => bandVar(this.mark().score));
  min100 = (n: number) => Math.min(100, n);
}
