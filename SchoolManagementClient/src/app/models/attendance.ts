export interface Attendance {
   id: number;
  studentId: number;
  studentName: string;   // ← الجديد
  classId: number;
  className: string;     // ← الجديد
  date: string;
  isPresent: boolean;
}