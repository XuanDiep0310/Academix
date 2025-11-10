import { useMemo, useState } from "react";
import {
  Table,
  Select,
  Typography,
  Space,
  Card,
  Tag,
  Button,
  Pagination,
  message,
} from "antd";
import { Download } from "lucide-react";
import styles from "../../assets/styles/ResultsView.module.scss";

const { Title, Text } = Typography;

/* ===================== DATASET MẪU NGAY TRONG FILE ===================== */
const TESTS = [
  {
    id: "1",
    title: "Kiểm tra giữa kỳ - Chương 1",
    className: "Toán cao cấp 1",
  },
  {
    id: "2",
    title: "Kiểm tra cuối kỳ - Tổng hợp",
    className: "Toán cao cấp 1",
  },
  { id: "3", title: "Bài tập tuần 5", className: "Đại số tuyến tính" },
];

function generateResults() {
  const arr = [];
  for (let i = 1; i <= 50; i++) {
    const submitted = i % 4 !== 0; // ~75% đã nộp
    const correct = submitted ? Math.floor(Math.random() * 11) : 0; // 0..10
    const hh = String(Math.floor(Math.random() * 12) + 8).padStart(2, "0");
    const mm = String(Math.floor(Math.random() * 60)).padStart(2, "0");
    const dd = String(Math.floor(Math.random() * 20) + 1).padStart(2, "0");

    arr.push({
      id: String(i),
      studentName: `Học sinh ${i}`,
      email: `student${i}@school.com`,
      score: submitted ? Number(((correct * 10) / 10).toFixed(1)) : 0, // thang 10
      totalQuestions: 10,
      correctAnswers: correct,
      status: submitted ? "submitted" : "not_submitted",
      submittedAt: submitted ? `2024-03-${dd} ${hh}:${mm}` : null,
    });
  }
  return arr;
}
/* ====================================================================== */

export default function ResultsView() {
  // Data cục bộ
  const [results, setResults] = useState(() => generateResults());

  // UI state
  const [selectedTest, setSelectedTest] = useState(TESTS[0].id);
  const [page, setPage] = useState(1);
  const pageSize = 10;

  // Thống kê
  const totalStudents = results.length;
  const submittedCount = useMemo(
    () => results.filter((r) => r.status === "submitted").length,
    [results]
  );
  const averageScore = useMemo(() => {
    if (!submittedCount) return "0";
    const sum = results
      .filter((r) => r.status === "submitted")
      .reduce((acc, r) => acc + r.score, 0);
    return (sum / submittedCount).toFixed(1);
  }, [results, submittedCount]);

  // Phân trang client
  const pagedData = useMemo(() => {
    const start = (page - 1) * pageSize;
    return results.slice(start, start + pageSize);
  }, [results, page]);

  // Export CSV (mở trực tiếp file tải về)
  const handleExport = () => {
    const headers = [
      "Học sinh",
      "Email",
      "Điểm",
      "Số câu đúng",
      "Tổng số câu",
      "Trạng thái",
      "Thời gian nộp",
    ];
    const rows = results.map((r) => [
      r.studentName,
      r.email,
      r.status === "submitted" ? `${r.score}/10` : "",
      r.status === "submitted" ? String(r.correctAnswers) : "",
      String(r.totalQuestions),
      r.status === "submitted" ? "Đã nộp" : "Chưa nộp",
      r.submittedAt || "",
    ]);

    const csv = [headers, ...rows]
      .map((row) =>
        row
          .map((cell) => {
            const s = String(cell ?? "");
            // Quote nếu có dấu phẩy/nháy
            if (/[",\n]/.test(s)) return `"${s.replace(/"/g, '""')}"`;
            return s;
          })
          .join(",")
      )
      .join("\n");

    const blob = new Blob([`\ufeff${csv}`], {
      type: "text/csv;charset=utf-8;",
    }); // BOM để Excel đọc UTF-8
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    const test = TESTS.find((t) => t.id === selectedTest);
    a.download = `ket-qua-${(test?.title || "bai-kiem-tra")
      .toLowerCase()
      .replace(/\s+/g, "-")}.csv`;
    a.click();
    URL.revokeObjectURL(url);
    message.success("Đã xuất CSV");
  };

  // Cột bảng
  const columns = [
    { title: "Học sinh", dataIndex: "studentName", key: "studentName" },
    { title: "Email", dataIndex: "email", key: "email" },
    {
      title: "Điểm",
      dataIndex: "score",
      key: "score",
      width: 120,
      render: (_, row) =>
        row.status === "submitted" ? (
          <span className={styles.bold}>{row.score}/10</span>
        ) : (
          <span className={styles.muted}>-</span>
        ),
    },
    {
      title: "Số câu đúng",
      key: "correct",
      width: 140,
      render: (_, row) =>
        row.status === "submitted" ? (
          `${row.correctAnswers}/${row.totalQuestions}`
        ) : (
          <span className={styles.muted}>-</span>
        ),
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      width: 140,
      render: (st) =>
        st === "submitted" ? (
          <Tag color="success">Đã nộp</Tag>
        ) : (
          <Tag>Chưa nộp</Tag>
        ),
    },
    {
      title: "Thời gian nộp",
      dataIndex: "submittedAt",
      key: "submittedAt",
      width: 180,
      render: (v) => v || <span className={styles.muted}>-</span>,
    },
  ];

  const currentTest = TESTS.find((t) => t.id === selectedTest);

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
        >
          Xuất Excel
        </Button>
      </div>

      {/* Chọn bài kiểm tra */}
      <div className={styles.testSelect}>
        <Text className={styles.label}>Chọn bài kiểm tra</Text>
        <Select
          value={selectedTest}
          onChange={(v) => {
            setSelectedTest(v);
            setPage(1);
            // Nếu muốn thay dataset theo bài kiểm tra, bạn có thể setResults(generateResults()) ở đây.
          }}
          style={{ width: 360 }}
          options={TESTS.map((t) => ({
            value: t.id,
            label: `${t.title} - ${t.className}`,
          }))}
        />
      </div>

      {/* Thống kê nhanh */}
      <div className={styles.kpis}>
        <Card className={styles.kpiCard}>
          <Text type="secondary">Tổng số học sinh</Text>
          <div className={styles.kpiValue}>{totalStudents}</div>
        </Card>

        <Card
          className={styles.kpiCard}
          style={{ background: "#f0fff4", borderColor: "#dcfce7" }}
        >
          <Text type="secondary">Đã nộp</Text>
          <div className={styles.kpiValue}>{submittedCount}</div>
        </Card>

        <Card
          className={styles.kpiCard}
          style={{ background: "#fff1f2", borderColor: "#ffe4e6" }}
        >
          <Text type="secondary">Chưa nộp</Text>
          <div className={styles.kpiValue}>
            {totalStudents - submittedCount}
          </div>
        </Card>

        <Card className={styles.kpiCard}>
          <Text type="secondary">Điểm trung bình</Text>
          <div className={styles.kpiValue}>{averageScore}</div>
        </Card>
      </div>

      {/* Bảng kết quả */}
      <div className={styles.tableCard}>
        <Table
          rowKey="id"
          dataSource={pagedData}
          columns={columns}
          pagination={false}
        />

        {results.length > pageSize && (
          <div className={styles.pagination}>
            <Pagination
              current={page}
              pageSize={pageSize}
              total={results.length}
              showSizeChanger={false}
              onChange={(p) => setPage(p)}
            />
          </div>
        )}
      </div>
    </div>
  );
}
