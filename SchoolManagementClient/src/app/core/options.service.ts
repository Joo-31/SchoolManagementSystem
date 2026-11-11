import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Option, Student } from './models';

/** Cached dropdown/lookup sources shared by forms. Call invalidate() after any create/update/delete. */
@Injectable({ providedIn: 'root' })
export class OptionsService {
  private api = inject(ApiService);
  private cache = new Map<string, Promise<any>>();

  invalidate(): void { this.cache.clear(); }
  private memo<T>(key: string, fn: () => Promise<T>): Promise<T> {
    if (!this.cache.has(key)) this.cache.set(key, fn());
    return this.cache.get(key) as Promise<T>;
  }

  readonly grades = (): Promise<Option[]> => this.memo('grades', async () => (await this.api.get<any[]>('/grades')).map((g) => ({ value: g.id, label: g.name })));
  readonly classes = (): Promise<Option[]> => this.memo('classes', async () => (await this.api.get<any[]>('/classes')).map((c) => ({ value: c.id, label: `${c.name} — ${c.gradeName}` })));
  readonly teachers = (): Promise<Option[]> => this.memo('teachers', async () => (await this.api.get<any[]>('/teachers')).map((t) => ({ value: t.id, label: `${t.name} (${t.specialization})` })));
  readonly courses = (): Promise<Option[]> => this.memo('courses', async () => (await this.api.get<any[]>('/courses')).map((c) => ({ value: c.id, label: `${c.name} — ${c.gradeName}`, teacherId: c.teacherId })));
  // Teacher assignments may be changed from another Admin session; fetch fresh data for each form open.
  readonly myCourses = async (): Promise<Option[]> => (await this.api.get<any[]>('/marks/my-courses')).map((c) => ({ value: c.id, label: `${c.name} — ${c.className || c.gradeName}`, teacherId: c.teacherId }));

  readonly studentSearch = async (q: string): Promise<Option[]> => {
    const rows: Student[] = q && q.length >= 2
      ? await this.api.get<Student[]>('/students/search?keyword=' + encodeURIComponent(q))
      : (await this.api.get<{ data: Student[] }>('/students/paged?pageNumber=1&pageSize=8')).data;
    return rows.map((s) => ({ value: s.id, label: s.fullName, sub: s.className }));
  };

  readonly myStudentSearch = async (q: string): Promise<Option[]> => {
    const all = await this.api.get<Student[]>('/marks/my-students');
    const k = (q || '').toLowerCase();
    return all.filter((s) => s.fullName.toLowerCase().includes(k)).map((s) => ({ value: s.id, label: s.fullName, sub: s.className }));
  };
}
