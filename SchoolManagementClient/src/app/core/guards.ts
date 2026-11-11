import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { Role } from './models';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.session() ? true : inject(Router).createUrlTree(['/login']);
};

export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.session() ? inject(Router).createUrlTree([auth.home()]) : true;
};

export const homeGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return inject(Router).createUrlTree([auth.session() ? auth.home() : '/login']);
};

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const s = auth.session();
  if (!s) return router.createUrlTree(['/login']);
  const roles = (route.data['roles'] ?? []) as Role[];
  return roles.includes(s.role) ? true : router.createUrlTree([auth.home()]);
};
