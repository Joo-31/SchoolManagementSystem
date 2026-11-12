export type Role = 'Admin' | 'Teacher' | 'Student';

export interface GradeBand { label: string; minPercent: number; }
export interface SchoolTerm { name: string; startDate: string; endDate: string; }
export interface SchoolSettings {
  schoolName: string; shortName: string; logo: string | null; tagline: string; address: string; phone: string; email: string; website: string;
  academicYear: string; terms: SchoolTerm[]; weekStartsOn: number; passMark: number; gradeBands: GradeBand[];
  lateThresholdMinutes: number; countExcusedAbsences: boolean; language: 'English' | 'Arabic'; dateFormat: string; timezone: string;
  defaultTheme: 'light' | 'dark' | 'system'; accentColor: string; apiMode: 'demo' | 'live'; apiUrl: string;
}

export interface Session {
  token: string;
  role: Role;
  username: string;
  teacherId: number | null;
  studentId: number | null;
  expires: number | null;
}

export interface Paged<T> { data: T[]; pageNumber: number; pageSize: number; totalCount: number; totalPages: number; }

export interface Student { id: number; firstName: string; lastName: string; fullName: string; birthDate: string; age: number; gender: string; classId: number; className: string; }
export interface Teacher { id: number; name: string; email: string; phone: string; specialization: string; hireDate: string; yearsOfExperience: number; }
export interface Course { id: number; name: string; code: string; credits: number; gradeId: number; gradeName: string; teacherId: number; teacherName: string; }
export interface SchoolClass { id: number; name: string; gradeId: number; gradeName: string; classTeacherId: number; classTeacherName: string; }
export interface Grade { id: number; name: string; level: number; }
export interface Attendance { id: number; studentId: number; studentName: string; classId: number; className: string; date: string; isPresent: boolean; }
export interface Mark { id: number; studentId: number; studentName: string; courseId: number; courseName: string; teacherId: number; teacherName: string; score: number; date: string; notes: string; }
export interface Homework { id: number; title: string; description: string; link: string; courseName: string; teacherName: string; createdAt: string; }
export interface AppNotification { id: number; title: string; message: string; targetRole: string; createdAt: string; }
export interface AppUser { id: number; username: string; role: Role; studentId: number | null; teacherId: number | null; createdAt: string; }

export class ApiError extends Error {
  constructor(message: string, public status = 0, public network = false) {
    super(message);
  }
}

/* ---- dialog / form / table configuration ---- */
export interface Option { value: any; label: string; sub?: string; teacherId?: number; }

export interface FormField {
  key: string;
  label: string;
  type?: 'text' | 'email' | 'password' | 'number' | 'date' | 'url' | 'file' | 'select' | 'textarea' | 'switch' | 'lookup';
  required?: boolean;
  options?: Option[];
  optionsLoader?: () => Promise<Option[]>;
  source?: (q: string) => Promise<Option[]>;
  hint?: string;
  placeholder?: string;
  min?: number; max?: number; step?: number; minlength?: number;
  full?: boolean;
  createOnly?: boolean;
}

export interface Column {
  label: string;
  hide?: boolean;
  type?: 'text' | 'person' | 'badge' | 'stamp' | 'date' | 'status' | 'muted';
  value: (r: any) => any;
  sub?: (r: any) => string;
  tone?: string;
}

export interface ListQuery { page: number; size: number; q: string; filters: Record<string, string>; }
export interface ListResult { rows: any[]; total: number; }
export type ListFn = ((q: ListQuery) => Promise<ListResult>) & { reset?: () => void };

export interface FilterDef { key: string; label: string; type?: 'date' | 'select'; options?: Option[]; optionsLoader?: () => Promise<Option[]>; }

export interface CrudConfig {
  base: string;
  singular: string;
  plural: string;
  sub: string;
  list: ListFn;
  columns: Column[];
  fields: FormField[];
  wide?: boolean;
  searchable?: boolean;
  filters?: FilterDef[];
  defaults?: any;
  toForm: (row: any) => any;
  toPayload: (v: any, isEdit: boolean, row?: any) => any | Promise<any>;
  deleteText?: (row: any) => string;
}
