import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ApiService } from '../core/api.service';
import { AuthService } from '../core/auth.service';
import { useLoad } from '../core/load';
import { Student, Teacher } from '../core/models';
import { AvatarComponent, StateComponent, LocalizedDatePipe } from '../shared/primitives';

@Component({
  selector: 'app-profile',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [StateComponent, AvatarComponent, LocalizedDatePipe],
  template: `
    <app-state [loading]="d.loading()" [error]="d.error()" (retry)="d.reload()">
      @if (d.data(); as x) {
        <div class="hero">
          <app-avatar [name]="x.name" [size]="72" />
          <div class="grow"><h2>{{ x.name }}</h2><p>{{ session?.role }} · &#64;{{ session?.username }}</p></div>
        </div>
        <div class="panel" style="margin-top:18px;max-width:640px">
          <header><h3>Details</h3></header>
          <div class="feed">
            @for (r of x.rows; track r.label) {
              <div class="item" style="grid-template-columns:140px 1fr;display:grid"><span class="muted">{{ r.label }}</span><b>{{ r.date ? (r.value | localizedDate: 'd MMM y') : (r.value || '—') }}</b></div>
            }
          </div>
        </div>
      }
    </app-state>`,
})
export class ProfileComponent {
  private api = inject(ApiService);
  readonly session = inject(AuthService).session();

  d = useLoad(async () => {
    if (this.session?.role === 'Teacher') {
      const me = await this.api.get<Teacher>('/teachers/me');
      return { name: me.name, rows: [
        { label: 'Subject', value: me.specialization }, { label: 'Email', value: me.email },
        { label: 'Phone', value: me.phone }, { label: 'Experience', value: `${me.yearsOfExperience} years` },
      ] };
    }
    const me = await this.api.get<Student>('/students/me');
    return { name: me.fullName, rows: [
        { label: 'Class', value: me.className }, { label: 'Date of birth', value: me.birthDate, date: true },
      { label: 'Age', value: String(me.age) }, { label: 'Gender', value: me.gender },
    ] };
  });
}
