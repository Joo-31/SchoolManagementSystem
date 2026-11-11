import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { DialogService } from '../core/dialog.service';
import { useLoad } from '../core/load';
import { AppNotification } from '../core/models';
import { ToastService } from '../core/toast.service';
import { EmptyComponent, IconComponent, StateComponent, TimeAgoPipe } from '../shared/primitives';

@Component({
  selector: 'app-announcements-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, StateComponent, EmptyComponent, IconComponent, TimeAgoPipe],
  template: `
    <div class="page-head"><div class="grow"><p>Tell students, teachers or everyone something important.</p></div></div>
    <div class="grid g-2e">
      <div class="panel">
        <header><h3>New announcement</h3></header>
        <form class="form-grid" [formGroup]="form" (ngSubmit)="send()" novalidate>
          <div class="field full" [class.bad]="submitted() && form.controls.title.invalid"><label for="n_title">Title</label><input id="n_title" formControlName="title" /></div>
          <div class="field full" [class.bad]="submitted() && form.controls.message.invalid"><label for="n_msg">Message</label><textarea id="n_msg" rows="4" formControlName="message"></textarea></div>
          <div class="field"><label for="n_role">Send to</label>
            <select id="n_role" formControlName="targetRole">
              <option value="All">Everyone</option><option value="Student">Students</option><option value="Teacher">Teachers</option>
            </select>
          </div>
        </form>
        <div style="margin-top:18px"><button class="btn primary" [disabled]="busy()" (click)="send()"><app-icon name="mega" [size]="16" />Send announcement</button></div>
      </div>
      <div class="panel">
        <header><h3>Sent announcements</h3></header>
        <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
          <div class="feed">
            @for (n of d.data() ?? []; track n.id) {
              <div class="item">
                <div style="display:flex;gap:10px;align-items:center">
                  <b>{{ n.title }}</b><span class="badge brand">{{ n.targetRole === 'All' ? 'Everyone' : n.targetRole + 's' }}</span><span style="margin-left:auto"></span>
                  <button class="icon-btn danger" aria-label="Delete announcement" (click)="remove(n)"><app-icon name="trash" [size]="16" /></button>
                </div>
                <p class="muted">{{ n.message }}</p><div class="meta">{{ n.createdAt | timeAgo }}</div>
              </div>
            } @empty { <app-empty title="No announcements yet" text="Send one and it appears in everyone’s bell menu." /> }
          </div>
        </app-state>
      </div>
    </div>`,
})
export class AnnouncementsPageComponent {
  private api = inject(ApiService);
  private dialogs = inject(DialogService);
  private toast = inject(ToastService);
  d = useLoad(() => this.api.get<AppNotification[]>('/notifications'));
  busy = signal(false);
  submitted = signal(false);
  form = new FormGroup({
    title: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    message: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    targetRole: new FormControl('All', { nonNullable: true }),
  });

  async send(): Promise<void> {
    this.submitted.set(true);
    if (this.form.invalid || this.busy()) return;
    this.busy.set(true);
    try {
      await this.api.post('/notifications', this.form.getRawValue());
      this.toast.ok('Announcement sent');
      this.form.reset({ title: '', message: '', targetRole: 'All' });
      this.submitted.set(false);
      void this.d.reload();
    } catch (e) { this.toast.err((e as Error).message); }
    finally { this.busy.set(false); }
  }

  async remove(n: AppNotification): Promise<void> {
    if (!(await this.dialogs.confirm({ title: 'Delete announcement?', text: `“${n.title}” will disappear for everyone.` }))) return;
    try { await this.api.delete('/notifications/' + n.id); this.toast.ok('Announcement deleted'); void this.d.reload(); }
    catch (e) { this.toast.err((e as Error).message); }
  }
}
