import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DialogHostComponent, ToastHostComponent } from './shared/overlays';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, ToastHostComponent, DialogHostComponent],
  template: `<router-outlet /><app-toast-host /><app-dialog-host />`,
})
export class App {}
