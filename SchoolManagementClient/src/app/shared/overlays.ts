import { ChangeDetectionStrategy, Component, ElementRef, HostListener, OnInit, forwardRef, inject, input, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { DialogService, FormDialogReq } from '../core/dialog.service';
import { FormField, Option } from '../core/models';
import { ToastService } from '../core/toast.service';
import { AvatarComponent, IconComponent } from './primitives';

@Component({
  selector: 'app-toast-host',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IconComponent],
  template: `
    <div id="toasts" aria-live="polite">
      @for (t of toasts.items(); track t.id) {
        <div class="toast" [class.ok]="t.type === 'ok'" [class.err]="t.type === 'err'" [class.out]="t.out" role="status">
          <app-icon [name]="t.type === 'ok' ? 'check' : 'alert'" [size]="16" /><span>{{ t.msg }}</span>
        </div>
      }
    </div>`,
})
export class ToastHostComponent {
  toasts = inject(ToastService);
}

@Component({
  selector: 'app-dialog-host',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IconComponent, forwardRef(() => FormDialogComponent)],
  template: `
    @for (d of dialogs.stack(); track d.id) {
      @if (d.kind === 'confirm') {
        <div class="overlay" (mousedown)="onBackdrop($event, d)">
          <div class="modal" role="dialog" aria-modal="true" [attr.aria-label]="d.title">
            <header><h2>{{ d.title }}</h2><button class="icon-btn" aria-label="Close" (click)="dialogs.close(d, false)"><app-icon name="x" /></button></header>
            <div class="modal-body"><p class="muted">{{ d.text }}</p></div>
            <footer>
              <button class="btn" type="button" (click)="dialogs.close(d, false)">Cancel</button>
              <button class="btn" type="button" [class.danger]="d.danger" [class.primary]="!d.danger" (click)="dialogs.close(d, true)">{{ d.confirm }}</button>
            </footer>
          </div>
        </div>
      } @else {
        <app-form-dialog [req]="d" />
      }
    }`,
  host: { '(document:keydown.escape)': 'onEscape()' },
})
export class DialogHostComponent {
  dialogs = inject(DialogService);
  onBackdrop(e: MouseEvent, d: any): void { if (e.target === e.currentTarget) this.dialogs.close(d, false); }
  onEscape(): void {
    const s = this.dialogs.stack();
    if (s.length) this.dialogs.close(s[s.length - 1], false);
  }
}

@Component({
  selector: 'app-form-dialog',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, IconComponent, AvatarComponent],
  template: `
    <div class="overlay" (mousedown)="onBackdrop($event)">
      <div class="modal" [class.wide]="req().wide" role="dialog" aria-modal="true" [attr.aria-label]="req().title">
        <header><h2>{{ req().title }}</h2><button class="icon-btn" aria-label="Close" (click)="close(false)"><app-icon name="x" /></button></header>
        <div class="modal-body">
          @if (form) {
            <form class="form-grid" [formGroup]="form" novalidate (ngSubmit)="save()">
              @for (f of fields; track f.key) {
                <div class="field" [class.full]="f.full" [class.bad]="submitted() && form.controls[f.key].invalid">
                  <label [attr.for]="'f_' + f.key">{{ f.label }}{{ f.required ? '' : ' (optional)' }}</label>
                  @switch (f.type) {
                    @case ('select') {
                      <select [id]="'f_' + f.key" [formControlName]="f.key">
                        <option [ngValue]="''">{{ f.placeholder || 'Select…' }}</option>
                        @for (o of selectOptions()[f.key] ?? []; track o.value) { <option [ngValue]="o.value">{{ o.label }}</option> }
                      </select>
                    }
                    @case ('number') {
                      <input [id]="'f_' + f.key" type="number" [formControlName]="f.key" [attr.min]="f.min" [attr.max]="f.max" [attr.step]="f.step" [attr.placeholder]="f.placeholder" autocomplete="off" />
                    }
                    @case ('textarea') { <textarea [id]="'f_' + f.key" rows="3" [formControlName]="f.key"></textarea> }
                    @case ('switch') {
                      <label class="switch"><input [id]="'f_' + f.key" type="checkbox" [formControlName]="f.key" /><span></span></label>
                    }
                    @case ('lookup') {
                      <div class="lookup">
                        <input type="text" autocomplete="off" [id]="'f_' + f.key" [placeholder]="f.placeholder || 'Type to search…'"
                          [value]="lookupText()[f.key] ?? ''" (input)="onLookup(f, $any($event.target).value)" (focus)="onLookup(f, $any($event.target).value)" />
                        @if (openLookup() === f.key) {
                          <ul class="lookup-list">
                            @for (o of lookupResults()[f.key] ?? []; track o.value) {
                              <li tabindex="0" (click)="pick(f, o)" (keydown.enter)="pick(f, o)"><app-avatar [name]="o.label" [size]="24" /><span>{{ o.label }}</span>@if (o.sub) { <small>{{ o.sub }}</small> }</li>
                            } @empty { <li class="none">No matches</li> }
                          </ul>
                        }
                      </div>
                    }
                    @default {
                      <input [id]="'f_' + f.key" [type]="f.type || 'text'" [formControlName]="f.key" [attr.min]="f.min" [attr.max]="f.max" [attr.step]="f.step"
                        [attr.placeholder]="f.placeholder" [attr.autocomplete]="f.type === 'password' ? 'new-password' : 'off'" />
                    }
                  }
                  @if (f.hint) { <small>{{ f.hint }}</small> }
                  @if (submitted() && form.controls[f.key].invalid) { <small class="field-error">This field needs a valid value.</small> }
                </div>
              }
              <button type="submit" hidden></button>
            </form>
          }
        </div>
        <footer>
          <button class="btn" type="button" (click)="close(false)">Cancel</button>
          <button class="btn primary" type="button" [disabled]="busy()" (click)="save()">{{ req().submitLabel }}</button>
        </footer>
      </div>
    </div>`,
})
export class FormDialogComponent implements OnInit {
  req = input.required<FormDialogReq>();
  private dialogs = inject(DialogService);
  private toast = inject(ToastService);
  private host = inject(ElementRef<HTMLElement>);

