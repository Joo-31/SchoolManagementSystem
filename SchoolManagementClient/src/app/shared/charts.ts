import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export interface ChartItem { label: string; value: number; color?: string; }

@Component({
  selector: 'app-bar-chart',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <svg class="chart" role="img" aria-label="Bar chart" [attr.viewBox]="'0 0 ' + width() + ' ' + height()">
      @for (g of gridLines(); track g) { <line class="grid" x1="0" [attr.x2]="width()" [attr.y1]="g" [attr.y2]="g" /> }
      @for (b of bars(); track b.label) {
        <g class="bar">
          <title>{{ b.label }}: {{ b.value }}</title>
          <rect [attr.x]="b.x" [attr.y]="b.y" width="36" [attr.height]="b.h" rx="6" [attr.fill]="b.color" />
          <text class="val" text-anchor="middle" [attr.x]="b.x + 18" [attr.y]="b.y - 6">{{ b.value }}</text>
          <text class="lbl" text-anchor="middle" [attr.x]="b.x + 18" [attr.y]="height() - 8">{{ b.label }}</text>
        </g>
      }
    </svg>`,
})
export class BarChartComponent {
  items = input.required<ChartItem[]>();
  height = input(220);
  width = computed(() => Math.max(1, this.items().length) * 56);
  private base = computed(() => this.height() - 28);
  gridLines = computed(() => [0.5, 1].map((t) => this.base() - (this.base() - 18) * t));
  bars = computed(() => {
    const items = this.items();
    const max = Math.max(...items.map((i) => i.value), 1);
    const base = this.base();
    return items.map((it, i) => {
      const h = Math.max(2, (base - 18) * (it.value / max));
      return { label: it.label, value: it.value, x: i * 56 + 10, y: base - h, h, color: it.color ?? 'var(--brand)' };
    });
  });
}

@Component({
  selector: 'app-donut',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <svg class="donut" viewBox="0 0 160 160" width="160" height="160" role="img" aria-label="Donut chart">
      <circle class="track" cx="80" cy="80" r="56" fill="none" stroke-width="22" />
      @for (a of arcs(); track a.label) {
        <circle cx="80" cy="80" r="56" fill="none" stroke-width="22" transform="rotate(-90 80 80)"
          [attr.stroke]="a.color" [attr.stroke-dasharray]="a.dash" [attr.stroke-dashoffset]="a.offset"><title>{{ a.label }}: {{ a.value }}</title></circle>
      }
      <text class="donut-n" x="80" y="78" text-anchor="middle">{{ total() }}</text>
      <text class="lbl" x="80" y="96" text-anchor="middle">total</text>
    </svg>`,
})
export class DonutComponent {
  items = input.required<ChartItem[]>();
  total = computed(() => this.items().reduce((a, b) => a + b.value, 0));
  arcs = computed(() => {
    const c = 2 * Math.PI * 56, total = this.total() || 1;
    let off = 0;
    return this.items().map((it) => {
      const len = (it.value / total) * c;
      const arc = { label: it.label, value: it.value, color: it.color ?? 'var(--brand)', dash: `${Math.max(len - 2, 0)} ${c}`, offset: -off };
      off += len;
      return arc;
    });
  });
}

@Component({
  selector: 'app-line-chart',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <svg class="chart" role="img" aria-label="Attendance trend" [attr.viewBox]="'0 0 420 ' + height()">
      @for (g of grid(); track g.t) {
        <line class="grid" x1="28" x2="420" [attr.y1]="g.y" [attr.y2]="g.y" />
        <text class="lbl" x="0" [attr.y]="g.y + 4">{{ g.t }}%</text>
      }
      <path class="area" [attr.d]="areaPath()" />
      <path class="line" [attr.d]="linePath()" />
      @for (p of pts(); track p.label + p.x) {
        <circle class="dot" r="4" [attr.cx]="p.x" [attr.cy]="p.y"><title>{{ p.label }}: {{ p.value }}%</title></circle>
        <text class="lbl" text-anchor="middle" [attr.x]="p.x" [attr.y]="height() - 6">{{ p.label }}</text>
      }
    </svg>`,
})
export class LineChartComponent {
  points = input.required<ChartItem[]>();
  height = input(180);
  private base = computed(() => this.height() - 26);
  grid = computed(() => [0, 50, 100].map((t) => ({ t, y: this.base() - (this.base() - 26) * (t / 100) })));
  pts = computed(() => {
    const p = this.points(), base = this.base();
    return p.map((x, i) => ({
      label: x.label, value: x.value,
      x: 40 + (p.length === 1 ? 0 : i * (360 / (p.length - 1))),
      y: base - (base - 26) * (x.value / 100),
    }));
  });
  linePath = computed(() => this.pts().map((p, i) => `${i ? 'L' : 'M'}${p.x},${p.y}`).join(' '));
  areaPath = computed(() => {
    const p = this.pts();
    if (!p.length) return '';
    return `${this.linePath()} L${p[p.length - 1].x},${this.base()} L${p[0].x},${this.base()}Z`;
  });
}
