// src/data/seed.ts

// =======================
// INTERFACES (theo bạn)
// =======================
export interface ClassInfo {
  id: string;
  name: string;
}

export interface Document {
  id: string;
  title: string;
  type: "pdf" | "video" | "article";
  className: string;
  uploadedBy: string;
  uploadedAt: string;
  viewed: boolean;
  version: number;
  size?: string;
  duration?: string;
}

export interface Exam {
  id: string;
  title: string;
  className: string;
  startTime: string;
  duration: number;
  totalQuestions: number;
  type: "multiple-choice" | "essay" | "mixed";
  status: "ongoing" | "upcoming" | "completed";
}

export interface Question {
  id: string;
  content: string;
  type: "multiple-choice" | "essay" | "fill-blank";
  options?: string[];
  points: number;
}

export interface ExamResult {
  id: string;
  examId: string;
  score: number;
  totalPoints: number;
  completedAt: string;
  focusLossCount: number;
  copyPasteCount: number;
}

export interface QAQuestion {
  id: string;
  title: string;
  content: string;
  author: string;
  className: string;
  timestamp: Date;
  likes: number;
  answers: number;
  status: "open" | "answered";
}

export interface Message {
  id: string;
  content: string;
  sender: "user" | "bot";
  timestamp: Date;
}

// ===============
// SEED DATA gốc
// ===============
export const SEED_CLASSES: ClassInfo[] = [
  { id: "1", name: "Toán 12A1" },
  { id: "2", name: "Vật lý 11B2" },
  { id: "3", name: "Hóa học 10C" },
];

export const SEED_DOCUMENTS: Document[] = [
  {
    id: "1",
    title: "Bài giảng Đại số",
    type: "pdf",
    className: "Toán 12A1",
    uploadedBy: "Thầy A",
    uploadedAt: "2025-10-20",
    viewed: true,
    version: 2,
    size: "2.5 MB",
  },
  {
    id: "2",
    title: "Video: Hàm số",
    type: "video",
    className: "Toán 12A1",
    uploadedBy: "Thầy A",
    uploadedAt: "2025-10-22",
    viewed: false,
    version: 1,
    duration: "45:30",
  },
  {
    id: "3",
    title: "Động học chất điểm",
    type: "pdf",
    className: "Vật lý 11B2",
    uploadedBy: "Cô B",
    uploadedAt: "2025-10-25",
    viewed: true,
    version: 1,
    size: "1.8 MB",
  },
];

export const SEED_EXAMS: Exam[] = [
  {
    id: "1",
    title: "Kiểm tra giữa kỳ - Toán 12",
    className: "Toán 12A1",
    startTime: "2025-11-05T08:00:00",
    duration: 45,
    totalQuestions: 10,
    type: "mixed",
    status: "ongoing",
  },
  {
    id: "2",
    title: "Kiểm tra 15 phút - Vật lý",
    className: "Vật lý 11B2",
    startTime: "2025-11-10T14:00:00",
    duration: 15,
    totalQuestions: 5,
    type: "multiple-choice",
    status: "upcoming",
  },
];

export const SEED_EXAM_QUESTIONS: Question[] = [
  {
    id: "q1",
    content: "Giải phương trình: 2x + 5 = 15",
    type: "multiple-choice",
    options: ["x = 5", "x = 10", "x = 7.5", "x = 20"],
    points: 10,
  },
  {
    id: "q2",
    content: "Tính đạo hàm của hàm số y = x³ + 2x² - 5x + 3",
    type: "essay",
    points: 15,
  },
  {
    id: "q3",
    content: "Giá trị của sin(90°) = ___",
    type: "fill-blank",
    points: 5,
  },
];

export const SEED_EXAM_RESULTS: ExamResult[] = [
  {
    id: "r1",
    examId: "1",
    score: 85,
    totalPoints: 100,
    completedAt: "2025-10-28T10:30:00",
    focusLossCount: 2,
    copyPasteCount: 0,
  },
];

export const SEED_QA_QUESTIONS: QAQuestion[] = [
  {
    id: "qa1",
    title: "Cách giải phương trình bậc 2?",
    content:
      "Em không hiểu cách giải phương trình ax² + bx + c = 0 khi delta < 0. Thầy/cô có thể giải thích thêm không ạ?",
    author: "Nguyễn Văn An",
    className: "Toán 12A1",
    timestamp: new Date("2025-10-31T10:00:00"),
    likes: 5,
    answers: 2,
    status: "answered",
  },
  {
    id: "qa3",
    title: "Cấu trúc câu điều kiện loại 2?",
    content:
      "Em hay nhầm lẫn giữa câu điều kiện loại 2 và loại 3. Có cách nào để phân biệt dễ hơn không ạ?",
    author: "Lê Hoàng Cường",
    className: "Toán 12A1",
    timestamp: new Date("2025-10-29T09:15:00"),
    likes: 8,
    answers: 0,
    status: "open",
  },
];

export const SEED_CHAT_MESSAGES: Message[] = [
  {
    id: "1",
    content:
      "Xin chào! Tôi là trợ lý AI của hệ thống. Tôi có thể giúp bạn về các vấn đề học tập, giải đáp thắc mắc về bài kiểm tra, tài liệu. Bạn cần hỗ trợ gì?",
    sender: "bot",
    timestamp: new Date(),
  },
];

