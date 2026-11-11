import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { DialogService } from '../core/dialog.service';
import { useLoad } from '../core/load';
import { AppNotification, Attendance, Homework, Mark, SchoolClass, Student, Teacher } from '../core/models';
import { OptionsService } from '../core/options.service';
import { ToastService } from '../core/toast.service';
import { day10, dt, isoDay, safeUrl } from '../core/util';
import { HwCardComponent, NoteFeedComponent } from '../shared/blocks';
import { AvatarComponent, EmptyComponent, IconComponent, StampComponent, StateComponent } from '../shared/primitives';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-teacher-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, DatePipe, StateComponent, AvatarComponent, EmptyComponent, IconComponent, StampComponent, NoteFeedComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        <div class="hero">
          <app-avatar [name]="x.me.name" [size]="72" />
          <div class="grow"><h2>Hello, {{ x.me.name.split(' ')[0] }}</h2><p>{{ x.me.specialization }} · {{ x.me.yearsOfExperience }} years teaching</p></div>
          <div class="stat-line">
            @for (s of x.stats; track s.label) { <div><b class="num">{{ s.value }}</b><span>{{ s.label }}</span></div> }
          </div>
        </div>
        <div class="grid g-2" style="margin-top:18px">
          <div class="panel">
            <header><h3>Take attendance</h3></header>
            @if (x.classes.length) {
              <div class="roster">
                @for (c of x.classes; track c.id) {
                  <a class="row" [routerLink]="['/my-classes', c.id]" style="text-decoration:none;color:inherit">
                    <span class="badge brand">{{ c.gradeName }}</span><span class="name">Class {{ c.name }}</span>
                    <span class="btn sm"><app-icon name="cal" [size]="14" />Open roster</span>
                  </a>
                }
              </div>
            } @else { <app-empty title="No classes assigned" text="Ask an admin to make you a class teacher." /> }
          </div>
          <div class="panel"><header><h3>Latest announcements</h3></header><app-note-feed [notes]="x.notes" /></div>
        </div>
        <div class="panel" style="margin-top:18px">
          <header><h3>Recently marked</h3><span class="grow"></span><a class="btn sm" routerLink="/my-marks">All marks</a></header>
          @for (m of x.recent; track m.id) {
            <div class="mark-row"><app-stamp [score]="m.score" /><div class="grow"><b>{{ m.studentName }}</b><span class="tiny">{{ m.courseName }} · {{ m.date | date: 'd MMM y' }}</span></div></div>
          } @empty { <app-empty title="No marks yet" text="Enter a mark and it shows up here." /> }
        </div>
      }
    </app-state>`,
})
export class TeacherDashboardComponent {
  private api = inject(ApiService);
  d = useLoad(async () => {
    const [me, classes, courses, marks, hw, notes] = await Promise.all([
      this.api.get<Teacher>('/teachers/me'),
      this.api.get<SchoolClass[]>('/teachers/me/classes'),
      this.api.get<unknown[]>('/marks/my-courses'),
      this.api.get<{ data: Mark[]; totalCount: number }>('/marks/paged?pageNumber=1&pageSize=5'),
      this.api.get<Homework[]>('/homework/my-homework'),
      this.api.get<AppNotification[]>('/notifications').catch(() => [] as AppNotification[]),
    ]);
    return {
      me, classes, notes, recent: marks.data,
      stats: [
        { value: classes.length, label: 'Classes' }, { value: courses.length, label: 'Courses' },
        { value: marks.totalCount, label: 'Marks entered' }, { value: hw.length, label: 'Homework posted' },
      ],
    };
  });
}

@Component({
  selector: 'app-my-classes',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, StateComponent, EmptyComponent, IconComponent],
  template: `
    <div class="page-head"><div class="grow"><p>Pick a class to take today’s attendance.</p></div></div>
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as list) {
        @if (list.length) {
          <div class="grid g-3">
            @for (c of list; track c.id) {
              <a class="class-card" [routerLink]="['/my-classes', c.id]">
                <div class="top-row"><h3>{{ c.name }}</h3><span class="badge brand">{{ c.gradeName }}</span><span class="go">Roster<app-icon name="right" [size]="16" /></span></div>
                <div class="muted">{{ c.count != null ? c.count + ' students' : '' }}</div>
              </a>
            }
          </div>
        } @else { <app-empty title="No classes assigned" text="An admin can make you the class teacher." /> }
      }
    </app-state>`,
})
export class MyClassesComponent {
  private api = inject(ApiService);
  d = useLoad(async () => {
    const classes = await this.api.get<SchoolClass[]>('/teachers/me/classes');
    const counts = await Promise.all(classes.map((c) => this.api.get<Student[]>(`/teachers/me/classes/${c.id}/students`).then((r) => r.length).catch(() => null)));
    return classes.map((c, i) => ({ ...c, count: counts[i] }));
  });
}

@Component({
  selector: 'app-class-roster',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, StateComponent, AvatarComponent, EmptyComponent, IconComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        <div class="page-head">
          <div class="grow"><p>{{ x.cls.gradeName }} · everyone starts as present — tap to change.</p></div>
          <a class="btn" routerLink="/my-classes"><app-icon name="left" [size]="16" />Classes</a>
        </div>
        <div class="toolbar">
          <input type="date" aria-label="Date" [value]="date()" [max]="today" (change)="setDate($any($event.target).value)" />
          <button class="btn" (click)="setAll(true)">Mark all present</button>
          <button class="btn" (click)="setAll(false)">Mark all absent</button>
        </div>
        <div class="panel">
          @if (x.students.length) {
            <div class="roster">
              @for (s of x.students; track s.id) {
                <div class="row" (click)="toggle(s.id)">
                  <app-avatar [name]="s.fullName" /><span class="name">{{ s.fullName }}</span>
                  <div class="toggle" (click)="$event.stopPropagation()">
                    <button [class.on-p]="isPresent(s.id)" [attr.aria-pressed]="isPresent(s.id)" (click)="set(s.id, true)">Present</button>
                    <button [class.on-a]="!isPresent(s.id)" [attr.aria-pressed]="!isPresent(s.id)" (click)="set(s.id, false)">Absent</button>
                  </div>
                </div>
              }
            </div>
          } @else { <app-empty title="No students in this class" /> }
        </div>
        <div class="sticky-bar">
          <div class="grow"><b class="num">{{ presentCount() }} present</b><span class="tiny" style="color:var(--nav-ink);margin-left:10px">{{ x.students.length - presentCount() }} absent · {{ x.students.length }} total</span></div>
          <button class="btn" style="background:var(--hi);border-color:var(--hi);color:var(--hi-ink)" [disabled]="saving()" (click)="save()"><app-icon name="check" [size]="16" />Save attendance</button>
        </div>
      }
    </app-state>`,
})
export class ClassRosterComponent {
  private api = inject(ApiService);
  private toast = inject(ToastService);
  private classId = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  readonly today = isoDay();

