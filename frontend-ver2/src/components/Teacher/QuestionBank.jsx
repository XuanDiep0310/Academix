import { useMemo, useState } from "react";
import {
  Button,
  Modal,
  Form,
  Input,
  Select,
  Card,
  Tag,
  Typography,
  Space,
  Pagination,
  Radio,
  message,
  Empty,
} from "antd";
import { Plus, Pencil, Trash2, Filter } from "lucide-react";
import styles from "../../assets/styles/QuestionBank.module.scss";

const { Title, Text } = Typography;

/* ====================== DATASET MẪU NGAY TRONG FILE ====================== */
const SUBJECTS = [
  "Toán cao cấp 1",
  "Đại số tuyến tính",
  "Giải tích",
  "Xác suất thống kê",
  "Lập trình C++",
];

const DIFFICULTY_LABELS = { easy: "Dễ", medium: "Trung bình", hard: "Khó" };
const DIFFICULTY_COLORS = {
  easy: "default",
  medium: "processing",
  hard: "error",
};

function generateQuestions() {
  const subjects = SUBJECTS;
  const difficulties = ["easy", "medium", "hard"];
  const templates = [
    "Đạo hàm của hàm số f(x) = x² là gì?",
    "Tích phân của 1/x là gì?",
    "Giới hạn của hàm số khi x tiến tới vô cùng?",
    "Ma trận nghịch đảo là gì?",
    "Định thức của ma trận đơn vị bằng?",
    "Xác suất của biến cố chắc chắn là?",
    "Hàm lượng giác sin(0) bằng?",
    "Vector đơn vị có độ dài là?",
  ];
  const arr = [];
  subjects.forEach((subject, si) => {
    for (let i = 1; i <= 20; i++) {
      arr.push({
        id: `${si}-${i}`,
        question: `[${subject}] ${templates[i % templates.length]} (Câu ${i})`,
        options: ["Đáp án A", "Đáp án B", "Đáp án C", "Đáp án D"],
        correctAnswer: i % 4,
        subject,
        difficulty: difficulties[i % 3],
        createdAt: `2024-0${Math.min((i % 3) + 1, 9)}-${String(
          (i % 28) + 1
        ).padStart(2, "0")}`,
      });
    }
  });
  return arr;
}
/* ======================================================================= */

