import { useEffect, useMemo, useState } from "react";
import { Card, Typography, Tag, Button, Radio, Alert, Space } from "antd";
import { Clock, AlertCircle, Calendar } from "lucide-react";
import styles from "../../assets/styles/TestTaking.module.scss";

const { Title, Text } = Typography;

/* ================== BASE DATA (no API) ================== */
const TESTS_BASE = [
  {
    id: "1",
    title: "Kiểm tra giữa kỳ - Chương 1",
    className: "Toán cao cấp 1",
    subject: "Toán cao cấp 1",
    duration: 45, // phút
    startTime: "2024-01-01T08:00",
    endTime: "2025-12-31T23:59",
    status: "available",
    questions: [
      {
        id: "1",
        question: "Đạo hàm của hàm số f(x) = x² là gì?",
        options: ["x", "2x", "x²", "2"],
      },
      {
        id: "2",
        question: "Tích phân của 1/x là gì?",
        options: ["ln|x| + C", "x² + C", "1/x² + C", "e^x + C"],
      },
    ],
  },
  {
    id: "2",
    title: "Kiểm tra tuần 5",
    className: "Toán cao cấp 1",
    subject: "Giải tích",
    duration: 30,
    startTime: "2025-01-10T08:00",
    endTime: "2025-01-12T23:59",
    status: "upcoming",
  },
  {
    id: "3",
    title: "Bài tập tuần 2",
    className: "Lập trình C++",
    subject: "Lập trình C++",
    duration: 60,
    startTime: "2024-02-01T08:00",
    endTime: "2024-02-15T23:59",
    status: "closed",
  },
];
/* ======================================================== */

