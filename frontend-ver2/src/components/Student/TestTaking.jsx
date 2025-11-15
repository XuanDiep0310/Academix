// src/components/Student/TestTaking.jsx
import { useEffect, useMemo, useState } from "react";
import {
  Card,
  Typography,
  Tag,
  Button,
  Radio,
  Alert,
  Space,
  message,
  Select,
  Spin,
} from "antd";
import { Clock, AlertCircle, Calendar } from "lucide-react";
import styles from "../../assets/styles/TestTaking.module.scss";
import {
  callListMyClassesAPI,
  callStudentListExamsByClassAPI,
  callStudentStartExamAPI,
  callStudentSaveAnswerAPI,
  callStudentSubmitAttemptAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;

/* ------------ helpers ------------ */
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

function getTestStatus(test) {
  const now = new Date();
  const start = test.startTime ? new Date(test.startTime) : null;
  const end = test.endTime ? new Date(test.endTime) : null;

  // client đánh dấu đã làm xong
  if (test.status === "completed") return "completed";

  if (start && now < start) return "upcoming";
  if (end && now > end) return "closed";

  // ưu tiên status từ backend
  if (test.status === "Available" || test.status === "available")
    return "available";
  if (test.status === "Closed" || test.status === "closed") return "closed";

  return "available";
}

function canStartTest(test) {
  const now = new Date();
  const start = test.startTime ? new Date(test.startTime) : null;
  const end = test.endTime ? new Date(test.endTime) : null;
  const status = getTestStatus(test);

  if (status !== "available") return false;
  if (start && now < start) return false;
  if (end && now > end) return false;

  return true;
}

export function TestTaking() {
  /** ====== LỚP HỌC CỦA STUDENT ====== */
  const [classes, setClasses] = useState([]);
  const [selectedClassId, setSelectedClassId] = useState(null);
  const [loadingClasses, setLoadingClasses] = useState(false);

  /** ====== DANH SÁCH BÀI KIỂM TRA (LIST VIEW) ====== */
  const [tests, setTests] = useState([]);
  const [loadingTests, setLoadingTests] = useState(false);

  /** ====== LÀM BÀI (DOING VIEW) ====== */
  const [activeTest, setActiveTest] = useState(null);
  const [attemptId, setAttemptId] = useState(null);
  const [questions, setQuestions] = useState([]);
  const [answers, setAnswers] = useState({});
  const [timeLeft, setTimeLeft] = useState(0); // giây

  const [starting, setStarting] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  /* ================== FETCH LỚP ================== */
  const fetchClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();

      if (res && res.success) {
        const data = res.data;

        const arr = Array.isArray(data)
          ? data
          : Array.isArray(data?.items)
          ? data.items
          : [];

        const mapped = arr.map((c) => ({
          id: c.classId || c.id,
          name: c.className || c.name,
          code: c.classCode || c.code,
          teacherName: c.teacherName || c.ownerName || "Chưa có GV",
          progress: c.progress ?? 0,
          materialsCount: c.materialsCount ?? 0,
          testsCount: c.testsCount ?? 0,
        }));

        setClasses(mapped);

        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].id);
        }
      } else {
        message.error(res?.message || "Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchClasses error:", err);
      message.error("Có lỗi khi tải danh sách lớp học");
    } finally {
      setLoadingClasses(false);
    }
  };

  useEffect(() => {
    fetchClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchTests = async () => {
    if (!selectedClassId) return;
    try {
      setLoadingTests(true);

      const res = await callStudentListExamsByClassAPI(selectedClassId);
      if (res && res.success) {
        const data = res.data;
        const arr = Array.isArray(data)
          ? data
          : Array.isArray(data?.items)
          ? data.items
          : [];

        const mapped =
          arr.map((e) => ({
            id: e.examId,
            title: e.title,
            classId: e.classId,
            className: e.className,
            subject: e.subject || e.className,
            duration: e.duration,
            startTime: e.startTime,
            endTime: e.endTime,
            status: e.status, // "Available", "Upcoming", "Closed"...
          })) || [];

        setTests(mapped);
      } else {
        message.error(res?.message || "Không thể tải danh sách bài kiểm tra");
        setTests([]);
      }
    } catch (err) {
      console.error("fetchTests error:", err);
      message.error("Có lỗi khi tải bài kiểm tra");
      setTests([]);
    } finally {
      setLoadingTests(false);
    }
  };

  useEffect(() => {
    if (selectedClassId) {
      setActiveTest(null);
      setAttemptId(null);
      setQuestions([]);
      setAnswers({});
      setTimeLeft(0);
      fetchTests();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId]);

  useEffect(() => {
    if (!activeTest || timeLeft <= 0) return;

    const id = setInterval(() => {
      setTimeLeft((prev) => {
        if (prev <= 1) {
          handleSubmit(true); // auto submit
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTest, timeLeft]);

  /* ================== START TEST ================== */
  const startTest = async (test) => {
    if (!canStartTest(test)) {
      message.warning("Bài kiểm tra chưa đến giờ hoặc đã hết hạn!");
      return;
    }

    try {
      setStarting(true);

      const res = await callStudentStartExamAPI(test.id);
      if (!res || !res.success || !res.data) {
        message.error(res?.message || "Không thể bắt đầu bài kiểm tra");
        return;
      }

      const exam = res.data;

      const mappedQuestions = (exam.questions || [])
        .map((q) => ({
          id: q.questionId,
          examQuestionId: q.examQuestionId,
          text: q.questionText,
          questionType: q.questionType,
          order: q.questionOrder,
          marks: q.marks,
          options: (q.options || []).map((opt) => ({
            id: opt.optionId,
            text: opt.optionText,
            order: opt.optionOrder,
          })),
        }))
        .sort((a, b) => a.order - b.order);

      setAttemptId(exam.attemptId);
      setQuestions(mappedQuestions);
      setAnswers({});

      const durationMinutes = exam.duration ?? test.duration ?? 0;
      setTimeLeft(durationMinutes * 60);

      setActiveTest({
        id: exam.examId,
        title: exam.title,
        description: exam.description,
        duration: durationMinutes,
        startTime: exam.startTime,
        endTime: exam.endTime,
        className: test.className,
        subject: test.subject,
      });
    } catch (err) {
      console.error("startTest error:", err);
      message.error("Có lỗi khi bắt đầu bài kiểm tra");
    } finally {
      setStarting(false);
    }
  };

  /* ================== ANSWER ================== */
  const handleAnswerChange = async (questionId, optionId) => {
    if (!attemptId) return;

    setAnswers((prev) => ({ ...prev, [questionId]: optionId }));

    try {
      // res ở đây cũng là { success, message, data }
      await callStudentSaveAnswerAPI(attemptId, {
        questionId,
        selectedOptionId: optionId,
      });
    } catch (err) {
      console.error("save answer error:", err);
      message.error("Không lưu được câu trả lời, vui lòng thử lại");
    }
  };

  /* ================== SUBMIT ================== */
  const handleSubmit = async (auto = false) => {
    if (!attemptId || !activeTest) return;
    if (submitting) return;

    const total = questions.length;
    const answered = Object.keys(answers).length;

    if (!auto && answered < total) {
      const ok = window.confirm(
        `Bạn mới trả lời ${answered}/${total} câu. Bạn vẫn muốn nộp bài?`
      );
      if (!ok) return;
    }

    const payload = {
      answers: Object.entries(answers).map(([qId, optId]) => ({
        questionId: Number(qId),
        selectedOptionId: optId,
      })),
    };

    try {
      setSubmitting(true);
      const res = await callStudentSubmitAttemptAPI(attemptId, payload);
      // res: { success, message, data: { ...result... } }
      if (!res || !res.success || !res.data) {
        message.error(res?.message || "Nộp bài thất bại");
        return;
      }

      const result = res.data;

      message.success(
        `Đã nộp bài! Bạn được ${result.totalScore}/${result.totalMarks} điểm (${result.percentage}%)`
      );

      setTests((prev) =>
        prev.map((t) =>
          t.id === activeTest.id ? { ...t, status: "completed" } : t
        )
      );
    } catch (err) {
      console.error("submit exam error:", err);
      message.error("Có lỗi khi nộp bài");
    } finally {
      setActiveTest(null);
      setAttemptId(null);
      setQuestions([]);
      setAnswers({});
      setTimeLeft(0);
      setSubmitting(false);
    }
  };

  const formatClock = (sec) => {
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return `${m}:${String(s).padStart(2, "0")}`;
  };

  const headerTitle = useMemo(
    () => (activeTest ? activeTest.title : "Bài kiểm tra"),
    [activeTest]
  );

  /* ================== VIEW ĐANG LÀM BÀI ================== */
  if (activeTest) {
    const dangerTime = timeLeft > 0 && timeLeft < 300;

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
          {questions.map((q, idx) => (
            <Card key={q.id} className={styles.qCard} bordered>
              <div className={styles.qTitle}>
                Câu {idx + 1}: {q.text}
              </div>

              <Radio.Group
                className={styles.qOptions}
                onChange={(e) => handleAnswerChange(q.id, e.target.value)}
                value={answers[q.id]}
              >
                {q.options.map((opt, i) => (
                  <div key={opt.id} className={styles.optionRow}>
                    <Radio value={opt.id} />
                    <label className={styles.optionLabel}>
                      {String.fromCharCode(65 + i)}. {opt.text}
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
              Đã trả lời: {Object.keys(answers).length}/{questions.length} câu
            </Text>
            <Button
              type="primary"
              size="large"
              loading={submitting}
              onClick={() => handleSubmit(false)}
            >
              Nộp bài
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  /* ================== VIEW DANH SÁCH BÀI KIỂM TRA ================== */
  return (
    <div className={styles.wrap}>
      {/* Header + chọn lớp */}
      <div className={styles.headerRow}>
        <div className={styles.headerCard}>
          <Title level={4} className={styles.title}>
            Bài kiểm tra
          </Title>
          <Text type="secondary">
            Danh sách các bài kiểm tra trong các lớp bạn đang học
          </Text>
        </div>

        <div className={styles.classSelect}>
          <Select
            loading={loadingClasses}
            value={selectedClassId ?? undefined}
            onChange={(v) => setSelectedClassId(v)}
            placeholder="Chọn lớp"
            style={{ minWidth: 260 }}
            options={classes.map((c) => ({
              value: c.id,
              label: `${c.name} (${c.code})`,
            }))}
          />
        </div>
      </div>

      <Spin spinning={loadingTests}>
        <div className={styles.list}>
          {(!selectedClassId || classes.length === 0) && !loadingClasses ? (
            <Card className={styles.item} bordered>
              <Text type="secondary">
                Bạn chưa tham gia lớp nào nên chưa có bài kiểm tra.
              </Text>
            </Card>
          ) : tests.length === 0 ? (
            <Card className={styles.item} bordered>
              <Text type="secondary">Chưa có bài kiểm tra nào.</Text>
            </Card>
          ) : (
            tests.map((t) => {
              const status = getTestStatus(t);
              const now = new Date();
              const start = t.startTime ? new Date(t.startTime) : null;
              const canStart = canStartTest(t);
              const hoursToStart =
                start && now < start
                  ? Math.ceil(
                      (start.getTime() - now.getTime()) / (1000 * 60 * 60)
                    )
                  : 0;

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
                      <Button
                        type="primary"
                        loading={starting}
                        onClick={() => startTest(t)}
                      >
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

                    {status === "upcoming" && hoursToStart > 0 && (
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
            })
          )}
        </div>
      </Spin>
    </div>
  );
}
