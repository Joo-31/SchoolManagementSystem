import { ApiService } from '../core/api.service';
import { Column, CrudConfig, ListFn, ListQuery } from '../core/models';
import { OptionsService } from '../core/options.service';
import { day10, dt, isoDay } from '../core/util';

export interface CrudDeps { api: ApiService; opts: OptionsService; }

const listPaged = (api: ApiService, base: string): ListFn => async ({ page, size, q }: ListQuery) => {
  if (q) {
    const all = await api.get<any[]>(`${base}/search?keyword=${encodeURIComponent(q)}`);
    return { rows: all.slice((page - 1) * size, page * size), total: all.length };
  }
  const r = await api.get<{ data: any[]; totalCount: number }>(`${base}/paged?pageNumber=${page}&pageSize=${size}`);
  return { rows: r.data, total: r.totalCount };
};

const genderOpts = [{ value: 0, label: 'Male' }, { value: 1, label: 'Female' }];
const person = (label: string, value: (r: any) => string, sub?: (r: any) => string): Column => ({ label, type: 'person', value, sub });

function marksConfig({ api, opts }: CrudDeps, isTeacher: boolean): CrudConfig {
  const courses = isTeacher ? opts.myCourses : opts.courses;
  // Server-side paging and search: /marks/paged (teachers are limited to their own marks by the API).
  const list: ListFn = async ({ page, size, q }) => {
    const qs = new URLSearchParams({ pageNumber: String(page), pageSize: String(size) });
    if (q) qs.set('search', q);
    const r = await api.get<{ data: any[]; totalCount: number }>('/marks/paged?' + qs);
    return { rows: r.data, total: r.totalCount };
  };
  return {
    base: '/marks', singular: 'mark', plural: 'marks',
    sub: isTeacher ? 'Marks you’ve entered for your own courses.' : 'Every mark across all courses.',
    list,
    columns: [
      person('Student', (r) => r.studentName),
      { label: 'Course', value: (r) => r.courseName },
      { label: 'Score', type: 'stamp', value: (r) => r.score },
      { label: 'Date', type: 'date', hide: true, value: (r) => r.date },
      ...(isTeacher ? [] : [{ label: 'Teacher', hide: true, value: (r: any) => r.teacherName } as Column]),
      { label: 'Notes', type: 'muted', hide: true, value: (r) => r.notes },
    ],
    fields: [
      { key: 'studentId', label: 'Student', type: 'lookup', source: isTeacher ? opts.myStudentSearch : opts.studentSearch, required: true, full: true, createOnly: true },
      { key: 'courseId', label: 'Course', type: 'select', optionsLoader: courses, required: true, full: true, createOnly: true },
      { key: 'score', label: 'Score (0–100)', type: 'number', min: 0, max: 100, step: 0.5, required: true },
      { key: 'date', label: 'Date', type: 'date', required: true },
      { key: 'notes', label: 'Notes', type: 'textarea', full: true },
    ],
    defaults: { date: isoDay() },
    toForm: (r) => ({ score: r.score, date: day10(r.date), notes: r.notes || '' }),
    toPayload: async (v, isEdit, row) => {
      if (isEdit) return { studentId: row.studentId, courseId: row.courseId, teacherId: row.teacherId, score: v.score, date: dt(v.date), notes: v.notes };
      const c = (await courses()).find((x) => x.value === v.courseId);
      return { studentId: v.studentId, courseId: v.courseId, teacherId: c?.teacherId ?? 0, score: v.score, date: dt(v.date), notes: v.notes };
    },
  };
}

