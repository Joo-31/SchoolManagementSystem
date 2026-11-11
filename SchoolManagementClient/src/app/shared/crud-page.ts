import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../core/api.service';
import { DialogService } from '../core/dialog.service';
import { CrudConfig, ListQuery, Option } from '../core/models';
import { OptionsService } from '../core/options.service';
import { ToastService } from '../core/toast.service';
import { cap } from '../core/util';
import { CRUD_CONFIGS } from './crud-configs';
import { AvatarComponent, EmptyComponent, IconComponent, StampComponent } from './primitives';

/** One list + add/edit/delete page, driven entirely by a CrudConfig chosen via route data { entity }. */
@Component({
  selector: 'app-crud-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, IconComponent, AvatarComponent, StampComponent, EmptyComponent],
  template: `
    <div class="page-head">
      <div class="grow"><p>{{ cfg.sub }}</p></div>
      <button class="btn primary" (click)="openForm()"><app-icon name="plus" [size]="16" />Add {{ cfg.singular }}</button>
    </div>

    <div class="toolbar">
      @if (cfg.searchable !== false) {
        <div class="search">
          <app-icon name="search" [size]="16" />
          <input type="search" [placeholder]="'Search ' + cfg.plural + '…'" aria-label="Search" (input)="onSearch($any($event.target).value)" />
        </div>
      }
      @for (f of cfg.filters ?? []; track f.key) {
        @if (f.type === 'date') {
          <input type="date" [attr.aria-label]="f.label" (change)="setFilter(f.key, $any($event.target).value)" />
        } @else {
          <select [attr.aria-label]="f.label" (change)="setFilter(f.key, $any($event.target).value)">
            <option value="">{{ f.label }}</option>
            @for (o of filterOptions()[f.key] ?? []; track o.value) { <option [value]="o.value">{{ o.label }}</option> }
          </select>
        }
      }
    </div>

    <div class="table-wrap">
      @if (loading()) {
        <div class="skeleton"><div class="sk-row"></div><div class="sk-row"></div><div class="sk-row"></div><div class="sk-row"></div><div class="sk-row"></div></div>
      } @else if (error(); as e) {
        <app-empty title="Something went wrong" [text]="e.message" [error]="true"><button class="btn" (click)="load()">Try again</button></app-empty>
      } @else if (!rows().length) {
        <app-empty [title]="filtered() ? 'Nothing matches' : 'No ' + cfg.plural + ' yet'"
          [text]="filtered() ? 'Try a different search or clear the filters.' : 'Add the first ' + cfg.singular + ' to get started.'">
          @if (!filtered()) { <button class="btn primary" (click)="openForm()">Add {{ cfg.singular }}</button> }
        </app-empty>
      } @else {
        <div class="scroll-x">
          <table>
            <thead>
              <tr>
                @for (c of cfg.columns; track c.label) { <th [class.hide-sm]="c.hide">{{ c.label }}</th> }
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (r of rows(); track r.id) {
                <tr>
                  @for (c of cfg.columns; track c.label) {
                    <td [class.hide-sm]="c.hide">
                      @switch (c.type) {
                        @case ('person') {
                          <div class="cell-person"><app-avatar [name]="c.value(r)" /><div><b>{{ c.value(r) }}</b>@if (c.sub) { <small>{{ c.sub(r) }}</small> }</div></div>
                        }
                        @case ('badge') { <span class="badge" [class]="c.tone ?? ''">{{ c.value(r) }}</span> }
                        @case ('stamp') { <app-stamp [score]="c.value(r)" /> }
                        @case ('date') { <span>{{ (c.value(r) | date: 'd MMM y') ?? '—' }}</span> }
                        @case ('status') { <span class="badge" [class]="c.value(r) ? 'ok' : 'bad'">{{ c.value(r) ? 'Present' : 'Absent' }}</span> }
                        @case ('muted') { <span class="muted">{{ c.value(r) }}</span> }
                        @default { <span>{{ c.value(r) }}</span> }
                      }
                    </td>
                  }
                  <td class="actions">
                    <button class="icon-btn" title="Edit" [attr.aria-label]="'Edit ' + cfg.singular" (click)="openForm(r)"><app-icon name="edit" [size]="16" /></button>
                    <button class="icon-btn danger" title="Delete" [attr.aria-label]="'Delete ' + cfg.singular" (click)="remove(r)"><app-icon name="trash" [size]="16" /></button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
      @if (!loading() && !error()) {
        <div class="pager">
          <span class="num">{{ from() }}–{{ to() }} of {{ total() }}</span>
          <span class="grow"></span>
          <select aria-label="Rows per page" (change)="setSize($any($event.target).value)">
            @for (n of [10, 25, 50]; track n) { <option [value]="n" [selected]="n === size()">{{ n }} / page</option> }
          </select>
          <button class="btn" [disabled]="page() <= 1" aria-label="Previous page" (click)="go(-1)"><app-icon name="left" [size]="16" /></button>
          <span class="num">{{ page() }} / {{ pages() }}</span>
          <button class="btn" [disabled]="page() >= pages()" aria-label="Next page" (click)="go(1)"><app-icon name="right" [size]="16" /></button>
        </div>
      }
    </div>`,
})
export class CrudPageComponent {
  private api = inject(ApiService);
  private opts = inject(OptionsService);
  private dialogs = inject(DialogService);
  private toast = inject(ToastService);
  readonly cfg: CrudConfig = CRUD_CONFIGS[inject(ActivatedRoute).snapshot.data['entity']]({ api: this.api, opts: this.opts });

