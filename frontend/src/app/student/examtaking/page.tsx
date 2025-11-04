"use client";
import React, { useState, useEffect } from "react";
import {
  Clock,
  AlertTriangle,
  ChevronLeft,
  ChevronRight,
  Send,
} from "lucide-react";
import { Card, Button, Input, Modal, notification, Progress } from "antd";
import styles from "@/assets/styles/ExamTaking.module.scss";
import { SEED_EXAMS, SEED_EXAM_QUESTIONS, Exam, Question } from "@/data/seed";

// Confirm dialog
interface IConfirmDialog {
  visible: boolean;
  onConfirm: () => void;
  onCancel: () => void;
  type: "submit" | "exit";
  answeredCount: number;
  totalCount: number;
}
const ConfirmDialog = ({
  visible,
  onConfirm,
  onCancel,
  type,
  answeredCount,
  totalCount,
}: IConfirmDialog) => {
  const isSubmit = type === "submit";
  const unAnsweredCount = totalCount - answeredCount;

  return (
    <Modal
      open={visible}
      onCancel={onCancel}
      footer={[
        <Button key="cancel" onClick={onCancel} className={styles.btnCancel}>
          Hủy
        </Button>,
        <Button
          key="confirm"
          onClick={onConfirm}
          className={isSubmit ? styles.btnConfirm : styles.btnPrimary}
        >
          {isSubmit ? "Nộp bài ngay" : "Thoát và tiếp tục đếm giờ"}
        </Button>,
      ]}
      className={styles.confirmDialog}
    >
      <div className={styles.dialogHeader}>
        <AlertTriangle size={32} style={{ color: "#f59e0b" }} />
        <h3>{isSubmit ? "Xác nhận nộp bài" : "Xác nhận thoát bài thi"}</h3>
      </div>

      <div className={styles.dialogContent}>
        {isSubmit ? (
          <>
            <p>
              Bạn chắc chắn muốn <strong>nộp bài</strong> không?
            </p>
            {unAnsweredCount > 0 && (
              <p>
                Hiện tại bạn còn <strong>{unAnsweredCount} câu hỏi</strong> chưa
                trả lời.
              </p>
            )}
            <p>Sau khi nộp, bạn không thể chỉnh sửa thêm.</p>
          </>
        ) : (
          <p>
            Bạn chắc chắn muốn <strong>thoát khỏi bài thi</strong> không? Thời
            gian làm bài sẽ vẫn tiếp tục tính.
          </p>
        )}
      </div>
    </Modal>
  );
};

interface ExamTakingProps {
  examId: string;
  onBack: () => void;
}

