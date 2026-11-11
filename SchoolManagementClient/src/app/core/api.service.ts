import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Paged } from './models';

/** Thin promise-based wrapper. Paths are relative ("/students"); the interceptor adds the base URL. */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);

  get<T = any>(path: string): Promise<T> { return firstValueFrom(this.http.get<T>(path)); }
  post<T = any>(path: string, body: unknown = {}): Promise<T> { return firstValueFrom(this.http.post<T>(path, body)); }
  put<T = any>(path: string, body: unknown = {}): Promise<T> { return firstValueFrom(this.http.put<T>(path, body)); }
  delete<T = any>(path: string): Promise<T> { return firstValueFrom(this.http.delete<T>(path)); }

  /** Walks every page of a paged endpoint (the API caps page size at 50). */
  async allPaged<T>(path: string, maxPages = 40): Promise<T[]> {
    const out: T[] = [];
    const sep = path.includes('?') ? '&' : '?';
    let page = 1, total = 1;
    while (page <= total && page <= maxPages) {
      const r = await this.get<Paged<T>>(`${path}${sep}pageNumber=${page}&pageSize=50`);
      out.push(...r.data);
      total = r.totalPages;
      page++;
    }
    return out;
  }
}
