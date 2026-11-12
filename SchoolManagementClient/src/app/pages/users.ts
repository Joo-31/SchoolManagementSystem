import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ApiService } from '../core/api.service';
import { DialogService } from '../core/dialog.service';
import { useLoad } from '../core/load';
import { AppUser } from '../core/models';
import { ToastService } from '../core/toast.service';
import { AvatarComponent, EmptyComponent, IconComponent, StateComponent, LocalizedDatePipe } from '../shared/primitives';

@Component({
  selector: 'app-users-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [LocalizedDatePipe, StateComponent, EmptyComponent, IconComponent, AvatarComponent],
  template: `
    <div class="page-head"><div class="grow"><p>Accounts for students, teachers and admins. Admin passwords can’t be reset here.</p></div></div>
    <div class="toolbar">
      <div class="search"><app-icon name="search" [size]="16" /><input type="search" placeholder="Search accounts…" aria-label="Search accounts" (input)="q.set($any($event.target).value.toLowerCase())" /></div>
    </div>
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      <div class="table-wrap">
        @if (rows().length) {
          <div class="scroll-x">
            <table>
              <thead><tr><th>Account</th><th>Role</th><th>Linked record</th><th>Created</th><th></th></tr></thead>
              <tbody>
                @for (u of rows(); track u.id) {
                  <tr>
                    <td><div class="cell-person"><app-avatar [name]="u.username" /><div><b>{{ u.username }}</b></div></div></td>
                    <td><span class="badge" [class]="u.role === 'Admin' ? 'warn' : u.role === 'Teacher' ? 'brand' : 'ok'">{{ u.role }}</span></td>
                    <td>{{ u.teacherId ? 'Teacher #' + u.teacherId : u.studentId ? 'Student #' + u.studentId : '—' }}</td>
                    <td class="muted">{{ u.createdAt | localizedDate: 'd MMM y' }}</td>
                    <td class="actions">@if (u.role !== 'Admin') { <button class="btn sm" (click)="reset(u)"><app-icon name="key" [size]="14" />Reset password</button> }</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        } @else { <app-empty title="No accounts match" /> }
      </div>
    </app-state>`,
})
export class UsersPageComponent {
  private api = inject(ApiService);
  private dialogs = inject(DialogService);
  private toast = inject(ToastService);
  d = useLoad(() => this.api.get<AppUser[]>('/users'));
  q = signal('');
  rows = computed(() => (this.d.data() ?? []).filter((u) => u.username.toLowerCase().includes(this.q()) || u.role.toLowerCase().includes(this.q())));

  async reset(u: AppUser): Promise<void> {
    const saved = await this.dialogs.form({
      title: `Reset password for ${u.username}`, submitLabel: 'Reset password',
      fields: [{ key: 'newPassword', label: 'New password', type: 'password', required: true, minlength: 6, hint: 'At least 6 characters.' }],
      submit: async (v) => { await this.api.put(`/users/${u.id}/reset-password`, { newPassword: v.newPassword }); },
    });
    if (saved) this.toast.ok('Password reset');
  }
}
