// src/components/Student/StudentResults.jsx
import { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router";
import { Card, Typography, Tag, Progress, Collapse, Spin, message } from "antd";
import { CheckCircle, XCircle, TrendingUp } from "lucide-react";
import styles from "../../assets/styles/StudentResults.module.scss";
import { callStudentGetAttemptResultAPI } from "../../services/api.service";

const { Title, Text } = Typography;
const { Panel } = Collapse;

function formatDateTime(dt) {
  const d = new Date(dt);
  if (Number.isNaN(d.getTime())) return "-";
  return d.toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function StudentResults() {
  const { attemptId } = useParams(); // route: /student/results/:attemptId
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  // ====== CALL API ======
  useEffect(() => {
    if (!attemptId) return;

    const fetchResult = async () => {
      try {
        setLoading(true);
        const res = await callStudentGetAttemptResultAPI(attemptId);
        console.log("fetchResult res:", res);
        if (!res || !res.success || !res.data) {
          message.error(res?.message || "Không thể tải kết quả");
          return;
        }

        setResult(res.data);
      } catch (err) {
        console.error("fetchResult error:", err);
        message.error("Có lỗi khi tải kết quả bài kiểm tra");
      } finally {
        setLoading(false);
      }
    };

    fetchResult();
  }, [attemptId]);

  // ====== KPIs (từ 1 bài làm) ======
  const score10 = useMemo(() => {
    if (!result) return "0.0";
    if (result.totalMarks === 0) return result.totalScore.toFixed(1);
    // quy ra thang điểm 10
    return ((result.totalScore / result.totalMarks) * 10).toFixed(1);
  }, [result]);

  const accuracyPercent = useMemo(() => {
    if (!result || !result.totalQuestions) return 0;
    return Math.round((result.correctAnswers / result.totalQuestions) * 100);
  }, [result]);

  if (!attemptId) {
    return (
      <div className={styles.wrap}>
        <Title level={4} className={styles.title}>
          Kết quả của tôi
        </Title>
        <Text type="secondary">
          Không tìm thấy attemptId trong đường dẫn. Hãy truy cập từ trang “Bài
          kiểm tra” sau khi nộp bài.
        </Text>
      </div>
    );
  }

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <Title level={4} className={styles.title}>
          Kết quả bài kiểm tra
        </Title>
        <Text type="secondary">
          Xem điểm số và đáp án chi tiết cho bài kiểm tra bạn vừa làm
        </Text>
      </div>

      <Spin spinning={loading}>
        {!result ? (
          <Card className={styles.item} bordered>
            <Text type="secondary">Chưa có dữ liệu kết quả.</Text>
          </Card>
        ) : (
          <>
            {/* KPIs */}
            <div className={styles.kpis}>
              <Card className={styles.kpiCard}>
                <Text type="secondary">Điểm số</Text>
                <div className={styles.kpiValue}>
                  {result.totalScore}/{result.totalMarks}
                </div>
                <Progress
                  percent={
                    result.totalMarks
                      ? Math.round(
                          (result.totalScore / result.totalMarks) * 100
                        )
                      : 0
                  }
                  size="small"
                />
              </Card>

              <Card className={styles.kpiCard}>
                <Text type="secondary">Điểm quy đổi (thang 10)</Text>
                <div className={styles.kpiValue}>{score10}/10</div>
                <div className={styles.inlineMuted}>
                  <TrendingUp size={16} />
                  <span>{result.status}</span>
                </div>
              </Card>

              <Card className={styles.kpiCard}>
                <Text type="secondary">Tỷ lệ chính xác</Text>
                <div className={styles.kpiValue}>{accuracyPercent}%</div>
                <Progress percent={accuracyPercent} size="small" />
              </Card>
            </div>

            {/* Thông tin bài kiểm tra */}
            <div className={styles.list}>
              <Card className={styles.item} bordered>
                <div className={styles.itemHeader}>
                  <div className={styles.itemMeta}>
                    <div className={styles.itemTitle}>
                      {result.examTitle || "Bài kiểm tra"}
                    </div>
                    <div className={styles.itemDesc}>
                      <Tag>{result.studentName}</Tag>
                      <span className={styles.time}>
                        Bắt đầu: {formatDateTime(result.startTime)} - Nộp lúc:{" "}
                        {formatDateTime(result.submitTime)}
                      </span>
                    </div>
                  </div>
                  <div className={styles.scoreBox}>
                    <div className={styles.score}>
                      {result.totalScore}
                      <span className={styles.scoreDenom}>
                        /{result.totalMarks}
                      </span>
                    </div>
                    <div className={styles.sub}>
                      {result.correctAnswers}/{result.totalQuestions} câu đúng
                    </div>
                  </div>
                </div>

                {/* Chi tiết câu hỏi */}
                {Array.isArray(result.answers) && result.answers.length > 0 && (
                  <div className={styles.details}>
                    <Collapse ghost>
                      <Panel header="Xem chi tiết đáp án" key="details">
                        <div className={styles.detailList}>
                          {result.answers.map((ans, idx) => (
                            <div
                              key={ans.questionId ?? idx}
                              className={`${styles.detailItem} ${
                                ans.isCorrect
                                  ? styles.correct
                                  : styles.incorrect
                              }`}
                            >
                              <div className={styles.detailIcon}>
                                {ans.isCorrect ? (
                                  <CheckCircle size={18} />
                                ) : (
                                  <XCircle size={18} />
                                )}
                              </div>
                              <div className={styles.detailContent}>
                                <div className={styles.question}>
                                  Câu {idx + 1}: {ans.questionText}
                                </div>
                                <div className={styles.answers}>
                                  <div className={styles.answerRow}>
                                    <span className={styles.muted}>
                                      Câu trả lời của bạn:
                                    </span>{" "}
                                    <span
                                      className={
                                        ans.isCorrect
                                          ? styles.ansOk
                                          : styles.ansBad
                                      }
                                    >
                                      {ans.selectedOptionText || "—"}
                                    </span>
                                  </div>
                                  {!ans.isCorrect && (
                                    <div className={styles.answerRow}>
                                      <span className={styles.muted}>
                                        Đáp án đúng:
                                      </span>{" "}
                                      <span className={styles.ansOk}>
                                        {ans.correctOptionText || "—"}
                                      </span>
                                    </div>
                                  )}
                                  <div className={styles.answerRow}>
                                    <span className={styles.muted}>Điểm:</span>{" "}
                                    <span>
                                      {ans.marksObtained}/{ans.totalMarks}
                                    </span>
                                  </div>
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
            </div>
          </>
        )}
      </Spin>
    </div>
  );
}
