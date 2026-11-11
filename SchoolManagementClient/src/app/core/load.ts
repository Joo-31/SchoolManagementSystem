import { signal } from '@angular/core';

/** Tiny async-state helper: call it in a field initializer; it loads immediately. */
export function useLoad<T>(fn: () => Promise<T>) {
  const data = signal<T | null>(null);
  const loading = signal(true);
  const error = signal<Error | null>(null);
  const reload = async (): Promise<void> => {
    loading.set(true);
    error.set(null);
    try { data.set(await fn()); }
    catch (e) { error.set(e as Error); }
    finally { loading.set(false); }
  };
  void reload();
  return { data, loading, error, reload };
}
