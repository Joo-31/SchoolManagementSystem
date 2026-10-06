export interface Mark {
  id: number;
  studentId: number;
  studentName: string;
  courseId: number;
  courseName: string;
  teacherId: number;
  teacherName: string;
  score: number;
  date: string;
  notes?: string;
}