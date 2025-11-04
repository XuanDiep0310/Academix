export interface ClassItem {
  id: string;
  name: string;
  teacherId: string;
  studentCount: number;
}
export interface StudentItem {
  id: string;
  name: string;
  email: string;
  classId: string;
}
export interface ExamItem {
  id: string;
  title: string;
  classId: string;
  className: string;
  type: "multiple-choice" | "essay" | "mixed";
  status: "upcoming" | "ongoing" | "completed";
  totalPoints: number;
  startTime?: string;
}
export interface QuestionItem {
  id: string;
  type: "multiple-choice" | "essay" | "fill-blank" | "matching";
  content: string;
  options?: string[];
  correctAnswer?: number;
  points: number;
  category?: string;
}
export interface DocumentItem {
  id: string;
  title: string;
  type: "pdf" | "video" | "article";
  classId: string;
  version: number;
  uploadedAt: string;
  uploadedBy: string;
}
export interface ExamResult {
  id: string;
  examId: string;
  studentId: string;
  score: number;
  totalPoints: number;
  completedAt: string;
  focusLossCount: number;
  copyPasteCount: number;
}
export interface HighRiskStudent {
  studentName: string;
  examTitle: string;
  focusLossCount: number;
  copyPasteCount: number;
  severity: "low" | "medium" | "high" | "critical";
}
export interface TeacherStats {
  totalClasses: number;
  totalStudents: number;
  activeExams: number;
  pendingGrading: number;
  highRiskCount: number;
}