  form: FormGroup | null = null;
  fields: FormField[] = [];
  submitted = signal(false);
  busy = signal(false);
  selectOptions = signal<Record<string, Option[]>>({});
  lookupText = signal<Record<string, string>>({});
  lookupResults = signal<Record<string, Option[]>>({});
  openLookup = signal<string | null>(null);
  private timer: ReturnType<typeof setTimeout> | undefined;

  async ngOnInit(): Promise<void> {
    const r = this.req();
    this.fields = r.fields.filter((f) => !(f.createOnly && r.isEdit));
    const controls: Record<string, FormControl> = {};
    for (const f of this.fields) {
      const v = r.values[f.key];
      const initial = f.type === 'switch' ? (v ?? true) : (v ?? '');
      controls[f.key] = new FormControl(initial, this.validators(f));
      if (f.type === 'lookup') this.lookupText.update((m) => ({ ...m, [f.key]: r.values[f.key + 'Label'] ?? '' }));
    }
    this.form = new FormGroup(controls);
    // Load select options after the form exists so the dialog opens instantly.
    const loaded: Record<string, Option[]> = {};
    await Promise.all(this.fields.filter((f) => f.type === 'select').map(async (f) => {
      try { loaded[f.key] = f.options ?? (await f.optionsLoader?.()) ?? []; } catch { loaded[f.key] = []; }
    }));
    this.selectOptions.set(loaded);
    // Re-apply values for selects now that their options exist.
    for (const f of this.fields) if (f.type === 'select' && r.values[f.key] !== undefined) this.form.controls[f.key].setValue(r.values[f.key]);
    queueMicrotask(() => (this.host.nativeElement as HTMLElement).querySelector<HTMLElement>('input,select,textarea')?.focus());
  }

  private validators(f: FormField): ValidatorFn[] {
    const v: ValidatorFn[] = [];
    if (f.required && f.type !== 'switch') v.push(Validators.required);
    if (f.min != null) v.push(Validators.min(f.min));
    if (f.max != null) v.push(Validators.max(f.max));
    if (f.minlength) v.push(Validators.minLength(f.minlength));
    return v;
  }

  onLookup(f: FormField, text: string): void {
    this.form!.controls[f.key].setValue('');
    this.lookupText.update((m) => ({ ...m, [f.key]: text }));
    clearTimeout(this.timer);
    this.timer = setTimeout(async () => {
      let rows: Option[] = [];
      try { rows = (await f.source?.(text)) ?? []; } catch { /* leave empty */ }
      this.lookupResults.update((m) => ({ ...m, [f.key]: rows.slice(0, 8) }));
      this.openLookup.set(f.key);
    }, 200);
  }

  pick(f: FormField, o: Option): void {
    this.form!.controls[f.key].setValue(o.value);
    this.lookupText.update((m) => ({ ...m, [f.key]: o.label }));
    this.openLookup.set(null);
  }

  @HostListener('document:click', ['$event'])
  onDocClick(e: Event): void {
    if (this.openLookup() && !(e.target as HTMLElement).closest('.lookup')) this.openLookup.set(null);
  }

  onBackdrop(e: MouseEvent): void { if (e.target === e.currentTarget) this.close(false); }
  close(saved: boolean): void { this.dialogs.close(this.req(), saved); }

  async save(): Promise<void> {
    if (!this.form || this.busy()) return;
    this.submitted.set(true);
    if (this.form.invalid) {
      (this.host.nativeElement as HTMLElement).querySelector<HTMLElement>('.field.bad input, .field.bad select, .field.bad textarea')?.focus();
      return;
    }
    const raw = this.form.getRawValue();
    const values: any = {};
    for (const f of this.fields) { const x = raw[f.key]; values[f.key] = typeof x === 'string' ? x.trim() : x; }
    this.busy.set(true);
    try {
      await this.req().submit(values);
      this.close(true);
    } catch (e) {
      this.toast.err((e as Error).message);
      this.busy.set(false);
    }
  }
}
