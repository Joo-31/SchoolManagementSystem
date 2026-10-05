export interface Class {
  id: number;
  name: string;
  gradeId: number;
  gradeName: string;         // ← الجديد
  classTeacherId: number;
  classTeacherName: string;
}