import { useMemo } from "react";
import { Card, Typography, Tag, Progress, Collapse } from "antd";
import { CheckCircle, XCircle, TrendingUp } from "lucide-react";
import styles from "../../assets/styles/StudentResults.module.scss";

const { Title, Text } = Typography;
const { Panel } = Collapse;

/* ================== BASE DATA (no API) ================== */
const RESULTS = [
  {
    id: "1",
    testTitle: "Kiểm tra giữa kỳ - Chương 1",
    className: "Toán cao cấp 1",
    score: 8.5,
    totalQuestions: 10,
    correctAnswers: 8,
    completedAt: "2024-03-10 14:30",
    details: [
      {
        question: "Đạo hàm của hàm số f(x) = x² là gì?",
        yourAnswer: "2x",
        correctAnswer: "2x",
        isCorrect: true,
      },
      {
        question: "Tích phân của 1/x là gì?",
        yourAnswer: "x² + C",
        correctAnswer: "ln|x| + C",
        isCorrect: false,
      },
    ],
  },
  {
    id: "2",
    testTitle: "Bài tập tuần 2",
    className: "Lập trình C++",
    score: 9.0,
    totalQuestions: 5,
    correctAnswers: 5,
    completedAt: "2024-03-08 10:15",
  },
];
/* ======================================================== */

export function StudentResults() {
  const averageScore = useMemo(
    () =>
      RESULTS.length
        ? (RESULTS.reduce((a, r) => a + r.score, 0) / RESULTS.length).toFixed(1)
        : "0.0",
    []
  );

  const accuracyPercent = useMemo(() => {
    const correct = RESULTS.reduce((a, r) => a + r.correctAnswers, 0);
    const total = RESULTS.reduce((a, r) => a + r.totalQuestions, 0);
    return total ? Math.round((correct / total) * 100) : 0;
  }, []);

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <Title level={4} className={styles.title}>
          Kết quả của tôi
        </Title>
        <Text type="secondary">
          Xem điểm số và đáp án chi tiết của các bài kiểm tra
        </Text>
      </div>

      {/* KPIs */}
      <div className={styles.kpis}>
        <Card className={styles.kpiCard}>
          <Text type="secondary">Điểm trung bình</Text>
          <div className={styles.kpiValue}>{averageScore}/10</div>
          <Progress percent={Number(averageScore) * 10} size="small" />
        </Card>

        <Card className={styles.kpiCard}>
          <Text type="secondary">Tổng số bài đã làm</Text>
          <div className={styles.kpiValue}>{RESULTS.length}</div>
          <div className={styles.inlineMuted}>
            <TrendingUp size={16} />
            <span>Đang tiến bộ</span>
          </div>
        </Card>

        <Card className={styles.kpiCard}>
          <Text type="secondary">Tỷ lệ chính xác</Text>
          <div className={styles.kpiValue}>{accuracyPercent}%</div>
          <Progress percent={accuracyPercent} size="small" />
        </Card>
      </div>

      {/* List results */}
      <div className={styles.list}>
        {RESULTS.map((r) => (
          <Card key={r.id} className={styles.item} bordered>
            <div className={styles.itemHeader}>
              <div className={styles.itemMeta}>
                <div className={styles.itemTitle}>{r.testTitle}</div>
                <div className={styles.itemDesc}>
                  <Tag>{r.className}</Tag>
                  <span className={styles.time}>{r.completedAt}</span>
                </div>
              </div>
              <div className={styles.scoreBox}>
                <div className={styles.score}>
                  {r.score}
                  <span className={styles.scoreDenom}>/10</span>
                </div>
                <div className={styles.sub}>
                  {r.correctAnswers}/{r.totalQuestions} câu đúng
                </div>
              </div>
            </div>

            {r.details && r.details.length > 0 && (
              <div className={styles.details}>
                <Collapse ghost>
                  <Panel header="Xem chi tiết đáp án" key="details">
                    <div className={styles.detailList}>
                      {r.details.map((d, idx) => (
                        <div
                          key={idx}
                          className={`${styles.detailItem} ${
                            d.isCorrect ? styles.correct : styles.incorrect
                          }`}
                        >
                          <div className={styles.detailIcon}>
                            {d.isCorrect ? (
                              <CheckCircle size={18} />
                            ) : (
                              <XCircle size={18} />
                            )}
                          </div>
                          <div className={styles.detailContent}>
                            <div className={styles.question}>{d.question}</div>
                            <div className={styles.answers}>
                              <div className={styles.answerRow}>
                                <span className={styles.muted}>
                                  Câu trả lời của bạn:
                                </span>{" "}
                                <span
                                  className={
                                    d.isCorrect ? styles.ansOk : styles.ansBad
                                  }
                                >
                                  {d.yourAnswer}
                                </span>
                              </div>
                              {!d.isCorrect && (
                                <div className={styles.answerRow}>
                                  <span className={styles.muted}>
                                    Đáp án đúng:
                                  </span>{" "}
                                  <span className={styles.ansOk}>
                                    {d.correctAnswer}
                                  </span>
                                </div>
                              )}
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </Panel>
                </Collapse>
              </div>
            )}
          </Card>
        ))}
      </div>
    </div>
  );
}
