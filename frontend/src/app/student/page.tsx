"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import {
  BookOpen,
  FileText,
  Award,
  Clock,
  AlertCircle,
  CheckCircle,
} from "lucide-react";
import { Tabs, Card, Progress, Button } from "antd";
import type { TabsProps } from "antd";
import styles from "@/assets/styles/StudentDashboard.module.scss";

import {
  SEED_CLASSES,
  SEED_EXAMS,
  SEED_EXAM_RESULTS,
  SEED_DOCUMENTS,
  Exam,
  ExamResult,
} from "@/data/seed";

interface DashboardStats {
  docsProgress: number;
  completedExamsCount: number;
  totalExamsForProgress: number;
  averageScore: number;
  examsProgress: number;
}

export default function StudentDashboard() {
  const router = useRouter();

  // giả lập dữ liệu (sau này thay bằng API)
  const exams = SEED_EXAMS;
  const results = SEED_EXAM_RESULTS;
  const documents = SEED_DOCUMENTS;

  const [selectedExamId, setSelectedExamId] = useState<string | null>(null);
  const [goExam, setGoExam] = useState(false);

  // điều hướng sang trang làm bài nếu cần
  useEffect(() => {
    if (goExam && selectedExamId) {
      router.push(`/student/examtaking?id=${selectedExamId}`);
    }
  }, [goExam, selectedExamId, router]);

  // thống kê
  const {
    docsProgress,
    completedExamsCount,
    totalExamsForProgress,
    averageScore,
    examsProgress,
  } = useMemo<DashboardStats>(() => {
    const totalDocs = documents.length;
    const viewedDocs = documents.filter((d) => d.viewed).length;
    const docsProgress = totalDocs > 0 ? (viewedDocs / totalDocs) * 100 : 0;

    const totalExamsForProgress = exams.filter(
      (e) => e.status !== "upcoming"
    ).length;
    const completedExamsCount = results.length;
    const examsProgress =
      totalExamsForProgress > 0
        ? (completedExamsCount / totalExamsForProgress) * 100
        : 0;

    const averageScore =
      results.length > 0
        ? results.reduce((acc, r) => acc + (r.score / r.totalPoints) * 100, 0) /
          results.length
        : 0;

    return {
      docsProgress,
      completedExamsCount,
      totalExamsForProgress,
      averageScore,
      examsProgress,
    };
  }, [documents, exams, results]);

  const renderExamActions = (exam: Exam, result?: ExamResult) => (
    <div className={styles.examContent}>
      <div className={styles.examInfo}>
        <div className={styles.infoItem}>
          <Clock size={16} /> {new Date(exam.startTime).toLocaleString("vi-VN")}
        </div>
        <div className={styles.infoItem}>
          <FileText size={16} />
          {exam.type === "multiple-choice"
            ? "Trắc nghiệm"
            : exam.type === "essay"
            ? "Tự luận"
            : "Hỗn hợp"}
        </div>
      </div>

      {result ? (
        <div className={styles.completedBanner}>
          <CheckCircle size={20} />
          <span>
            Đã hoàn thành — Điểm: {result.score}/{result.totalPoints}
          </span>
        </div>
      ) : exam.status === "ongoing" ? (
        <Button
          type="primary"
          className={styles.btnPrimary}
          onClick={() => {
            setSelectedExamId(exam.id);
            setGoExam(true);
          }}
        >
          Bắt đầu làm bài
        </Button>
      ) : exam.status === "upcoming" ? (
        <div className={styles.upcomingBanner}>
          <AlertCircle size={20} />
          <span>
            Bài kiểm tra sẽ mở vào{" "}
            {new Date(exam.startTime).toLocaleString("vi-VN")}
          </span>
        </div>
      ) : (
        <Button className={styles.btnDisabled} disabled>
          Đã kết thúc
        </Button>
      )}
    </div>
  );

  const renderResult = (result: ExamResult, exam: Exam) => {
    const percentage = (result.score / result.totalPoints) * 100;
    return (
      <div className={styles.resultContent}>
        <div className={styles.scoreSection}>
          <span>Điểm số:</span>
          <span className={styles.resultScore}>
            {result.score}/{result.totalPoints}
          </span>
        </div>
        <Progress percent={percentage} showInfo={false} strokeColor="#3b82f6" />
        <div className={styles.resultDetails}>
          <div>
            <p className={styles.label}>Hoàn thành lúc</p>
            <p>{new Date(result.completedAt).toLocaleString("vi-VN")}</p>
          </div>
          <div>
            <p className={styles.label}>Chống gian lận</p>
            <p>
              <span
                className={result.focusLossCount > 0 ? styles.alertText : ""}
              >
                {result.focusLossCount} lần mất focus
              </span>
              {" • "}
              <span
                className={result.copyPasteCount > 0 ? styles.alertText : ""}
              >
                {result.copyPasteCount} lần copy
              </span>
            </p>
          </div>
        </div>
      </div>
    );
  };

  const tabItems: TabsProps["items"] = [
    {
      key: "exams",
      label: "Bài kiểm tra",
      children: (
        <div className={styles.examsList}>
          {exams.map((exam) => {
            const result = results.find((r) => r.examId === exam.id);
            return (
              <Card key={exam.id} className={styles.examCard}>
                <div className={styles.examHeader}>
                  <div>
                    <h3>{exam.title}</h3>
                    <p>
                      {exam.className} • {exam.totalQuestions} câu hỏi •{" "}
                      {exam.duration} phút
                    </p>
                  </div>
                  <span
                    className={`${styles.badge} ${
                      styles[`badge${exam.status}`]
                    }`}
                  >
                    {exam.status === "ongoing"
                      ? "Đang diễn ra"
                      : exam.status === "completed"
                      ? "Đã kết thúc"
                      : "Sắp diễn ra"}
                  </span>
                </div>
                {renderExamActions(exam, result)}
              </Card>
            );
          })}
        </div>
      ),
    },
    {
      key: "results",
      label: "Kết quả",
      children: (
        <div className={styles.resultsList}>
          {results.map((result) => {
            const exam = exams.find((e) => e.id === result.examId);
            if (!exam) return null;
            return (
              <Card key={result.id} className={styles.resultCard}>
                <div className={styles.resultHeader}>
                  <div>
                    <h3>{exam.title}</h3>
                    <p>{exam.className}</p>
                  </div>
                </div>
                {renderResult(result, exam)}
              </Card>
            );
          })}
        </div>
      ),
    },
  ];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Xin chào, Học sinh!</h1>
        <p>Chào mừng bạn đến với hệ thống học tập trực tuyến</p>
      </div>

      <div className={styles.statsGrid}>
        <Card className={styles.statCard}>
          <div className={styles.statHeader}>
            <span>Tiến độ tài liệu</span>
            <BookOpen size={16} />
          </div>
          <div className={styles.statContent}>
            <Progress
              percent={docsProgress}
              showInfo={false}
              strokeColor="#3b82f6"
              className={styles.progressBarAntd}
            />
            <p>
              {documents.filter((d) => d.viewed).length}/{documents.length} tài
              liệu đã xem
            </p>
          </div>
        </Card>

        <Card className={styles.statCard}>
          <div className={styles.statHeader}>
            <span>Bài kiểm tra</span>
            <FileText size={16} />
          </div>
          <div className={styles.statContent}>
            <Progress
              percent={examsProgress}
              showInfo={false}
              strokeColor="#3b82f6"
              className={styles.progressBarAntd}
            />
            <p>
              {completedExamsCount}/{totalExamsForProgress} bài đã hoàn thành
            </p>
          </div>
        </Card>

        <Card className={styles.statCard}>
          <div className={styles.statHeader}>
            <span>Điểm trung bình</span>
            <Award size={16} />
          </div>
          <div className={styles.statContent}>
            <div className={styles.scoreDisplay}>
              <span className={styles.scoreValue}>
                {averageScore.toFixed(1)}
              </span>
              <span className={styles.scoreMax}>/100</span>
            </div>
            <p>
              {averageScore >= 80
                ? "Xuất sắc!"
                : averageScore >= 65
                ? "Khá tốt!"
                : "Cần cố gắng thêm"}
            </p>
          </div>
        </Card>
      </div>

      <div className={styles.tabsWrapper}>
        <Tabs defaultActiveKey="exams" items={tabItems} />
      </div>
    </div>
  );
}
