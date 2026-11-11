import { HttpErrorResponse, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, from, map, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { createMock } from './mock-api';
import { ApiError } from './models';
import { readSession } from './session';
import { SettingsService } from './settings.service';

let mock: ReturnType<typeof createMock> | undefined;

/**
 * - Only touches API-relative URLs (those starting with "/").
 * - Demo mode: answers from the in-browser mock instead of the network.
 * - Live mode: prefixes the API base URL, adds the Bearer token, turns failures into ApiError.
 */
export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith('/')) return next(req);
  const settings = inject(SettingsService);
  const auth = inject(AuthService);
  const session = readSession();

  if (settings.mode() === 'demo') {
    mock ??= createMock();
    return from(mock(req.method, req.url, req.body, session)).pipe(
      map((body) => new HttpResponse({ status: 200, body })),
      catchError((e) => throwError(() => new ApiError(e.message, e.status || 400))),
    );
  }

  const authed = req.clone({
    url: settings.apiUrl() + req.url,
    setHeaders: session ? { Authorization: 'Bearer ' + session.token } : {},
  });
  return next(authed).pipe(
    catchError((e: HttpErrorResponse) => {
      if (e.status === 401 && req.url !== '/auth/login') auth.expire();
      return throwError(() => toApiError(e, settings.apiUrl()));
    }),
  );
};

function toApiError(e: HttpErrorResponse, apiUrl: string): ApiError {
  if (e.status === 0) {
    return new ApiError(`Can't reach the API at ${apiUrl}. Check that it's running and that your browser trusts its HTTPS certificate.`, 0, true);
  }
  const d = e.error;
  let msg: string | undefined = typeof d === 'string' ? d : d?.message || d?.title || d?.details;
  if (d?.details && d?.message) msg = `${d.message}: ${d.details}`;
  if (!msg) msg = e.status === 403 ? "You don't have permission to do that." : e.status === 401 ? 'Your session has expired. Sign in again.' : `Request failed (${e.status})`;
  return new ApiError(msg, e.status);
}
