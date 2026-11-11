import { Injectable, signal } from '@angular/core';

export interface ToastItem { id: number; msg: string; type: 'ok' | 'err'; out: boolean; }

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly items = signal<ToastItem[]>([]);
  private seq = 0;

  ok(msg: string): void { this.push(msg, 'ok'); }
  err(msg: string): void { this.push(msg, 'err'); }

  private push(msg: string, type: 'ok' | 'err'): void {
    const id = ++this.seq;
    this.items.update((l) => [...l, { id, msg, type, out: false }]);
    setTimeout(() => {
      this.items.update((l) => l.map((t) => (t.id === id ? { ...t, out: true } : t)));
      setTimeout(() => this.items.update((l) => l.filter((t) => t.id !== id)), 250);
    }, type === 'ok' ? 3200 : 5500);
  }
}