  date = signal(isoDay());
  state = signal<Record<number, boolean>>({});
  saving = signal(false);
  private students: Student[] = [];
  private allAtt: Attendance[] = [];

  d = useLoad(async () => {
    const [classes, students, att] = await Promise.all([
      this.api.get<SchoolClass[]>('/teachers/me/classes'),
      this.api.get<Student[]>(`/teachers/me/classes/${this.classId}/students`),
      this.api.get<Attendance[]>(`/attendances/class/${this.classId}`).catch(() => [] as Attendance[]),
    ]);
    const cls = classes.find((c) => c.id === this.classId);
    if (!cls) throw new Error('This class isn’t assigned to you.');
    this.students = students;
    this.allAtt = att;
    this.seed();
    return { cls, students };
  });

  presentCount = computed(() => Object.values(this.state()).filter(Boolean).length);
  isPresent = (id: number) => this.state()[id] ?? true;

  /** Pre-fills from any attendance already recorded for the chosen date; everyone else defaults to present. */
  private seed(): void {
    const existing = this.allAtt.filter((a) => day10(a.date) === this.date());
    const next: Record<number, boolean> = {};
    for (const s of this.students) next[s.id] = existing.find((a) => a.studentId === s.id)?.isPresent ?? true;
    this.state.set(next);
  }
  setDate(v: string): void { this.date.set(v || isoDay()); this.seed(); }
  set(id: number, v: boolean): void { this.state.update((s) => ({ ...s, [id]: v })); }
  toggle(id: number): void { this.set(id, !this.isPresent(id)); }
  setAll(v: boolean): void { const next: Record<number, boolean> = {}; for (const s of this.students) next[s.id] = v; this.state.set(next); }

