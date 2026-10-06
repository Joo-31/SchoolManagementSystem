import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { StudentsList } from './pages/students-list/students-list';
import { AddStudent } from './pages/add-student/add-student';
import { EditStudent } from './pages/edit-student/edit-student';
import { TeachersList } from './pages/teachers-list/teachers-list';
import { AddTeacher } from './pages/add-teacher/add-teacher';
import { EditTeacher } from './pages/edit-teacher/edit-teacher';
import { CoursesList } from './pages/courses-list/courses-list';
import { AddCourse } from './pages/add-course/add-course';
import { EditCourse } from './pages/edit-course/edit-course';
import { ClassesList } from './pages/classes-list/classes-list';
import { AddClass } from './pages/add-class/add-class';
import { EditClass } from './pages/edit-class/edit-class';
import { GradesList } from './pages/grades-list/grades-list';
import { AddGrade } from './pages/add-grade/add-grade';
import { EditGrade } from './pages/edit-grade/edit-grade';
import { AttendancesList } from './pages/attendances-list/attendances-list';
import { AddAttendance } from './pages/add-attendance/add-attendance';
import { EditAttendance } from './pages/edit-attendance/edit-attendance';
import { MyClasses } from './pages/my-classes/my-classes';
import { ClassStudents } from './pages/class-students/class-students';
import { MyProfile } from './pages/my-profile/my-profile';
import { MyProfileStudent } from './pages/my-profile-student/my-profile-student';
import { MyAttendance } from './pages/my-attendance/my-attendance';
import { Dashboard } from './pages/dashboard/dashboard';
import { MarksList } from './pages/marks-list/marks-list';
import { AddMark } from './pages/add-mark/add-mark';
import { EditMark } from './pages/edit-mark/edit-mark';
import { MyMarks } from './pages/my-marks/my-marks';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },


  
  // Students
  { path: 'students', component: StudentsList },
  { path: 'students/add', component: AddStudent },
  { path: 'students/edit/:id', component: EditStudent },
  { path: 'my-profile-student', component: MyProfileStudent },
  { path: 'my-attendance', component: MyAttendance },
  
  // Teachers
  { path: 'teachers', component: TeachersList },
  { path: 'teachers/add', component: AddTeacher },
  { path: 'teachers/edit/:id', component: EditTeacher },
  { path: 'my-classes', component: MyClasses },
   { path: 'my-classes/:id/students', component: ClassStudents },
    { path: 'my-profile', component: MyProfile },
  
  // Courses
  { path: 'courses', component: CoursesList },
  { path: 'courses/add', component: AddCourse },
  { path: 'courses/edit/:id', component: EditCourse },

  // Classes
  { path: 'classes', component: ClassesList },
  { path: 'classes/add', component: AddClass },
  { path: 'classes/edit/:id', component: EditClass },

  // Grades
  { path: 'grades', component: GradesList },
  { path: 'grades/add', component: AddGrade },
  { path: 'grades/edit/:id', component: EditGrade },

   // Attendances
  { path: 'attendances', component: AttendancesList },
  { path: 'attendances/add', component: AddAttendance },
  { path: 'attendances/edit/:id', component: EditAttendance },

  { path: 'dashboard', component: Dashboard },

  // Marks
  { path: 'marks', component: MarksList },
  { path: 'marks/add', component: AddMark },
  { path: 'marks/edit/:id', component: EditMark },
  { path: 'my-marks', component: MyMarks }
];