import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { useLoad } from '../core/load';
import { AppNotification, Attendance, Homework, Mark, Student } from '../core/models';
import { avg, round1 } from '../core/util';
import { HwCardComponent, MarkRowComponent, NoteFeedComponent } from '../shared/blocks';
import { AvatarComponent, EmptyComponent, IconComponent, KpiComponent, StampComponent, StateComponent } from '../shared/primitives';

const byDateDesc = (a: { date: string }, b: { date: string }) => new Date(b.date).getTime() - new Date(a.date).getTime();

@Component({
  selector: 'app-student-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, StateComponent, AvatarComponent, StampComponent, EmptyComponent, MarkRowComponent, NoteFeedComponent, HwCardComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        <div class="hero">
          <app-avatar [name]="x.me.fullName" [size]="72" />
          <div class="grow">
            <h2>Hi, {{ x.me.firstName }}</h2><p>Class {{ x.me.className }} · age {{ x.me.age }}</p>
            <div class="stat-line" style="margin-top:14px">
              @for (s of x.stats; track s.label) { <div><b class="num">{{ s.value }}</b><span>{{ s.label }}</span></div> }
            </div>
          </div>
          @if (x.hasMarks) { <div style="text-align:center"><app-stamp [score]="x.average" size="lg" /><div class="tiny" style="color:var(--nav-ink);margin-top:8px">Overall average</div></div> }
        </div>
        <div class="grid g-2" style="margin-top:18px">
          <div class="panel">
            <header><h3>Latest marks</h3><span class="grow"></span><a class="btn sm" routerLink="/my-grades">See all</a></header>
            @for (m of x.recent; track m.id) { <app-mark-row [mark]="m" /> } @empty { <app-empty title="No marks yet" text="Your teachers’ marks appear here." /> }
          </div>
          <div class="panel"><header><h3>Announcements</h3></header><app-note-feed [notes]="x.notes" /></div>
        </div>
        <div class="panel" style="margin-top:18px">
          <header><h3>Newest homework</h3><span class="grow"></span><a class="btn sm" routerLink="/homework">All homework</a></header>
          @if (x.homework.length) { <div class="grid g-3">@for (h of x.homework; track h.id) { <app-hw-card [hw]="h" /> }</div> }
          @else { <app-empty title="No homework" text="Nothing assigned right now." /> }
        </div>
      }
    </app-state>`,
})
export class StudentDashboardComponent {
  private api = inject(ApiService);
  d = useLoad(async () => {
    const [me, att, marks, hw, notes] = await Promise.all([
      this.api.get<Student>('/students/me'),
      this.api.get<Attendance[]>('/students/me/attendances'),
      this.api.get<Mark[]>('/marks/me'),
      this.api.get<Homework[]>('/homework/student'),
      this.api.get<AppNotification[]>('/notifications').catch(() => [] as AppNotification[]),
    ]);
    const present = att.filter((a) => a.isPresent).length;
    const rate = att.length ? Math.round((present / att.length) * 100) : 0;
    return {
      me, notes, hasMarks: marks.length > 0, average: avg(marks.map((m) => m.score)),
      recent: [...marks].sort(byDateDesc).slice(0, 4), homework: hw.slice(0, 3),
      stats: [{ value: rate + '%', label: 'Attendance' }, { value: marks.length, label: 'Courses marked' }, { value: hw.length, label: 'Homework items' }],
    };
  });
}

@Component({
  selector: 'app-student-marks',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [StateComponent, EmptyComponent, StampComponent, MarkRowComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        @if (x.marks.length) {
          <div class="page-head"><div class="grow"><p>Every mark your teachers have entered.</p></div></div>
          <div class="grid g-kpi">
            <div class="kpi"><span>Average</span><div style="display:flex;align-items:center;gap:14px"><app-stamp [score]="x.average" size="lg" /></div></div>
            <div class="kpi"><span>Strongest</span><b>{{ x.best.courseName }}</b><span class="num">{{ round(x.best.score) }} / 100</span></div>
            <div class="kpi"><span>Needs attention</span><b>{{ x.low.courseName }}</b><span class="num">{{ round(x.low.score) }} / 100</span></div>
            <div class="kpi"><span>Courses marked</span><b class="num">{{ x.marks.length }}</b></div>
          </div>
          <div class="panel" style="margin-top:18px">@for (m of x.marks; track m.id) { <app-mark-row [mark]="m" /> }</div>
        } @else { <app-empty title="No marks yet" text="When a teacher enters a mark, it appears here." /> }
      }
    </app-state>`,
})
export class StudentMarksComponent {
  private api = inject(ApiService);
  round = round1;
  d = useLoad(async () => {
    const marks = (await this.api.get<Mark[]>('/marks/me')).sort(byDateDesc);
    const best = marks.reduce((a, b) => (b.score > a.score ? b : a), marks[0]);
    const low = marks.reduce((a, b) => (b.score < a.score ? b : a), marks[0]);
    return { marks, best, low, average: avg(marks.map((m) => m.score)) };
  });
}

