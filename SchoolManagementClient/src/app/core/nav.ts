import { Role } from './models';

export interface NavItem { path: string; title: string; icon: string; group: string; roles: Role[]; }

export const NAV: NavItem[] = [
  { path: 'dashboard', title: 'Dashboard', icon: 'layout', group: 'Overview', roles: ['Admin'] },
  { path: 'students', title: 'Students', icon: 'cap', group: 'People', roles: ['Admin'] },
  { path: 'teachers', title: 'Teachers', icon: 'teacher', group: 'People', roles: ['Admin'] },
  { path: 'users', title: 'Accounts', icon: 'key', group: 'People', roles: ['Admin'] },
  { path: 'courses', title: 'Courses', icon: 'book', group: 'Academics', roles: ['Admin'] },
  { path: 'classes', title: 'Classes', icon: 'school', group: 'Academics', roles: ['Admin'] },
  { path: 'grades', title: 'Grades', icon: 'layers', group: 'Academics', roles: ['Admin'] },
  { path: 'attendances', title: 'Attendance', icon: 'cal', group: 'Records', roles: ['Admin'] },
  { path: 'marks', title: 'Marks', icon: 'award', group: 'Records', roles: ['Admin'] },
  { path: 'announcements', title: 'Announcements', icon: 'mega', group: 'Communicate', roles: ['Admin'] },
  { path: 'settings', title: 'Settings', icon: 'settings', group: 'School', roles: ['Admin'] },

  { path: 'teacher', title: 'Dashboard', icon: 'layout', group: 'Overview', roles: ['Teacher'] },
  { path: 'my-classes', title: 'My classes', icon: 'school', group: 'Teaching', roles: ['Teacher'] },
  { path: 'my-marks', title: 'My marks', icon: 'award', group: 'Teaching', roles: ['Teacher'] },
  { path: 'my-homework', title: 'Homework', icon: 'clip', group: 'Teaching', roles: ['Teacher'] },
  { path: 'profile', title: 'Profile', icon: 'user', group: 'Account', roles: ['Teacher', 'Student'] },

  { path: 'student', title: 'Dashboard', icon: 'layout', group: 'Overview', roles: ['Student'] },
  { path: 'my-grades', title: 'My marks', icon: 'award', group: 'School', roles: ['Student'] },
  { path: 'my-attendance', title: 'My attendance', icon: 'cal', group: 'School', roles: ['Student'] },
  { path: 'homework', title: 'Homework', icon: 'clip', group: 'School', roles: ['Student'] },
];
