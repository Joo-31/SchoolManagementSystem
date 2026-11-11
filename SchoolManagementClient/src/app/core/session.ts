import { Role, Session } from './models';

const ROLE_KEY = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAME_KEY = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/name';

export const store = {
  get: (k: string): string | null => { try { return localStorage.getItem('cl.' + k); } catch { return null; } },
  set: (k: string, v: string | null): void => { try { v == null ? localStorage.removeItem('cl.' + k) : localStorage.setItem('cl.' + k, v); } catch { /* storage unavailable */ } },
};

export function decodeJwt(token: string): any | null {
  try {
    const b = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(Array.from(atob(b + '='.repeat((4 - (b.length % 4)) % 4)), (c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0')).join(''));
    return JSON.parse(json);
  } catch { return null; }
}

/** The session is read from the JWT itself, so it does not depend on the shape of the login response. */
export function readSession(): Session | null {
  const token = store.get('token');
  if (!token) return null;
  const p = decodeJwt(token);
  if (!p) return null;
  if (p.exp && p.exp * 1000 < Date.now()) { store.set('token', null); return null; }
  return {
    token,
    role: (p[ROLE_KEY] || p['role']) as Role,
    username: p[NAME_KEY] || p['unique_name'] || p['name'] || store.get('username') || 'user',
    teacherId: p['TeacherId'] ? +p['TeacherId'] : null,
    studentId: p['StudentId'] ? +p['StudentId'] : null,
    expires: p.exp ? p.exp * 1000 : null,
  };
}