const ExamTaking = ({ examId, onBack }: ExamTakingProps) => {
  const exam = SEED_EXAMS.find((e: Exam) => e.id === examId);
  // Giả định đã fetch đúng bộ câu hỏi cho examId
  const questions: Question[] = SEED_EXAM_QUESTIONS;

  const [currentQuestion, setCurrentQuestion] = useState(0);
  const [answers, setAnswers] = useState<Record<number, string>>({});
  const [timeLeft, setTimeLeft] = useState(exam ? exam.duration * 60 : 0);
  const [focusLossCount, setFocusLossCount] = useState(0);
  const [copyPasteCount, setCopyPasteCount] = useState(0);
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [dialogType, setDialogType] = useState<"submit" | "exit">("submit");

  // Anti-cheat listeners
  useEffect(() => {
    const handleBlur = () => {
      setFocusLossCount((prev) => prev + 1);
      notification.warning({
        message: "Cảnh báo chống gian lận",
        description:
          "Bạn đã mất focus khỏi cửa sổ trình duyệt. Hành vi này sẽ được ghi nhận.",
        placement: "topLeft",
      });
    };
    const handleCopy = (e: ClipboardEvent) => {
      setCopyPasteCount((prev) => prev + 1);
      e.preventDefault();
      notification.error({
        message: "Cảnh báo Copy",
        description: "Sao chép bị cấm trong bài thi.",
        placement: "topLeft",
      });
    };
    const handlePaste = (e: ClipboardEvent) => {
      setCopyPasteCount((prev) => prev + 1);
      e.preventDefault();
      notification.error({
        message: "Cảnh báo Paste",
        description: "Dán nội dung bị cấm trong bài thi.",
        placement: "topLeft",
      });
    };

    window.addEventListener("blur", handleBlur);
    document.addEventListener("copy", handleCopy);
    document.addEventListener("paste", handlePaste);
    return () => {
      window.removeEventListener("blur", handleBlur);
      document.removeEventListener("copy", handleCopy);
      document.removeEventListener("paste", handlePaste);
    };
  }, []);

  // Timer
  useEffect(() => {
    if (timeLeft <= 0 || isSubmitted) return;
    const timer = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          handleSubmit();
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(timer);
  }, [timeLeft, isSubmitted]);

  if (!exam) {
    return (
      <div className={styles.errorContainer}>
        <p>Không tìm thấy bài kiểm tra</p>
        <Button onClick={onBack} type="primary">
          Quay lại
        </Button>
      </div>
    );
  }

  const answeredCount = Object.keys(answers).length;
  const progress = (answeredCount / questions.length) * 100;

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, "0")}:${secs
      .toString()
      .padStart(2, "0")}`;
  };

  const handleSubmit = () => {
    // TODO: gọi API nộp bài thật sự (hiện đang giả lập)
    setIsSubmitted(true);
    setShowConfirmDialog(false);
  };

  const handleOpenSubmitConfirm = () => {
    setDialogType("submit");
    setShowConfirmDialog(true);
  };
  const handleOpenExitConfirm = () => {
    setDialogType("exit");
    setShowConfirmDialog(true);
  };

  const currentQ = questions[currentQuestion];

  if (isSubmitted) {
    return (
      <div className={styles.submittedContainer}>
        <div className={styles.iconSuccess}>
          <Send size={40} />
        </div>
        <h2>Đã nộp bài thành công!</h2>
        <p>
          Bài kiểm tra của bạn đã được gửi đi. Kết quả sẽ được công bố sau khi
          giáo viên chấm xong.
        </p>

        <Card className={styles.card}>
          <h3>Thông tin nộp bài</h3>
          <div className={styles.infoList}>
            <div className={styles.infoItem}>
              <span>Số câu đã trả lời:</span>
              <span>
                {answeredCount}/{questions.length}
              </span>
            </div>
            <div className={styles.infoItem}>
              <span>Thời gian còn lại:</span>
              <span>{formatTime(timeLeft)}</span>
            </div>
            <div className={styles.infoItem}>
              <span>Số lần mất focus:</span>
              <span
                className={
                  focusLossCount > 5 ? styles.badgeError : styles.badge
                }
              >
                {focusLossCount}
              </span>
            </div>
            <div className={styles.infoItem}>
              <span>Số lần copy/paste:</span>
              <span
                className={
                  copyPasteCount > 0 ? styles.badgeError : styles.badge
                }
              >
                {copyPasteCount}
              </span>
            </div>
          </div>
        </Card>

        <Button onClick={onBack} type="primary" className={styles.btnPrimary}>
          Quay về trang chủ
        </Button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <Button className={styles.btnBack} onClick={handleOpenExitConfirm}>
          <ChevronLeft size={16} /> Thoát
        </Button>

        <div className={styles.examInfo}>
          <h2>{exam.title}</h2>
          <p className={styles.className}>{exam.className}</p>
        </div>

        <div className={styles.timer}>
          <Clock size={22} />
          <span className={timeLeft < 300 ? styles.timeWarning : ""}>
            {formatTime(timeLeft)}
          </span>
        </div>
      </div>

      {focusLossCount > 0 && (
        <div className={styles.alert}>
          <AlertTriangle size={20} />
          <span>
            Cảnh báo: Bạn đã mất focus {focusLossCount} lần. Hành vi này sẽ được
            ghi nhận.
          </span>
        </div>
      )}

      {copyPasteCount > 0 && (
        <div className={styles.alert}>
          <AlertTriangle size={20} />
          <span>
            Cảnh báo: Copy/Paste không được phép trong bài thi. Đã phát hiện{" "}
            {copyPasteCount} lần vi phạm.
          </span>
        </div>
      )}

      <Card className={styles.card}>
        <div className={styles.progressSection}>
          <div className={styles.progressText}>
            <span>
              Tiến độ: {answeredCount}/{questions.length} câu
            </span>
            <span>{progress.toFixed(0)}%</span>
          </div>
          <Progress
            percent={progress}
            showInfo={false}
            strokeColor={{ "0%": "#3b82f6", "100%": "#2563eb" }}
          />
        </div>
      </Card>

      <Card className={styles.card}>
        <div className={styles.questionHeader}>
          <h3>
            Câu {currentQuestion + 1}/{questions.length}
          </h3>
          <span className={styles.badge}>{currentQ.points} điểm</span>
        </div>

        <div className={styles.questionContent}>
          <p className={styles.questionText}>{currentQ.content}</p>

          {currentQ.type === "multiple-choice" && currentQ.options && (
            <div className={styles.optionsGroup}>
              {currentQ.options.map((option, idx) => {
                const isSelected = answers[currentQuestion] === idx.toString();
                return (
                  <label
                    key={idx}
                    className={`${styles.optionLabel} ${
                      isSelected ? styles.optionSelected : ""
                    }`}
                  >
                    <input
                      type="radio"
                      name="answer"
                      value={idx}
                      checked={isSelected}
                      onChange={(e) =>
                        setAnswers({
                          ...answers,
                          [currentQuestion]: e.target.value,
                        })
                      }
                    />
                    <span>{option}</span>
                  </label>
                );
              })}
            </div>
          )}

          {currentQ.type === "essay" && (
            <Input.TextArea
              placeholder="Nhập câu trả lời của bạn..."
              value={answers[currentQuestion] || ""}
              onChange={(e) =>
                setAnswers({ ...answers, [currentQuestion]: e.target.value })
              }
              rows={10}
              className={styles.textarea}
            />
          )}

          {currentQ.type === "fill-blank" && (
            <Input
              type="text"
              placeholder="Nhập đáp án..."
              value={answers[currentQuestion] || ""}
              onChange={(e) =>
                setAnswers({ ...answers, [currentQuestion]: e.target.value })
              }
              className={styles.input}
            />
          )}
        </div>
      </Card>

      <div className={styles.navigation}>
        <Button
          className={styles.btnOutline}
          onClick={() => setCurrentQuestion((prev) => Math.max(0, prev - 1))}
          disabled={currentQuestion === 0}
        >
          <ChevronLeft size={16} /> Câu trước
        </Button>

        {currentQuestion === questions.length - 1 ? (
          <Button
            onClick={handleOpenSubmitConfirm}
            className={styles.btnPrimary}
          >
            <Send size={16} /> Nộp bài
          </Button>
        ) : (
          <Button
            onClick={() =>
              setCurrentQuestion((prev) =>
                Math.min(questions.length - 1, prev + 1)
              )
            }
            className={styles.btnPrimary}
          >
            Câu sau <ChevronRight size={16} />
          </Button>
        )}
      </div>

      <Card className={styles.card}>
        <h3 className={styles.cardTitle}>Danh sách câu hỏi</h3>
        <div className={styles.questionGrid}>
          {questions.map((_, idx) => (
            <Button
              key={idx}
              onClick={() => setCurrentQuestion(idx)}
              className={`${styles.questionBtn} ${
                currentQuestion === idx
                  ? styles.questionBtnActive
                  : answers[idx]
                  ? styles.questionBtnAnswered
                  : ""
              }`}
            >
              {idx + 1}
            </Button>
          ))}
        </div>
      </Card>

      <ConfirmDialog
        visible={showConfirmDialog}
        onConfirm={dialogType === "submit" ? handleSubmit : onBack}
        onCancel={() => setShowConfirmDialog(false)}
        type={dialogType}
        answeredCount={answeredCount}
        totalCount={questions.length}
      />
    </div>
  );
};

export default ExamTaking;
