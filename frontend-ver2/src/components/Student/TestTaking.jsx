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
  Modal,
} from "antd";
import { Clock, AlertCircle, Calendar } from "lucide-react";
import styles from "../../assets/styles/TestTaking.module.scss";
import {
  callListMyClassesAPI,
  callStudentListExamsByClassAPI,
  callStudentStartExamAPI,
  callStudentSaveAnswerAPI,
  callStudentSubmitAttemptAPI,
  callStudentGetAttemptResultAPI,
  callStudentGetExamHistoryAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;

/* ------------ helpers ------------ */
function formatDateTime(dt) {
  const d = new Date(dt);
  if (Number.isNaN(d.getTime())) return "-";
  // Cộng thêm 7 giờ (7 * 60 * 60 * 1000 milliseconds) để điều chỉnh timezone
  const adjustedDate = new Date(d.getTime() + 7 * 60 * 60 * 1000);
  return adjustedDate.toLocaleString("vi-VN", {
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

  // Đã làm (attemptCount > 0) => coi như completed
  if (typeof test.attemptCount === "number" && test.attemptCount > 0) {
    return "completed";
  }

  if (test.status === "completed") return "completed";

  if (start && now < start) return "upcoming";
  if (end && now > end) return "closed";

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

  /** ====== KẾT QUẢ & MODAL ====== */
  const [examResults, setExamResults] = useState({}); // examId -> latest result
  const [attemptMap, setAttemptMap] = useState({}); // examId -> latest attemptId
  const [resultModalOpen, setResultModalOpen] = useState(false);
  const [selectedResult, setSelectedResult] = useState(null); // { test, result }
  const [loadingResult, setLoadingResult] = useState(false);

  const fetchExamHistory = async (classId) => {
    if (!classId) return;
    try {
      const res = await callStudentGetExamHistoryAPI(classId);
      console.log("fetchExamHistory res:", res);

      if (res && res.success && Array.isArray(res.data)) {
        const history = res.data;

        const resultMap = {};
        const attemptMapLocal = {};

        history.forEach((h) => {
          const existing = resultMap[h.examId];
          if (!existing) {
            resultMap[h.examId] = h;
            attemptMapLocal[h.examId] = h.attemptId;
          } else {
            const prevTime = new Date(
              existing.submitTime || existing.startTime || 0
            ).getTime();
            const curTime = new Date(
              h.submitTime || h.startTime || 0
            ).getTime();
            if (curTime >= prevTime) {
              resultMap[h.examId] = h;
              attemptMapLocal[h.examId] = h.attemptId;
            }
          }
        });

        setExamResults(resultMap);
        setAttemptMap(attemptMapLocal);
      }
    } catch (err) {
      console.error("fetchExamHistory error:", err);
    }
  };
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

  /* ================== FETCH BÀI KIỂM TRA ================== */
  const fetchTests = async () => {
    if (!selectedClassId) return;
    try {
      setLoadingTests(true);

      const res = await callStudentListExamsByClassAPI(selectedClassId);

      if (res && res.success) {
        const data = res.data;
        const arr = Array.isArray(data) ? data : [];

        const mapped = arr.map((e) => ({
          id: e.examId,
          title: e.title,
          classId: e.classId,
          className: e.className,
          subject: e.subject || e.className,
          duration: e.duration,
          startTime: e.startTime,
          endTime: e.endTime,
          attemptCount: typeof e.attemptCount === "number" ? e.attemptCount : 0,
          status: e.status || null,
        }));

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
      fetchExamHistory(selectedClassId);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId]);

  /* ================== ĐẾM NGƯỢC ================== */
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
    // Không cho làm lại nếu đã có attempt
    if (typeof test.attemptCount === "number" && test.attemptCount > 0) {
      message.warning("Bạn đã làm bài kiểm tra này rồi, không thể làm lại.");
      return;
    }

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
          id: q.questionId || q.id,
          examQuestionId: q.examQuestionId,
          text: q.questionText || q.text || q.content,
          questionType: q.questionType,
          order: q.questionOrder ?? q.order ?? 0,
          marks: q.marks ?? q.point ?? 1,
          options: (q.options || q.answers || []).map((opt) => ({
            id: opt.optionId || opt.id,
            text:
              opt.optionText || opt.text || opt.content || opt.answerText || "",
            order: opt.optionOrder ?? opt.order ?? 0,
          })),
        }))
        .sort((a, b) => a.order - b.order);

      setAttemptId(exam.attemptId);
      setAttemptMap((prev) => ({
        ...prev,
        [exam.examId]: exam.attemptId,
      }));

      setQuestions(mappedQuestions);
      console.log(">>> question", mappedQuestions);
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
      if (!res || !res.success || !res.data) {
        message.error(res?.message || "Nộp bài thất bại");
        return;
      }

      const result = res.data;

      message.success(
        `Đã nộp bài! Bạn được ${result.totalScore}/${result.totalMarks} điểm (${result.percentage}%)`
      );

      // cache kết quả & attemptId
      setExamResults((prev) => ({
        ...prev,
        [activeTest.id]: result,
      }));
      setAttemptMap((prev) => ({
        ...prev,
        [activeTest.id]: result.attemptId || attemptId,
      }));

      // đánh dấu completed + tăng attemptCount
      setTests((prev) =>
        prev.map((t) =>
          t.id === activeTest.id
            ? {
                ...t,
                status: "completed",
                attemptCount: (t.attemptCount || 0) + 1,
              }
            : t
        )
      );

      setSelectedResult({ test: { ...activeTest }, result });
      setResultModalOpen(true);
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

  // test: object trong mảng tests (1 bài kiểm tra)
  const handleViewResult = async (test) => {
    // 1. Nếu đã cache trong examResults (từ history hoặc vừa nộp) -> dùng luôn
    const cached = examResults[test.id];
    if (cached) {
      setSelectedResult({ test, result: cached });
      setResultModalOpen(true);
      return;
    }

    // 2. Nếu không có cached nhưng có attemptId (fallback)
    const attemptIdForExam = attemptMap[test.id];
    if (!attemptIdForExam) {
      message.info("Bạn chưa làm bài kiểm tra này nên chưa có kết quả.");
      return;
    }

    try {
      setLoadingResult(true);
      const res = await callStudentGetAttemptResultAPI(attemptIdForExam);
      if (!res || !res.success || !res.data) {
        message.error(res?.message || "Không thể lấy kết quả bài kiểm tra");
        return;
      }

      const result = res.data;

      // cache lại để lần sau bấm nhanh
      setExamResults((prev) => ({
        ...prev,
        [test.id]: result,
      }));

      setSelectedResult({ test, result });
      setResultModalOpen(true);
    } catch (err) {
      console.error("get result error:", err);
      message.error("Có lỗi khi tải kết quả");
    } finally {
      setLoadingResult(false);
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
            message="Còn lại khá ít! Hãy kiểm tra lại các câu trả lời."
            className={styles.alert}
          />
        )}

        <div className={styles.questions}>
          {questions.map((q, idx) => (
            <Card key={q.id} className={styles.qCard} bordered>
              <div
                className={styles.qTitle}
                dangerouslySetInnerHTML={{
                  __html: `Câu ${idx + 1}: ${q.text}`,
                }}
              />

              <Radio.Group
                className={styles.qOptions}
                onChange={(e) => handleAnswerChange(q.id, e.target.value)}
                value={answers[q.id]}
              >
                {q.options.map((opt, i) => (
                  <div key={opt.id} className={styles.optionRow}>
                    <Radio value={opt.id} />
                    <label
                      className={styles.optionLabel}
                      style={{}}
                      dangerouslySetInnerHTML={{
                        __html: `${String.fromCharCode(65 + i)}. ${opt.text}`,
                      }}
                    />
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

  return (
    <>
      <div className={styles.wrap}>
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
                        <Space>
                          <Button onClick={() => handleViewResult(t)}>
                            Xem kết quả
                          </Button>
                          <Button disabled>Đã nộp</Button>
                        </Space>
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

      {/* MODAL KẾT QUẢ */}
      <Modal
        open={resultModalOpen}
        onCancel={() => setResultModalOpen(false)}
        footer={null}
        confirmLoading={loadingResult}
        title={
          selectedResult
            ? `Kết quả: ${selectedResult.test.title}`
            : "Kết quả bài kiểm tra"
        }
      >
        {selectedResult && (
          <>
            <p>
              Điểm:{" "}
              <strong>
                {selectedResult.result.totalScore}/
                {selectedResult.result.totalMarks} (
                {selectedResult.result.percentage}%)
              </strong>
            </p>
            <p>Trạng thái: {selectedResult.result.status}</p>
            <p>
              Thời gian làm: {formatDateTime(selectedResult.result.startTime)} -{" "}
              {formatDateTime(selectedResult.result.submitTime)}
            </p>

            <p>
              Số câu đúng: {selectedResult.result.correctAnswers} /{" "}
              {selectedResult.result.totalQuestions}
            </p>
            {/* Nếu muốn, bạn có thể map selectedResult.result.answers để hiển thị từng câu */}
          </>
        )}
      </Modal>
    </>
  );
}