export const CRUD_CONFIGS: Record<string, (d: CrudDeps) => CrudConfig> = {
  students: ({ api, opts }) => ({
    base: '/students', singular: 'student', plural: 'students', sub: 'Everyone enrolled this term.', wide: true,
    list: listPaged(api, '/students'),
    columns: [
      person('Student', (r) => r.fullName, (r) => 'ID ' + r.id),
      { label: 'Class', type: 'badge', tone: 'brand', value: (r) => r.className || '—' },
      { label: 'Age', hide: true, value: (r) => r.age },
      { label: 'Gender', hide: true, value: (r) => r.gender },
    ],
    fields: [
      { key: 'firstName', label: 'First name', required: true },
      { key: 'lastName', label: 'Last name', required: true },
      { key: 'birthDate', label: 'Date of birth', type: 'date', required: true },
      { key: 'gender', label: 'Gender', type: 'select', options: genderOpts, required: true },
      { key: 'classId', label: 'Class', type: 'select', optionsLoader: opts.classes, required: true, full: true },
      { key: 'username', label: 'Username', required: true, createOnly: true, hint: 'The student signs in with this.' },
      { key: 'password', label: 'Password', type: 'password', required: true, createOnly: true, minlength: 6 },
    ],
    defaults: { gender: 0 },
    toForm: (r) => ({ firstName: r.firstName, lastName: r.lastName, birthDate: day10(r.birthDate), gender: r.gender === 'Female' ? 1 : 0, classId: r.classId }),
    toPayload: (v, isEdit) => ({ firstName: v.firstName, lastName: v.lastName, birthDate: dt(v.birthDate), gender: v.gender, classId: v.classId, ...(isEdit ? {} : { username: v.username, password: v.password }) }),
    deleteText: (r) => `${r.fullName} and their login will be removed.`,
  }),

  teachers: ({ api }) => ({
    base: '/teachers', singular: 'teacher', plural: 'teachers', sub: 'Teaching staff and their subjects.', wide: true,
    list: listPaged(api, '/teachers'),
    columns: [
      person('Teacher', (r) => r.name, (r) => r.email),
      { label: 'Subject', type: 'badge', tone: 'brand', value: (r) => r.specialization },
      { label: 'Phone', hide: true, value: (r) => r.phone },
      { label: 'Experience', hide: true, value: (r) => r.yearsOfExperience + ' yrs' },
    ],
    fields: [
      { key: 'name', label: 'Full name', required: true, full: true },
      { key: 'email', label: 'Email', type: 'email', required: true },
      { key: 'phone', label: 'Phone', required: true },
      { key: 'specialization', label: 'Subject', required: true },
      { key: 'hireDate', label: 'Hire date', type: 'date', required: true },
      { key: 'username', label: 'Username', required: true, createOnly: true },
      { key: 'password', label: 'Password', type: 'password', required: true, createOnly: true, minlength: 6 },
    ],
    toForm: (r) => {
      // The API now returns hireDate; fall back to an estimate from years of experience for older servers.
      let hire = day10(r.hireDate);
      if (!hire) { const d = new Date(); d.setFullYear(d.getFullYear() - (r.yearsOfExperience || 0)); hire = isoDay(d); }
      return { name: r.name, email: r.email, phone: r.phone, specialization: r.specialization, hireDate: hire };
    },
    toPayload: (v, isEdit) => ({ name: v.name, email: v.email, phone: v.phone, specialization: v.specialization, hireDate: dt(v.hireDate), ...(isEdit ? {} : { username: v.username, password: v.password }) }),
  }),

  courses: ({ api, opts }) => ({
    base: '/courses', singular: 'course', plural: 'courses', sub: 'Subjects offered, by grade and teacher.',
    list: listPaged(api, '/courses'),
    columns: [
      { label: 'Course', type: 'person', value: (r) => r.name, sub: (r) => r.code },
      { label: 'Grade', type: 'badge', value: (r) => r.gradeName },
      { label: 'Class', value: (r) => r.className || 'Unassigned' },
      { label: 'Teacher', hide: true, value: (r) => r.teacherName },
      { label: 'Credits', hide: true, value: (r) => r.credits },
    ],
    fields: [
      { key: 'name', label: 'Course name', required: true },
      { key: 'code', label: 'Code', required: true },
      { key: 'credits', label: 'Credits', type: 'number', min: 1, max: 10, required: true },
      { key: 'gradeId', label: 'Grade', type: 'select', optionsLoader: opts.grades, required: true },
      { key: 'classId', label: 'Class', type: 'select', optionsLoader: opts.classes, required: true },
      { key: 'teacherId', label: 'Subject teacher', type: 'select', optionsLoader: opts.teachers, required: true, full: true },
    ],
    toForm: (r) => ({ name: r.name, code: r.code, credits: r.credits, gradeId: r.gradeId, classId: r.classId, teacherId: r.teacherId }),
    toPayload: (v) => ({ name: v.name, code: v.code, credits: v.credits, gradeId: v.gradeId, classId: v.classId, teacherId: v.teacherId }),
  }),

  classes: ({ api, opts }) => ({
    base: '/classes', singular: 'class', plural: 'classes', sub: 'Homerooms and who leads them.',
    list: listPaged(api, '/classes'),
    columns: [
      { label: 'Class', value: (r) => r.name },
      { label: 'Grade', type: 'badge', value: (r) => r.gradeName },
      person('Class teacher', (r) => r.classTeacherName),
    ],
    fields: [
      { key: 'name', label: 'Class name', required: true, full: true },
      { key: 'gradeId', label: 'Grade', type: 'select', optionsLoader: opts.grades, required: true },
      { key: 'classTeacherId', label: 'Class teacher', type: 'select', optionsLoader: opts.teachers, required: true },
    ],
    toForm: (r) => ({ name: r.name, gradeId: r.gradeId, classTeacherId: r.classTeacherId }),
    toPayload: (v) => ({ name: v.name, gradeId: v.gradeId, classTeacherId: v.classTeacherId }),
  }),

  grades: ({ api }) => ({
    base: '/grades', singular: 'grade', plural: 'grades', sub: 'Year levels at your school.',
    list: listPaged(api, '/grades'),
    columns: [
      { label: 'Name', value: (r) => r.name },
      { label: 'Level', type: 'badge', tone: 'brand', value: (r) => 'Level ' + r.level },
    ],
    fields: [
      { key: 'name', label: 'Name', required: true },
      { key: 'level', label: 'Level', type: 'number', min: 1, max: 20, required: true },
    ],
    toForm: (r) => ({ name: r.name, level: r.level }),
    toPayload: (v) => ({ name: v.name, level: v.level }),
  }),

  attendances: ({ api, opts }) => ({
    base: '/attendances', singular: 'record', plural: 'records', sub: 'Daily presence, filtered by date, class or status.', searchable: false,
    list: async ({ page, size, filters: f }) => {
      const qs = new URLSearchParams({ pageNumber: String(page), pageSize: String(size) });
      if (f['date']) qs.set('date', f['date']);
      if (f['classId']) qs.set('classId', f['classId']);
      if (f['status']) qs.set('status', f['status']);
      const r = await api.get<{ data: any[]; totalCount: number }>('/attendances/filter?' + qs);
      return { rows: r.data, total: r.totalCount };
    },
    filters: [
      { key: 'date', label: 'Date', type: 'date' },
      { key: 'classId', label: 'All classes', type: 'select', optionsLoader: opts.classes },
      { key: 'status', label: 'Any status', type: 'select', options: [{ value: 'present', label: 'Present' }, { value: 'absent', label: 'Absent' }] },
    ],
    columns: [
      person('Student', (r) => r.studentName),
      { label: 'Class', type: 'badge', value: (r) => r.className },
      { label: 'Date', type: 'date', value: (r) => r.date },
      { label: 'Status', type: 'status', value: (r) => r.isPresent },
    ],
    fields: [
      { key: 'studentId', label: 'Student', type: 'lookup', source: opts.studentSearch, required: true, full: true, createOnly: true },
      { key: 'classId', label: 'Class', type: 'select', optionsLoader: opts.classes, required: true },
      { key: 'date', label: 'Date', type: 'date', required: true },
      { key: 'isPresent', label: 'Present', type: 'switch' },
    ],
    defaults: { date: isoDay(), isPresent: true },
    toForm: (r) => ({ classId: r.classId, date: day10(r.date), isPresent: r.isPresent }),
    toPayload: (v, isEdit, row) => ({ studentId: isEdit ? row.studentId : v.studentId, classId: v.classId, date: dt(v.date), isPresent: v.isPresent }),
  }),

  marks: (d) => marksConfig(d, false),
  teacherMarks: (d) => marksConfig(d, true),
};