function formatDateTime(dt) {
  const d = new Date(dt);
  return d.toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function getTestStatus(test) {
  const now = new Date();
  const start = new Date(test.startTime);
  const end = new Date(test.endTime);
  if (test.status === "completed") return "completed";
  if (now < start) return "upcoming";
  if (now > end) return "closed";
  return "available";
}

function canStartTest(test) {
  const now = new Date();
  const start = new Date(test.startTime);
  const end = new Date(test.endTime);
  return now >= start && now <= end && getTestStatus(test) === "available";
}

export function TestTaking() {
  const [tests, setTests] = useState(() => TESTS_BASE);
  const [activeTest, setActiveTest] = useState(null);
  const [answers, setAnswers] = useState({});
  const [timeLeft, setTimeLeft] = useState(0); // giây

  // Tick countdown
  useEffect(() => {
    if (!activeTest || timeLeft <= 0) return;
    const id = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          handleSubmit(); // auto nộp khi hết giờ
          return 0;
        }
        return prev - 1;
      });
    }, 1000);
    return () => clearInterval(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTest, timeLeft]);

  const startTest = (test) => {
    if (!canStartTest(test)) {
      alert("Bài kiểm tra chưa đến giờ hoặc đã hết hạn!");
      return;
    }
    setActiveTest(test);
    setTimeLeft(test.duration * 60);
    setAnswers({});
  };

  const handleAnswerChange = (qid, idx) => {
    setAnswers((prev) => ({ ...prev, [qid]: idx }));
  };

  const handleSubmit = () => {
    const total = activeTest?.questions?.length || 0;
    const answered = Object.keys(answers).length;
    // mock submit:
    alert(`Đã nộp bài! Số câu đã trả lời: ${answered}/${total}`);

    setTests((prev) =>
      prev.map((t) =>
        t.id === activeTest?.id ? { ...t, status: "completed" } : t
      )
    );
    setActiveTest(null);
    setAnswers({});
    setTimeLeft(0);
  };

  const formatClock = (sec) => {
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return `${m}:${String(s).padStart(2, "0")}`;
  };

  const headerTitle = useMemo(() => {
    if (!activeTest) return "Bài kiểm tra";
    return activeTest.title;
  }, [activeTest]);

  /* ====================== DOING TEST VIEW ====================== */
  if (activeTest) {
    const dangerTime = timeLeft > 0 && timeLeft < 300; // < 5 phút
    return (
      <div className={styles.wrap}>
        <div className={styles.topBar}>
          <Title level={4} className={styles.title}>
            {headerTitle}
          </Title>
          <Space size={12} className={styles.badges}>
            <Tag>{activeTest.className}</Tag>
            <Tag>{activeTest.subject}</Tag>
          </Space>

          <div
            className={`${styles.timer} ${
              dangerTime ? styles.timerDanger : ""
            }`}
          >
            <Clock size={18} />
            <span>{formatClock(timeLeft)}</span>
          </div>
        </div>

        {dangerTime && (
          <Alert
            type="error"
            showIcon
            icon={<AlertCircle size={16} />}
            message="Còn lại ít hơn 5 phút! Hãy kiểm tra lại các câu trả lời."
            className={styles.alert}
          />
        )}

        <div className={styles.questions}>
          {activeTest.questions?.map((q, idx) => (
            <Card key={q.id} className={styles.qCard} bordered>
              <div className={styles.qTitle}>
                Câu {idx + 1}: {q.question}
              </div>

              <Radio.Group
                className={styles.qOptions}
                onChange={(e) => handleAnswerChange(q.id, e.target.value)}
                value={answers[q.id]}
              >
                {q.options.map((opt, i) => (
                  <div key={i} className={styles.optionRow}>
                    <Radio value={i} />
                    <label className={styles.optionLabel}>
                      {String.fromCharCode(65 + i)}. {opt}
                    </label>
                  </div>
                ))}
              </Radio.Group>
            </Card>
          ))}
        </div>

        <Card className={styles.submitBar} bordered>
          <div className={styles.submitInner}>
            <Text type="secondary">
              Đã trả lời: {Object.keys(answers).length}/
              {activeTest.questions?.length || 0} câu
            </Text>
            <Button type="primary" size="large" onClick={handleSubmit}>
              Nộp bài
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  /* ====================== LIST TESTS VIEW ====================== */
  return (
    <div className={styles.wrap}>
      <div className={styles.headerCard}>
        <Title level={4} className={styles.title}>
          Bài kiểm tra
        </Title>
        <Text type="secondary">Danh sách các bài kiểm tra của bạn</Text>
      </div>

      <div className={styles.list}>
        {tests.map((t) => {
          const status = getTestStatus(t);
          const canStart = canStartTest(t);
          const now = new Date();
          const start = new Date(t.startTime);
          const hoursToStart = Math.ceil(
            (start.getTime() - now.getTime()) / (1000 * 60 * 60)
          );

          return (
            <Card key={t.id} className={styles.item} bordered>
              <div className={styles.itemHead}>
                <div className={styles.itemMeta}>
                  <div className={styles.itemTitle}>{t.title}</div>
                  <div className={styles.tags}>
                    <Tag>{t.className}</Tag>
                    <Tag>{t.subject}</Tag>
                    <Tag
                      color={
                        status === "completed"
                          ? "default"
                          : status === "available"
                          ? "green"
                          : status === "upcoming"
                          ? "blue"
                          : "red"
                      }
                    >
                      {status === "completed"
                        ? "Đã hoàn thành"
                        : status === "available"
                        ? "Đang mở"
                        : status === "upcoming"
                        ? "Sắp mở"
                        : "Đã đóng"}
                    </Tag>
                  </div>
                </div>

                {canStart ? (
                  <Button type="primary" onClick={() => startTest(t)}>
                    Bắt đầu làm bài
                  </Button>
                ) : status === "completed" ? (
                  <Button disabled>Đã nộp</Button>
                ) : status === "upcoming" ? (
                  <Button disabled>Chưa đến giờ</Button>
                ) : (
                  <Button disabled>Đã hết hạn</Button>
                )}
              </div>

              <div className={styles.itemBody}>
                <div className={styles.inline}>
                  <Clock size={16} />
                  <span>Thời lượng: {t.duration} phút</span>
                </div>
                <div className={styles.inlineTop}>
                  <Calendar size={16} />
                  <div>
                    <div>Bắt đầu: {formatDateTime(t.startTime)}</div>
                    <div>Kết thúc: {formatDateTime(t.endTime)}</div>
                  </div>
                </div>

                {status === "upcoming" && now < start && (
                  <Alert
                    type="info"
                    showIcon
                    message={`Bài kiểm tra sẽ mở trong ${hoursToStart} giờ nữa`}
                    className={styles.alert}
                  />
                )}
                {status === "available" && (
                  <Alert
                    type="success"
                    showIcon
                    message="Bài kiểm tra đang mở. Bạn có thể làm bài ngay bây giờ!"
                    className={styles.alert}
                  />
                )}
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
