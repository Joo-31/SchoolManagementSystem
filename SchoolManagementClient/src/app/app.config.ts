import { ApplicationConfig, importProvidersFrom, provideAppInitializer, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth-interceptor';
import { firstValueFrom } from 'rxjs';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

// ✅ دالة بتحمل الترجمة قبل ما التطبيق يشتغل
export function initTranslations() {
  const translate = inject(TranslateService);
  translate.setDefaultLang('en');
  const savedLang = localStorage.getItem('lang') || 'en';
  return firstValueFrom(translate.use(savedLang)); // بنستنى لحد ما الترجمة تتحمل
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'en',
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    ),
    // ✅ السطر ده هو اللي هيخلي الترجمة تشتغل
    provideAppInitializer(initTranslations)
  ]
};