export default function QuestionBank() {
  // Data local
  const [questions, setQuestions] = useState(() => generateQuestions());

  // UI state
  const [filterSubject, setFilterSubject] = useState("all");
  const [page, setPage] = useState(1);
  const pageSize = 8;

  // Modal + form
  const [openEditor, setOpenEditor] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form] = Form.useForm();

  /* --------------------------- FILTER + PAGINATION --------------------------- */
  const filtered = useMemo(() => {
    if (filterSubject === "all") return questions;
    return questions.filter((q) => q.subject === filterSubject);
  }, [questions, filterSubject]);

  const total = filtered.length;
  const paged = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  /* --------------------------------- CRUD --------------------------------- */
  const openCreate = () => {
    setEditing(null);
    form.setFieldsValue({
      subject: SUBJECTS[0],
      difficulty: "medium",
      question: "",
      option1: "",
      option2: "",
      option3: "",
      option4: "",
      correctAnswer: 0,
    });
    setOpenEditor(true);
  };

  const openEdit = (row) => {
    setEditing(row);
    form.setFieldsValue({
      subject: row.subject,
      difficulty: row.difficulty,
      question: row.question,
      option1: row.options[0] || "",
      option2: row.options[1] || "",
      option3: row.options[2] || "",
      option4: row.options[3] || "",
      correctAnswer: row.correctAnswer,
    });
    setOpenEditor(true);
  };

  const onSubmit = async () => {
    const values = await form.validateFields();
    const payload = {
      id: editing?.id || String(Date.now()),
      question: values.question,
      options: [values.option1, values.option2, values.option3, values.option4],
      correctAnswer: values.correctAnswer,
      subject: values.subject,
      difficulty: values.difficulty,
      createdAt: editing?.createdAt || new Date().toISOString().split("T")[0],
    };

    if (editing) {
      setQuestions((prev) =>
        prev.map((q) => (q.id === editing.id ? payload : q))
      );
      message.success("Đã cập nhật câu hỏi");
    } else {
      setQuestions((prev) => [payload, ...prev]);
      setPage(1);
      message.success("Đã thêm câu hỏi");
    }
    setOpenEditor(false);
    setEditing(null);
    form.resetFields();
  };

  const onDelete = (id) => {
    setQuestions((prev) => prev.filter((q) => q.id !== id));
    message.success("Đã xóa câu hỏi");
  };

  /* --------------------------------- RENDER -------------------------------- */
  return (
    <div className={styles.wrap}>
      {/* Header + Filter */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Ngân hàng câu hỏi
          </Title>
          <Text type="secondary">
            Quản lý câu hỏi trắc nghiệm cho bài kiểm tra
          </Text>
        </div>

        <Space>
          <Select
            value={filterSubject}
            onChange={(v) => {
              setFilterSubject(v);
              setPage(1);
            }}
            style={{ width: 240 }}
            suffixIcon={<Filter size={16} />}
            options={[
              { value: "all", label: "Tất cả môn học" },
              ...SUBJECTS.map((s) => ({ value: s, label: s })),
            ]}
          />
          <Button type="primary" icon={<Plus size={16} />} onClick={openCreate}>
            Thêm câu hỏi
          </Button>
        </Space>
      </div>

      {/* List */}
      <div className={styles.grid}>
        {paged.length === 0 ? (
          <Empty description="Chưa có câu hỏi" />
        ) : (
          paged.map((q) => (
            <Card key={q.id} className={styles.card} bordered>
              <div className={styles.cardHeader}>
                <div className={styles.cardMeta}>
                  <div className={styles.question}>{q.question}</div>
                  <Space size={8} wrap>
                    <Tag>{q.subject}</Tag>
                    <Tag color={DIFFICULTY_COLORS[q.difficulty]}>
                      {DIFFICULTY_LABELS[q.difficulty]}
                    </Tag>
                  </Space>
                </div>

                <Space>
                  <Button
                    size="small"
                    type="primary"
                    ghost
                    icon={<Pencil size={16} />}
                    onClick={() => openEdit(q)}
                  >
                    Sửa
                  </Button>
                  <Button
                    size="small"
                    danger
                    icon={<Trash2 size={16} />}
                    onClick={() => onDelete(q.id)}
                  >
                    Xóa
                  </Button>
                </Space>
              </div>

              <div className={styles.options}>
                {q.options.map((opt, idx) => {
                  const isCorrect = idx === q.correctAnswer;
                  return (
                    <div
                      key={idx}
                      className={`${styles.option} ${
                        isCorrect ? styles.correct : ""
                      }`}
                    >
                      <span className={styles.optionLabel}>
                        {String.fromCharCode(65 + idx)}.
                      </span>
                      <span>{opt}</span>
                      {isCorrect && (
                        <Tag color="success" className={styles.answerTag}>
                          Đáp án đúng
                        </Tag>
                      )}
                    </div>
                  );
                })}
              </div>
            </Card>
          ))
        )}
      </div>

      {/* Pagination */}
      {total > pageSize && (
        <div className={styles.pagination}>
          <Pagination
            current={page}
            pageSize={pageSize}
            total={total}
            showSizeChanger={false}
            onChange={(p) => setPage(p)}
          />
        </div>
      )}

      {/* Modal thêm/sửa */}
      <Modal
        title={editing ? "Chỉnh sửa câu hỏi" : "Thêm câu hỏi mới"}
        open={openEditor}
        onCancel={() => {
          setOpenEditor(false);
          setEditing(null);
          form.resetFields();
        }}
        onOk={onSubmit}
        okText={editing ? "Cập nhật" : "Thêm câu hỏi"}
        destroyOnClose
        width={720}
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            subject: SUBJECTS[0],
            difficulty: "medium",
            question: "",
            option1: "",
            option2: "",
            option3: "",
            option4: "",
            correctAnswer: 0,
          }}
        >
          <Form.Item
            label="Môn học"
            name="subject"
            rules={[{ required: true }]}
          >
            <Select options={SUBJECTS.map((s) => ({ value: s, label: s }))} />
          </Form.Item>

          <Form.Item
            label="Độ khó"
            name="difficulty"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: "easy", label: "Dễ" },
                { value: "medium", label: "Trung bình" },
                { value: "hard", label: "Khó" },
              ]}
            />
          </Form.Item>

          <Form.Item
            label="Câu hỏi"
            name="question"
            rules={[{ required: true, message: "Vui lòng nhập câu hỏi" }]}
          >
            <Input.TextArea rows={3} placeholder="Nhập nội dung câu hỏi..." />
          </Form.Item>

          {/* Đáp án */}
          <div className={styles.answerBlock}>
            <div className={styles.answerHeader}>
              <Text strong>Đáp án</Text>
              <Text type="secondary" style={{ fontSize: 12 }}>
                Chọn đáp án đúng (A/B/C/D)
              </Text>
            </div>

            <Form.Item
              name="correctAnswer"
              rules={[{ required: true }]}
              noStyle
            >
              <Radio.Group className={styles.radioRow}>
                {[1, 2, 3, 4].map((n) => (
                  <div key={n} className={styles.answerRow}>
                    <Radio value={n - 1} className={styles.radioOnly} />
                    <Form.Item
                      name={`option${n}`}
                      rules={[{ required: true, message: `Nhập đáp án ${n}` }]}
                      className={styles.answerInput}
                    >
                      <Input placeholder={`Đáp án ${n}`} />
                    </Form.Item>
                  </div>
                ))}
              </Radio.Group>
            </Form.Item>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
