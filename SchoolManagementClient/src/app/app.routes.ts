import { Routes } from '@angular/router';
import { authGuard, guestGuard, homeGuard, roleGuard } from './core/guards';
import { Role } from './core/models';
import { settingsCanDeactivate } from './core/settings.guard';

const A: Role[] = ['Admin'];
const T: Role[] = ['Teacher'];
const S: Role[] = ['Student'];

/** A guarded child route. `title` feeds both the browser tab and the top bar. */
const page = (path: string, title: string, roles: Role[], loadComponent: Routes[number]['loadComponent'], data: Record<string, unknown> = {}) => ({
  path,
  title: `${title} — Chalkline`,
  canActivate: [roleGuard],
  data: { title, roles, ...data },
  loadComponent,
});
const crud = () => import('./shared/crud-page').then((m) => m.CrudPageComponent);

export const routes: Routes = [
  { path: 'login', title: 'Sign in — Chalkline', canActivate: [guestGuard], loadComponent: () => import('./pages/login').then((m) => m.LoginComponent) },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell').then((m) => m.ShellComponent),
    children: [
      { path: '', pathMatch: 'full', canActivate: [homeGuard], children: [] },

      // Admin
      page('dashboard', 'Dashboard', A, () => import('./pages/admin-dashboard').then((m) => m.AdminDashboardComponent)),
      page('students', 'Students', A, crud, { entity: 'students' }),
      page('teachers', 'Teachers', A, crud, { entity: 'teachers' }),
      page('users', 'Accounts', A, () => import('./pages/users').then((m) => m.UsersPageComponent)),
      page('courses', 'Courses', A, crud, { entity: 'courses' }),
      page('classes', 'Classes', A, crud, { entity: 'classes' }),
      page('grades', 'Grades', A, crud, { entity: 'grades' }),
      page('attendances', 'Attendance', A, crud, { entity: 'attendances' }),
      page('marks', 'Marks', A, crud, { entity: 'marks' }),
      page('announcements', 'Announcements', A, () => import('./pages/announcements').then((m) => m.AnnouncementsPageComponent)),
      { ...page('settings', 'Settings', A, () => import('./pages/settings').then((m) => m.SettingsPageComponent)), canDeactivate: [settingsCanDeactivate] },

      // Teacher
      page('teacher', 'Dashboard', T, () => import('./pages/teacher-pages').then((m) => m.TeacherDashboardComponent)),
      page('my-classes', 'My classes', T, () => import('./pages/teacher-pages').then((m) => m.MyClassesComponent)),
      page('my-classes/:id', 'Attendance', T, () => import('./pages/teacher-pages').then((m) => m.ClassRosterComponent)),
      page('my-marks', 'My marks', T, crud, { entity: 'teacherMarks' }),
      page('my-homework', 'Homework', T, () => import('./pages/teacher-pages').then((m) => m.TeacherHomeworkComponent)),

      // Student
      page('student', 'Dashboard', S, () => import('./pages/student-pages').then((m) => m.StudentDashboardComponent)),
      page('my-grades', 'My marks', S, () => import('./pages/student-pages').then((m) => m.StudentMarksComponent)),
      page('my-attendance', 'My attendance', S, () => import('./pages/student-pages').then((m) => m.StudentAttendanceComponent)),
      page('homework', 'Homework', S, () => import('./pages/student-pages').then((m) => m.StudentHomeworkComponent)),

      // Teacher + student
      page('profile', 'Profile', [...T, ...S], () => import('./pages/profile').then((m) => m.ProfileComponent)),
    ],
  },
  { path: '**', redirectTo: '' },
];
