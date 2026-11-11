import { Injectable, signal } from '@angular/core';
import { FormField } from './models';

export interface FormDialogReq {
  kind: 'form';
  id: number;
  title: string;
  fields: FormField[];
  values: any;
  isEdit: boolean;
  submitLabel: string;
  wide: boolean;
  submit: (values: any) => Promise<void>;
  resolve: (saved: boolean) => void;
}
export interface ConfirmDialogReq {
  kind: 'confirm';
  id: number;
  title: string;
  text: string;
  confirm: string;
  danger: boolean;
  resolve: (ok: boolean) => void;
}
export type DialogReq = FormDialogReq | ConfirmDialogReq;

@Injectable({ providedIn: 'root' })
export class DialogService {
  readonly stack = signal<DialogReq[]>([]);
  private seq = 0;

  /** Resolves true if the user confirmed. */
  confirm(o: { title: string; text: string; confirm?: string; danger?: boolean }): Promise<boolean> {
    return new Promise((resolve) => this.open({ kind: 'confirm', id: ++this.seq, title: o.title, text: o.text, confirm: o.confirm ?? 'Delete', danger: o.danger ?? true, resolve }));
  }

  /** Opens a form. `submit` runs inside the dialog: if it throws, the dialog stays open. Resolves true if saved. */
  form(o: { title: string; fields: FormField[]; values?: any; isEdit?: boolean; submitLabel: string; wide?: boolean; submit: (v: any) => Promise<void> }): Promise<boolean> {
    return new Promise((resolve) => this.open({ kind: 'form', id: ++this.seq, title: o.title, fields: o.fields, values: o.values ?? {}, isEdit: !!o.isEdit, submitLabel: o.submitLabel, wide: !!o.wide, submit: o.submit, resolve }));
  }

  close(req: DialogReq, result: boolean): void {
    this.stack.update((l) => l.filter((d) => d.id !== req.id));
    req.resolve(result);
  }

  closeAll(): void { [...this.stack()].forEach((d) => this.close(d, false)); }

  private open(req: DialogReq): void { this.stack.update((l) => [...l, req]); }
}
