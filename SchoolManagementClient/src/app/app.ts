import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DialogHostComponent, ToastHostComponent } from './shared/overlays';
import { LanguageService } from './core/language.service';

@Component({
  selector: 'app-root',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, ToastHostComponent, DialogHostComponent],
  template: `<router-outlet /><app-toast-host /><app-dialog-host />`,
})
export class App { private readonly language = inject(LanguageService); }
