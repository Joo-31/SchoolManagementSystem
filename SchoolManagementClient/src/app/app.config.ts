import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { apiInterceptor } from './core/api.interceptor';
import { provideAppInitializer, inject } from '@angular/core';
import { SchoolSettingsService } from './core/school-settings.service';
import { IMAGE_CONFIG } from '@angular/common';
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([apiInterceptor])),
    provideAppInitializer(() => inject(SchoolSettingsService).load()),
    // ✅ ضيفنا الـ Config ده عشان نشيل الـ Warning
    {
      provide: IMAGE_CONFIG,
      useValue: {
        disableImageSizeWarning: true,
        disableImageLazyLoadWarning: true
      }
    }
  ],
};