  rows = signal<any[]>([]);
  total = signal(0);
  loading = signal(true);
  error = signal<Error | null>(null);
  page = signal(1);
  size = signal(10);
  q = signal('');
  filters = signal<Record<string, string>>({});
  filterOptions = signal<Record<string, Option[]>>({});
  private timer: ReturnType<typeof setTimeout> | undefined;

  pages = () => Math.max(1, Math.ceil(this.total() / this.size()));
  from = () => (this.total() ? (this.page() - 1) * this.size() + 1 : 0);
  to = () => Math.min(this.total(), this.page() * this.size());
  filtered = () => !!this.q() || Object.values(this.filters()).some(Boolean);

  constructor() {
    for (const f of this.cfg.filters ?? []) {
      const src = f.options ? Promise.resolve(f.options) : f.optionsLoader?.();
      src?.then((o) => this.filterOptions.update((m) => ({ ...m, [f.key]: o }))).catch(() => undefined);
    }
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    const query: ListQuery = { page: this.page(), size: this.size(), q: this.q(), filters: this.filters() };
    try {
      const r = await this.cfg.list(query);
      this.rows.set(r.rows);
      this.total.set(r.total);
    } catch (e) { this.error.set(e as Error); }
    finally { this.loading.set(false); }
  }

  onSearch(v: string): void {
    clearTimeout(this.timer);
    this.timer = setTimeout(() => { this.q.set(v.trim()); this.page.set(1); void this.load(); }, 280);
  }
  setFilter(key: string, v: string): void { this.filters.update((f) => ({ ...f, [key]: v })); this.page.set(1); void this.load(); }
  setSize(v: string): void { this.size.set(+v); this.page.set(1); void this.load(); }
  go(d: number): void { this.page.update((p) => p + d); void this.load(); }

  async openForm(row?: any): Promise<void> {
    const isEdit = !!row;
    const saved = await this.dialogs.form({
      title: `${isEdit ? 'Edit' : 'Add'} ${this.cfg.singular}`,
      fields: this.cfg.fields,
      values: isEdit ? this.cfg.toForm(row) : (this.cfg.defaults ?? {}),
      isEdit, wide: this.cfg.wide,
      submitLabel: isEdit ? 'Save changes' : `Add ${this.cfg.singular}`,
      submit: async (v) => {
        const payload = await this.cfg.toPayload(v, isEdit, row);
        if (isEdit) await this.api.put(`${this.cfg.base}/${row.id}`, payload);
        else await this.api.post(this.cfg.base, payload);
      },
    });
    if (saved) {
      this.opts.invalidate();
      this.toast.ok(`${cap(this.cfg.singular)} ${isEdit ? 'updated' : 'added'}`);
      void this.load();
    }
  }

  async remove(row: any): Promise<void> {
    const ok = await this.dialogs.confirm({ title: `Delete ${this.cfg.singular}?`, text: this.cfg.deleteText?.(row) ?? 'This can’t be undone.' });
    if (!ok) return;
    try {
      await this.api.delete(`${this.cfg.base}/${row.id}`);
      this.opts.invalidate();
      this.toast.ok(`${cap(this.cfg.singular)} deleted`);
      if (this.rows().length === 1 && this.page() > 1) this.page.update((p) => p - 1);
      void this.load();
    } catch (e) { this.toast.err((e as Error).message); }
  }
}