@Component({
  selector: 'app-student-attendance',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, StateComponent, EmptyComponent, KpiComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        @if (x.total) {
          <div class="page-head"><div class="grow"><p>Each square is a school day.</p></div></div>
          <div class="grid g-kpi">
            <div app-kpi icon="check" [value]="x.present" label="Days present" tone="ok"></div>
            <div app-kpi icon="x" [value]="x.absent" label="Days absent" tone="bad"></div>
            <div app-kpi icon="cal" [value]="x.rate + '%'" label="Attendance rate" tone="brand"></div>
          </div>
          <div class="panel" style="margin-top:18px">
            <header><h3>Last {{ x.recent.length }} school days</h3></header>
            <div class="punch">
              @for (a of x.recent; track $index) {
                <div class="cell" [class.p]="a.isPresent" [class.a]="!a.isPresent" [attr.title]="(a.date | date: 'd MMM y') + ' — ' + (a.isPresent ? 'Present' : 'Absent')">{{ a.date | date: 'd' }}</div>
              }
            </div>
            <div class="legend">
              <span><i style="background:var(--ok)"></i>Present</span>
              <span><i style="background:var(--bad-wash);border:2px solid var(--bad)"></i>Absent</span>
            </div>
          </div>
          @if (x.missed.length) {
            <div class="panel" style="margin-top:18px">
              <header><h3>Days you missed</h3></header>
              <div style="display:flex;gap:8px;flex-wrap:wrap">@for (a of x.missed; track $index) { <span class="badge bad">{{ a.date | date: 'd MMM y' }}</span> }</div>
            </div>
          }
        } @else { <app-empty title="No attendance yet" text="Your attendance appears after teachers record it." /> }
      }
    </app-state>`,
})
export class StudentAttendanceComponent {
  private api = inject(ApiService);
  d = useLoad(async () => {
    const att = (await this.api.get<Attendance[]>('/students/me/attendances')).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
    const present = att.filter((a) => a.isPresent).length;
    return {
      total: att.length, present, absent: att.length - present,
      rate: att.length ? Math.round((present / att.length) * 100) : 0,
      recent: att.slice(-42), missed: att.filter((a) => !a.isPresent).reverse(),
    };
  });
}

@Component({
  selector: 'app-student-homework',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [StateComponent, EmptyComponent, HwCardComponent],
  template: `
    <div class="page-head"><div class="grow"><p>Assignments from your teachers, newest first.</p></div></div>
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as list) {
        @if (list.length) { <div class="grid g-3">@for (h of list; track h.id) { <app-hw-card [hw]="h" /> }</div> }
        @else { <app-empty title="No homework" text="You’re all caught up." /> }
      }
    </app-state>`,
})
export class StudentHomeworkComponent {
  private api = inject(ApiService);
  d = useLoad(() => this.api.get<Homework[]>('/homework/student'));
}
