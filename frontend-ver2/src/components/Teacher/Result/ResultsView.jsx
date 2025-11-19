import { useEffect, useMemo, useState } from "react";
import {
  Table,
  Select,
  Typography,
  Space,
  Card,
  Tag,
  Button,
  message,
  Empty,
} from "antd";
import { Download } from "lucide-react";
import dayjs from "dayjs";
import styles from "../../../assets/styles/ResultsView.module.scss";
import {
  callListMyClassesAPI,
  callListExamsByClassAPI,
  callGetExamResultsAPI,
  callListStudentOnClassesAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;

// map status từ API
const STATUS_TEXT = {
  Completed: "Đã nộp",
  InProgress: "Đang làm",
  NotStarted: "Chưa nộp bài",
  Graded: "Đã chấm",
};

const STATUS_COLOR = {
  Completed: "success",
  Graded: "success",
  InProgress: "processing",
  NotStarted: "default",
};

export default function ResultsView() {
  /* =================== STATE =================== */
  const [students, setStudents] = useState([]);

  const [classes, setClasses] = useState([]);
  const [loadingClasses, setLoadingClasses] = useState(false);
  const [selectedClassId, setSelectedClassId] = useState(null);

  const [exams, setExams] = useState([]);
  const [loadingExams, setLoadingExams] = useState(false);
  const [selectedExamId, setSelectedExamId] = useState(null);

  const [results, setResults] = useState([]);
  const [stats, setStats] = useState(null);
  const [loadingResults, setLoadingResults] = useState(false);

  /* =================== FETCH STUDENTS =================== */
  const fetchStudents = async (classId) => {
    if (!classId) return;
    try {
      const qs = new URLSearchParams();
      qs.set("page", "1");
      qs.set("pageSize", "1000"); // đủ lớn cho 1 lớp

      const res = await callListStudentOnClassesAPI(classId, qs.toString());
      console.log("check Student >>> ", res);

      if (res?.success && Array.isArray(res.data)) {
        const mapped = res.data.map((s) => ({
          id: s.classMemberId,
          userId: s.userId,
          name: s.fullName,
          email: s.email,
        }));
        setStudents(mapped);
      } else {
        message.error("Không thể tải danh sách học sinh");
        setStudents([]);
      }
    } catch (err) {
      console.error("fetchStudents error:", err);
      message.error("Có lỗi khi tải danh sách học sinh");
      setStudents([]);
    }
  };

  /* =================== FETCH LỚP HỌC =================== */
  const fetchClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();
      if (res?.success && Array.isArray(res.data)) {
        const mapped = res.data.map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
        }));
        setClasses(mapped);

        // chọn lớp đầu tiên nếu chưa chọn
        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].id);
        }
      } else {
        message.error("Không thể tải danh sách lớp học");
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

  /* =================== FETCH EXAMS CỦA 1 LỚP =================== */
  const fetchExams = async (classId) => {
    if (!classId) return;
    try {
      setLoadingExams(true);

      const qs = new URLSearchParams();
      qs.set("page", "1");
      qs.set("pageSize", "100");
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");

      const res = await callListExamsByClassAPI(classId, qs.toString());
      if (res?.success && res.data) {
        const api = res.data;
        const mapped =
          api.exams?.map((e) => ({
            id: e.examId,
            title: e.title,
            className: e.className,
          })) || [];

        setExams(mapped);

        if (!selectedExamId && mapped.length > 0) {
          setSelectedExamId(mapped[0].id);
        }
      } else {
        message.error("Không thể tải danh sách bài kiểm tra");
      }
    } catch (err) {
      console.error("fetchExams error:", err);
      message.error("Có lỗi khi tải danh sách bài kiểm tra");
    } finally {
      setLoadingExams(false);
    }
  };

  useEffect(() => {
    setSelectedExamId(null);
    if (selectedClassId) {
      fetchExams(selectedClassId);
      fetchStudents(selectedClassId);
    } else {
      setExams([]);
      setStudents([]);
    }
    setResults([]);
    setStats(null);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId]);

  /* =================== FETCH KẾT QUẢ CỦA 1 BÀI =================== */
  const fetchResults = async () => {
    if (!selectedClassId || !selectedExamId) return;

    try {
      setLoadingResults(true);

      // luôn lấy full kết quả, không phân trang
      const qs = new URLSearchParams();
      qs.set("page", "1");
      qs.set("pageSize", "1000");

      const res = await callGetExamResultsAPI(
        selectedClassId,
        selectedExamId,
        qs.toString()
      );
      console.log("fetchResults res:", res);
      if (res?.success && res.data) {
        const api = res.data;

        const mapped =
          api.results?.map((r) => {
            const correct = r.correctAnswers ?? 0;
            const totalQ = r.totalQuestions ?? r.totalMarks ?? 0;
            const score100 =
              totalQ > 0 ? Number(((correct * 100) / totalQ).toFixed(1)) : 0;

            return {
              id: r.attemptId,
              studentId: r.studentId,
              studentName: r.studentName,
              email: r.studentEmail,
              score100,
              correctAnswers: correct,
              totalQuestions: totalQ,
              status: r.status || "Completed",
              submittedAt: r.submitTime,
            };
          }) || [];

        setResults(mapped);
        setStats(api.statistics || null);
      } else {
        message.error("Không thể tải kết quả bài kiểm tra");
      }
    } catch (err) {
      console.error("fetchResults error:", err);
      message.error("Có lỗi khi tải kết quả bài kiểm tra");
    } finally {
      setLoadingResults(false);
    }
  };

  useEffect(() => {
    fetchResults();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId, selectedExamId]);

  /* =================== MERGE STUDENTS + RESULTS =================== */
  const mergedRows = useMemo(() => {
    if (!students.length) return results;

    return students.map((s) => {
      const r =
        results.find(
          (x) =>
            x.studentId === s.userId ||
            x.email?.toLowerCase() === s.email?.toLowerCase()
        ) || null;

      if (r) return r; // đã nộp

      // chưa có attempt → chưa nộp bài
      return {
        id: `no_attempt_${s.id}`,
        studentId: s.userId,
        studentName: s.name,
        email: s.email,
        score100: null,
        correctAnswers: 0,
        totalQuestions: 0,
        status: "NotStarted",
        submittedAt: null,
      };
    });
  }, [students, results]);

  // điểm trung bình chỉ tính học sinh đã làm bài
  const averageScore = useMemo(() => {
    const valid = mergedRows.filter((r) => r.totalQuestions > 0);
    if (!valid.length) return 0;

    const sum = valid.reduce((acc, r) => acc + (r.score100 || 0), 0);
    return Number((sum / valid.length).toFixed(1));
  }, [mergedRows]);

  const totalStudents = students.length || mergedRows.length;
  const submittedCount = mergedRows.filter((r) =>
    ["Completed", "Graded"].includes(r.status)
  ).length;

  /* =================== EXPORT =================== */
  const handleExport = () => {
    if (!mergedRows.length) {
      message.warning("Không có dữ liệu để xuất");
      return;
    }

    const headers = [
      "Học sinh",
      "Email",
      "Điểm",
      "Số câu đúng",
      "Tổng số câu",
      "Trạng thái",
      "Thời gian nộp",
    ];

    const rows = mergedRows.map((r) => {
      const scoreText =
        r.totalQuestions > 0 && r.score100 != null ? `${r.score100}/100` : "";
      return [
        r.studentName,
        r.email,
        scoreText,
        r.correctAnswers,
        r.totalQuestions,
        STATUS_TEXT[r.status] || r.status || "",
        r.submittedAt ? dayjs(r.submittedAt).format("DD/MM/YYYY HH:mm") : "",
      ];
    });

    const csv = [headers, ...rows]
      .map((row) =>
        row
          .map((cell) => {
            const s = String(cell ?? "");
            return /[",\n]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s;
          })
          .join(",")
      )
      .join("\n");

    const blob = new Blob([`\ufeff${csv}`], {
      type: "text/csv;charset=utf-8;",
    });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");

    const exam = exams.find((e) => e.id === selectedExamId);
    a.download = `ket-qua-${
      exam?.title?.toLowerCase().replace(/\s+/g, "-") || "bai-kiem-tra"
    }.csv`;
    a.href = url;
    a.click();
    URL.revokeObjectURL(url);
    message.success("Đã xuất CSV");
  };

  /* =================== COLUMNS =================== */
  const columns = [
    { title: "Học sinh", dataIndex: "studentName", key: "studentName" },
    { title: "Email", dataIndex: "email", key: "email" },
    {
      title: "Điểm",
      key: "score",
      width: 140,
      render: (_, row) =>
        row.totalQuestions ? (
          <span className={styles.bold}>{row.score100}</span>
        ) : (
          <span className={styles.muted}>-</span>
        ),
    },
    {
      title: "Số câu đúng",
      key: "correct",
      width: 140,
      render: (_, row) =>
        row.totalQuestions ? (
          `${row.correctAnswers}/${row.totalQuestions}`
        ) : (
          <span className={styles.muted}>-</span>
        ),
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      width: 150,
      render: (st) => (
        <Tag color={STATUS_COLOR[st] || "default"}>
          {STATUS_TEXT[st] || st || "-"}
        </Tag>
      ),
    },
    {
      title: "Thời gian nộp",
      dataIndex: "submittedAt",
      key: "submittedAt",
      width: 200,
      render: (v) =>
        v ? (
          dayjs(v).format("DD/MM/YYYY HH:mm")
        ) : (
          <span className={styles.muted}>-</span>
        ),
    },
  ];

  const currentClass = classes.find((c) => c.id === selectedClassId);
  const currentExam = exams.find((e) => e.id === selectedExamId);

  /* =================== RENDER =================== */
  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Kết quả bài kiểm tra
          </Title>
          <Text type="secondary">
            Xem điểm và trạng thái nộp bài của học sinh
          </Text>
        </div>

        <Button
          type="primary"
          icon={<Download size={16} />}
          onClick={handleExport}
          disabled={!mergedRows.length}
        >
          Xuất Excel
        </Button>
      </div>

      {/* Chọn lớp & bài kiểm tra */}
      <div className={styles.testSelect}>
        <Space wrap>
          <div>
            <Text className={styles.label}>Lớp học</Text>
            <Select
              loading={loadingClasses}
              value={selectedClassId ?? undefined}
              placeholder="Chọn lớp"
              style={{ minWidth: 260 }}
              onChange={(v) => {
                setSelectedClassId(v);
              }}
              options={classes.map((c) => ({
                value: c.id,
                label: `${c.name} (${c.code})`,
              }))}
            />
          </div>

          <div>
            <Text className={styles.label}>Bài kiểm tra</Text>
            <Select
              loading={loadingExams}
              value={selectedExamId ?? undefined}
              placeholder="Chọn bài kiểm tra"
              style={{ minWidth: 360 }}
              disabled={!selectedClassId}
              onChange={(v) => {
                setSelectedExamId(v);
              }}
              options={exams.map((e) => ({
                value: e.id,
                label: `${e.title} - ${e.className}`,
              }))}
            />
          </div>
        </Space>
      </div>

      {/* Thống kê nhanh */}
      {currentExam && (
        <div className={styles.kpis}>
          <Card className={styles.kpiCard}>
            <Text type="secondary">Lớp</Text>
            <div className={styles.kpiValue}>{currentClass?.name || "—"}</div>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Bài kiểm tra</Text>
            <div className={styles.kpiValue}>{currentExam.title}</div>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Tổng số học sinh</Text>
            <div className={styles.kpiValue}>{totalStudents}</div>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Đã nộp</Text>
            <div className={styles.kpiValue}>{submittedCount}</div>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Điểm trung bình</Text>
            <div className={styles.kpiValue}>{averageScore}/100</div>
          </Card>
        </div>
      )}

      {/* Bảng kết quả */}
      <div className={styles.tableCard}>
        {loadingResults ? (
          <div style={{ padding: 24 }}>
            <Text type="secondary">Đang tải kết quả...</Text>
          </div>
        ) : !selectedExamId ? (
          <Empty description="Hãy chọn lớp và bài kiểm tra để xem kết quả" />
        ) : mergedRows.length === 0 ? (
          <Empty description="Chưa có học sinh / kết quả nào" />
        ) : (
          <Table
            rowKey="id"
            dataSource={mergedRows}
            columns={columns}
            pagination={false}
          />
        )}
      </div>
    </div>
  );
}