  async save(): Promise<void> {
    this.saving.set(true);
    try {
      await this.api.post('/attendances/take', {
        classId: this.classId, date: dt(this.date()),
        records: this.students.map((s) => ({ studentId: s.id, isPresent: this.isPresent(s.id) })),
      });
      // keep the local copy in sync so switching dates back and forth shows what was saved
      this.allAtt = [
        ...this.allAtt.filter((a) => day10(a.date) !== this.date()),
        ...this.students.map((s) => ({ id: 0, studentId: s.id, studentName: s.fullName, classId: this.classId, className: '', date: dt(this.date()), isPresent: this.isPresent(s.id) })),
      ];
      this.toast.ok(`Attendance saved for ${new Date(this.date()).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })}`);
    } catch (e) { this.toast.err((e as Error).message); }
    finally { this.saving.set(false); }
  }
}

@Component({
  selector: 'app-teacher-homework',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [StateComponent, EmptyComponent, IconComponent, HwCardComponent],
  template: `
    <div class="page-head">
      <div class="grow"><p>What you’ve assigned to your students.</p></div>
      <button class="btn primary" (click)="add()"><app-icon name="plus" [size]="16" />Add homework</button>
    </div>
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as list) {
        @if (list.length) {
          <div class="grid g-3">@for (h of list; track h.id) { <app-hw-card [hw]="h" [deletable]="true" (remove)="remove(h)" /> }</div>
        } @else {
          <app-empty title="No homework posted" text="Share a video, file or quiz link with your students."><button class="btn primary" (click)="add()">Add homework</button></app-empty>
        }
      }
    </app-state>`,
})
export class TeacherHomeworkComponent {
  private api = inject(ApiService);
  private opts = inject(OptionsService);
  private dialogs = inject(DialogService);
  private toast = inject(ToastService);
  d = useLoad(() => this.api.get<Homework[]>('/homework/my-homework'));

  async add(): Promise<void> {
    const saved = await this.dialogs.form({
      title: 'Add homework', submitLabel: 'Post homework',
      fields: [
        { key: 'title', label: 'Title', required: true, full: true },
        { key: 'courseId', label: 'Course', type: 'select', optionsLoader: this.opts.myCourses, required: true },
        { key: 'link', label: 'Link', type: 'url', required: true, placeholder: 'https://…', hint: 'A video, file or quiz URL.' },
        { key: 'description', label: 'Description', type: 'textarea', full: true },
      ],
      submit: async (v) => {
        if (!safeUrl(v.link)) throw new Error('The link must start with http:// or https://');
        await this.api.post('/homework', v);
      },
    });
    if (saved) { this.toast.ok('Homework posted'); void this.d.reload(); }
  }

  async remove(h: Homework): Promise<void> {
    if (!(await this.dialogs.confirm({ title: 'Delete homework?', text: `“${h.title}” will be removed for students.` }))) return;
    try { await this.api.delete('/homework/' + h.id); this.toast.ok('Homework deleted'); void this.d.reload(); }
    catch (e) { this.toast.err((e as Error).message); }
  }
}
