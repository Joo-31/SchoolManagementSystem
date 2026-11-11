import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../core/api.service';
import { useLoad } from '../core/load';
import { AppNotification } from '../core/models';
import { avg, isoDay, PALETTE, pctNorm, round1 } from '../core/util';
import { BarChartComponent, ChartItem, DonutComponent, LineChartComponent } from '../shared/charts';
import { NoteFeedComponent } from '../shared/blocks';
import { EmptyComponent, KpiComponent, StateComponent } from '../shared/primitives';

@Component({
  selector: 'app-admin-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, StateComponent, KpiComponent, EmptyComponent, BarChartComponent, DonutComponent, LineChartComponent, NoteFeedComponent],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        <div class="grid g-kpi">
          @for (k of x.kpis; track k.label) { <a app-kpi [routerLink]="k.link" [icon]="k.icon" [value]="k.value" [label]="k.label"></a> }
        </div>
        <div class="grid g-2" style="margin-top:18px">
          <div class="panel">
            <header><h3>Attendance this week</h3><span class="grow"></span>@if (x.trend.length) { <span class="badge ok num">{{ x.trendAvg }}% average</span> }</header>
            @if (x.trend.length) { <app-line-chart [points]="x.trend" /> } @else { <app-empty title="No attendance recorded this week" text="Teachers record it from My classes." /> }
          </div>
          <div class="panel"><header><h3>Students per grade</h3></header><app-bar-chart [items]="x.byGrade" [height]="200" /></div>
        </div>
        <div class="grid g-2e" style="margin-top:18px">
          <div class="panel">
            <header><h3>Teachers by subject</h3></header>
            @if (x.bySpec.length) {
              <div class="donut-wrap">
                <app-donut [items]="x.bySpec" />
                <div class="donut-legend">@for (s of x.bySpec; track s.label) { <div><i [style.background]="s.color"></i>{{ s.label }}<b>{{ s.value }}</b></div> }</div>
              </div>
            } @else { <app-empty title="No teachers yet" /> }
          </div>
          <div class="panel">
            <header><h3>Latest announcements</h3><span class="grow"></span><a class="btn sm" routerLink="/announcements">Manage</a></header>
            <app-note-feed [notes]="x.notes" />
          </div>
        </div>
      }
    </app-state>`,
})
export class AdminDashboardComponent {
  private api = inject(ApiService);
  private none = {} as Record<string, number>;

  d = useLoad(async () => {
    const days = Array.from({ length: 7 }, (_, i) => { const dt = new Date(); dt.setDate(dt.getDate() - (6 - i)); return dt; });
    const count = (k: string) => this.api.get<{ count?: number }>(`/${k}/count`).then((r) => r.count ?? 0).catch(() => 0);
    const [counts, byGrade, bySpec, trend, notes] = await Promise.all([
      Promise.all(['students', 'teachers', 'courses', 'classes', 'grades'].map(count)),
      this.api.get<Record<string, number>>('/grades/statistics/count-by-grade').catch(() => this.none),
      this.api.get<Record<string, number>>('/teachers/statistics/count-by-specialization').catch(() => this.none),
      Promise.all(days.map((dt) =>
        this.api.get<{ percentage?: number }>(`/attendances/statistics/percentage/${isoDay(dt)}`)
          .then((r): ChartItem => ({ label: dt.toLocaleDateString('en-GB', { weekday: 'short' }), value: round1(pctNorm(r.percentage ?? 0)) }))
          .catch((): ChartItem => ({ label: dt.toLocaleDateString('en-GB', { weekday: 'short' }), value: 0 })))),
      this.api.get<AppNotification[]>('/notifications').catch(() => [] as AppNotification[]),
    ]);
    const t = trend;
    return {
      kpis: [
        { icon: 'cap', value: counts[0], label: 'Students', link: '/students' },
        { icon: 'teacher', value: counts[1], label: 'Teachers', link: '/teachers' },
        { icon: 'book', value: counts[2], label: 'Courses', link: '/courses' },
        { icon: 'school', value: counts[3], label: 'Classes', link: '/classes' },
        { icon: 'layers', value: counts[4], label: 'Grades', link: '/grades' },
      ],
      trend: t,
      trendAvg: round1(avg(t.map((p) => p.value))),
      byGrade: Object.entries(byGrade).map(([label, value]): ChartItem => ({ label: label.replace('Grade ', 'G'), value })),
      bySpec: Object.entries(bySpec).map(([label, value], i): ChartItem => ({ label, value, color: PALETTE[i % PALETTE.length] })),
      notes,
    };
  });
}