export const SEED_BOT_RESPONSES: Record<string, string> = {
  "bài kiểm tra":
    'Bạn có thể xem danh sách bài kiểm tra ở tab "Bài kiểm tra" trên trang chủ. Các bài kiểm tra đang diễn ra sẽ được đánh dấu và bạn có thể bắt đầu làm bài ngay.',
  "tài liệu":
    'Bạn có thể xem tất cả tài liệu học tập ở mục "Tài liệu" trên thanh menu bên trái. Tài liệu được phân loại theo PDF, Video và Bài viết.',
  default:
    'Tôi hiểu bạn đang hỏi về "{query}". Bạn có thể cụ thể hơn được không? Hoặc bạn có thể hỏi về: bài kiểm tra, điểm số, tài liệu, giáo viên.',
};

// =============================================
// MOCK DATA mở rộng cho UI (Next.js + AntD)
// =============================================

// Các type nội bộ phục vụ UI
export type StudentItem = {
  id: string;
  name: string;
  email: string;
  classId: string;
};
export type ExamItem = Exam & { classId: string; totalPoints: number };
export type QuestionItem = Question & { category?: string };
export type DocumentItem = Document & { classId: string };
export type ExamResultNormalized = ExamResult & { studentId: string };
export type HighRiskSeverity = "low" | "medium" | "high" | "critical";
export type CheatingAlert = {
  id: string;
  attemptId: string;
  studentId: string;
  examId: string;
  severity: HighRiskSeverity;
  createdAt: string;
};

// map className -> id
const classIdByName = new Map(SEED_CLASSES.map((c) => [c.name, c.id]));

// Lớp cho dashboard (thêm studentCount, teacherId)
export const mockClasses: Array<
  ClassInfo & { studentCount: number; teacherId: string }
> = [
  { id: "1", name: "Toán 12A1", studentCount: 32, teacherId: "t1" },
  { id: "2", name: "Vật lý 11B2", studentCount: 28, teacherId: "t1" },
  { id: "3", name: "Hóa học 10C", studentCount: 30, teacherId: "t2" },
];

// Học sinh
export const mockStudents: StudentItem[] = [
  { id: "s1", name: "Nguyễn Văn A", email: "a@example.com", classId: "1" },
  { id: "s2", name: "Trần Thị B", email: "b@example.com", classId: "1" },
  { id: "s3", name: "Lê Văn C", email: "c@example.com", classId: "2" },
  { id: "s4", name: "Phạm Minh D", email: "d@example.com", classId: "2" },
];

// Bài kiểm tra (bổ sung classId & totalPoints cho grading)
export const mockExams: ExamItem[] = SEED_EXAMS.map((e) => ({
  ...e,
  classId: classIdByName.get(e.className) || "1",
  totalPoints: 100,
}));

// Câu hỏi (thêm category nhẹ)
export const mockQuestions: QuestionItem[] = SEED_EXAM_QUESTIONS.map(
  (q, i) => ({
    ...q,
    id: q.id || `q${i + 1}`,
    category: q.type === "essay" ? "Đại số" : "Tổng hợp",
  })
);

// Tài liệu (thêm classId)
export const mockDocuments: DocumentItem[] = SEED_DOCUMENTS.map((d) => ({
  ...d,
  classId: classIdByName.get(d.className) || "1",
}));

// Kết quả bài thi (chuẩn hóa có studentId & Date cho completedAt)
export const mockExamResults: ExamResultNormalized[] = [
  {
    ...SEED_EXAM_RESULTS[0],
    studentId: "s1",
    // để UI toLocaleString() dùng Date
    completedAt: new Date(SEED_EXAM_RESULTS[0].completedAt).toISOString(),
  },
  {
    id: "r2",
    examId: "1",
    studentId: "s2",
    score: 72,
    totalPoints: 100,
    completedAt: new Date("2025-10-28T10:40:00").toISOString(),
    focusLossCount: 12,
    copyPasteCount: 2,
  },
  {
    id: "r3",
    examId: "1",
    studentId: "s3",
    score: 64,
    totalPoints: 100,
    completedAt: new Date("2025-10-28T10:45:00").toISOString(),
    focusLossCount: 8,
    copyPasteCount: 6,
  },
];

// Cảnh báo gian lận (để CheatingMonitor map sang high-risk list)
export const mockCheatingAlerts: CheatingAlert[] = [
  {
    id: "a1",
    attemptId: "r1",
    studentId: "s1",
    examId: "1",
    severity: "low",
    createdAt: "2025-10-28T10:31:00",
  },
  {
    id: "a2",
    attemptId: "r2",
    studentId: "s2",
    examId: "1",
    severity: "high",
    createdAt: "2025-10-28T10:41:00",
  },
  {
    id: "a3",
    attemptId: "r3",
    studentId: "s3",
    examId: "1",
    severity: "critical",
    createdAt: "2025-10-28T10:46:00",
  },
];

// =========================
// Helpers (tùy chọn dùng)
// =========================
export const classMapById = Object.fromEntries(
  mockClasses.map((c) => [c.id, c])
);
export const classMapByName = Object.fromEntries(
  mockClasses.map((c) => [c.name, c.id])
);
