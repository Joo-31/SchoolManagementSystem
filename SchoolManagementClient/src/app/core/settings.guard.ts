import { CanDeactivateFn } from '@angular/router';
import type { SettingsPageComponent } from '../pages/settings';

export const settingsCanDeactivate: CanDeactivateFn<SettingsPageComponent> = (component) => component.confirmDiscard();
