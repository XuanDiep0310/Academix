// --- INTERFACES ---

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

// --- SEED DATA (Dữ liệu mẫu để hiển thị UI) ---

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
