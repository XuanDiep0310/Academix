"use client";
import React, { useMemo, useState } from "react";
import { ThumbsUp, Send } from "lucide-react";
import { Card, Button, Modal, Tabs, Input, Select, notification } from "antd";
import styles from "@/assets/styles/QAForum.module.scss";
import { SEED_QA_QUESTIONS, SEED_CLASSES, QAQuestion } from "@/data/seed";

const { TextArea } = Input;

type FilterStatus = "all" | "open" | "answered";

const QAForum: React.FC = () => {
  // seed & sort desc by time
  const initialQuestions = useMemo(
    () =>
      [...SEED_QA_QUESTIONS].sort((a, b) => {
        const ta =
          a.timestamp instanceof Date
            ? a.timestamp.getTime()
            : new Date(a.timestamp).getTime();
        const tb =
          b.timestamp instanceof Date
            ? b.timestamp.getTime()
            : new Date(b.timestamp).getTime();
        return tb - ta;
      }),
    []
  );
  const classes = SEED_CLASSES;

  const [questions, setQuestions] = useState<QAQuestion[]>(initialQuestions);
  const [newQuestion, setNewQuestion] = useState({
    title: "",
    content: "",
    className: classes[0]?.name || "",
  });
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [likedIds, setLikedIds] = useState<string[]>([]);
  const [filterStatus, setFilterStatus] = useState<FilterStatus>("all");

  const MAX_TITLE_LENGTH = 100;
  const MAX_CONTENT_LENGTH = 500;

  const getInitials = (name: string) =>
    name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .slice(0, 2)
      .toUpperCase();

  const handleAddQuestion = () => {
    if (!newQuestion.title.trim() || !newQuestion.content.trim()) {
      notification.error({
        message: "Lỗi",
        description: "Vui lòng nhập đầy đủ tiêu đề và nội dung câu hỏi!",
      });
      return;
    }
    const q: QAQuestion = {
      id: Date.now().toString(),
      title: newQuestion.title.trim(),
      content: newQuestion.content.trim(),
      author: "Học sinh hệ thống",
      className: newQuestion.className,
      timestamp: new Date(),
      likes: 0,
      answers: 0,
      status: "open",
    };
    setQuestions((prev) => [q, ...prev]);
    setNewQuestion({
      title: "",
      content: "",
      className: classes[0]?.name || "",
    });
    setIsDialogOpen(false);
    notification.success({
      message: "Thành công",
      description: "Câu hỏi của bạn đã được đăng.",
    });
  };

  const handleLike = (id: string) => {
    setQuestions((prev) =>
      prev.map((q) =>
        q.id === id
          ? { ...q, likes: likedIds.includes(id) ? q.likes - 1 : q.likes + 1 }
          : q
      )
    );
    setLikedIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const totals = useMemo(
    () => ({
      all: questions.length,
      open: questions.filter((q) => q.status === "open").length,
      answered: questions.filter((q) => q.status === "answered").length,
    }),
    [questions]
  );

  const byStatus = useMemo<Record<FilterStatus, QAQuestion[]>>(
    () => ({
      all: questions,
      open: questions.filter((q) => q.status === "open"),
      answered: questions.filter((q) => q.status === "answered"),
    }),
    [questions]
  );

  const renderList = (status: FilterStatus) => {
    const data = byStatus[status];
    if (data.length === 0) {
      return (
        <div className={styles.emptyState}>
          <h3>Chưa có câu hỏi nào</h3>
          <p>Hãy là người đầu tiên đặt câu hỏi!</p>
        </div>
      );
    }
    return (
      <div className={styles.questionsList}>
        {data.map((question) => {
          const isLiked = likedIds.includes(question.id);
          const ts =
            question.timestamp instanceof Date
              ? question.timestamp
              : new Date(question.timestamp);
          return (
            <Card key={question.id} className={styles.questionCard}>
              <div className={styles.cardHeader}>
                <div className={styles.badges}>
                  <span
                    className={
                      question.status === "answered"
                        ? styles.badgeAnswered
                        : styles.badgeOpen
                    }
                  >
                    {question.status === "answered"
                      ? "✓ Đã trả lời"
                      : "⏱ Chưa trả lời"}
                  </span>
                  <span className={styles.badgeClass}>
                    {question.className}
                  </span>
                </div>
                <h3>{question.title}</h3>
                <p>{question.content}</p>
              </div>
              <div className={styles.cardFooter}>
                <div className={styles.authorInfo}>
                  <div className={styles.avatar}>
                    {getInitials(question.author)}
                  </div>
                  <span className={styles.authorName}>{question.author}</span>
                  <span className={styles.separator}>•</span>
                  <span className={styles.timestamp}>
                    {ts.toLocaleDateString("vi-VN")}
                  </span>
                </div>
                <div className={styles.actions}>
                  <Button
                    className={`${styles.btnLike} ${
                      isLiked ? styles.liked : ""
                    }`}
                    onClick={() => handleLike(question.id)}
                    icon={
                      <ThumbsUp size={16} fill={isLiked ? "#3b82f6" : "none"} />
                    }
                  >
                    {question.likes}
                  </Button>
                  <div className={styles.answerCount}>
                    <Send size={16} />
                    <span>{question.answers} câu trả lời</span>
                  </div>
                </div>
              </div>
            </Card>
          );
        })}
      </div>
    );
  };

  const items = [
    {
      key: "all",
      label: `Tất cả (${totals.all})`,
      children: renderList("all"),
    },
    {
      key: "open",
      label: `Chưa trả lời (${totals.open})`,
      children: renderList("open"),
    },
    {
      key: "answered",
      label: `Đã trả lời (${totals.answered})`,
      children: renderList("answered"),
    },
  ];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>Hỏi đáp</h1>
          <p>Đặt câu hỏi và nhận câu trả lời từ giáo viên và bạn học</p>
        </div>
        <Button
          type="primary"
          className={styles.btnPrimary}
          onClick={() => setIsDialogOpen(true)}
          icon={<Send size={16} />}
        >
          Đặt câu hỏi
        </Button>
      </div>

      <div className={styles.filterTabs}>
        <Tabs
          activeKey={filterStatus}
          onChange={(k) => setFilterStatus(k as FilterStatus)}
          items={items}
          destroyInactiveTabPane
        />
      </div>

      <Modal
        title={
          <div className={styles.dialogHeader}>
            <h2>Đặt câu hỏi mới</h2>
            <p>Đặt câu hỏi để nhận sự hỗ trợ từ giáo viên và bạn học</p>
          </div>
        }
        open={isDialogOpen}
        onCancel={() => setIsDialogOpen(false)}
        footer={[
          <Button
            key="cancel"
            onClick={() => setIsDialogOpen(false)}
            className={styles.btnCancel}
          >
            Hủy
          </Button>,
          <Button
            key="submit"
            type="primary"
            onClick={handleAddQuestion}
            className={styles.btnSubmit}
          >
            Đăng câu hỏi
          </Button>,
        ]}
        className={styles.dialog}
      >
        <div className={styles.dialogContent}>
          <div className={styles.formGroup}>
            <label>
              Tiêu đề <span className={styles.required}>*</span>
            </label>
            <Input
              placeholder={`Nhập tiêu đề câu hỏi (tối đa ${MAX_TITLE_LENGTH} ký tự)...`}
              value={newQuestion.title}
              onChange={(e) =>
                setNewQuestion((s) => ({
                  ...s,
                  title: e.target.value.slice(0, MAX_TITLE_LENGTH),
                }))
              }
              maxLength={MAX_TITLE_LENGTH}
            />
            <span className={styles.charCount}>
              {newQuestion.title.length}/{MAX_TITLE_LENGTH} ký tự
            </span>
          </div>

          <div className={styles.formGroup}>
            <label>
              Lớp học <span className={styles.required}>*</span>
            </label>
            <Select
              value={newQuestion.className}
              onChange={(value) =>
                setNewQuestion((s) => ({ ...s, className: value }))
              }
              options={classes.map((c) => ({ label: c.name, value: c.name }))}
              style={{ width: "100%" }}
            />
          </div>

          <div className={styles.formGroup}>
            <label>
              Nội dung <span className={styles.required}>*</span>
            </label>
            <TextArea
              placeholder="Mô tả chi tiết câu hỏi của bạn..."
              value={newQuestion.content}
              onChange={(e) =>
                setNewQuestion((s) => ({
                  ...s,
                  content: e.target.value.slice(0, MAX_CONTENT_LENGTH),
                }))
              }
              rows={6}
              maxLength={MAX_CONTENT_LENGTH}
            />
            <span className={styles.charCount}>
              {newQuestion.content.length}/{MAX_CONTENT_LENGTH} ký tự
            </span>
          </div>
        </div>
      </Modal>
    </div>
  );
};
export default QAForum;
