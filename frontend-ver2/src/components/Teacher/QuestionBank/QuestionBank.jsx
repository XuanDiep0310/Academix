import { useEffect, useMemo, useState } from "react";
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
import styles from "../../../assets/styles/QuestionBank.module.scss";
import {
  callListQuestionBankAPI,
  callCreateQuestionAPI,
  deleteQuestionAPI,
  editQuestionAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;

const DIFFICULTY_LABELS = { easy: "Dễ", medium: "Trung bình", hard: "Khó" };
const DIFFICULTY_COLORS = {
  easy: "default",
  medium: "processing",
  hard: "error",
};

const mapDifficultyToApi = (d) => {
  switch (d) {
    case "easy":
      return "Easy";
    case "hard":
      return "Hard";
    default:
      return "Medium";
  }
};

export default function QuestionBank() {
  const [questions, setQuestions] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(false);

  const [filterSubject, setFilterSubject] = useState("all");
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(8);

  const [openEditor, setOpenEditor] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form] = Form.useForm();
  const questionType = Form.useWatch("questionType", form);

  // ======================= FETCH API =======================

  const fetchQuestions = async () => {
    try {
      setLoading(true);

      const query = `page=1&pageSize=1000&sortBy=CreatedAt&sortOrder=desc`;
      const res = await callListQuestionBankAPI(query);

      if (res?.success && res.data) {
        const api = res.data;

        const mapped =
          api.questions?.map((q) => {
            const opts =
              q.options || q.questionOptions || q.answerOptions || [];

            const correctIndex = opts.findIndex((o) => o.isCorrect);

            const difficultyRaw = String(
              q.difficultyLevel || q.difficulty || "Medium"
            ).toLowerCase();

            const difficulty = ["easy", "medium", "hard"].includes(
              difficultyRaw
            )
              ? difficultyRaw
              : "medium";

            return {
              id: q.questionId,
              question: q.questionText || "",
              options: opts.map((o) => o.optionText),
              correctAnswer: correctIndex >= 0 ? correctIndex : 0,
              subject: q.subject || "Chưa gán môn",
              difficulty,
              questionType: q.questionType || "MultipleChoice",
              createdAt: q.createdAt,
            };
          }) || [];

        setQuestions(mapped);

        const uniqueSubjects = Array.from(
          new Set(mapped.map((x) => x.subject).filter(Boolean))
        );

        setSubjects(uniqueSubjects);
      } else {
        message.error("Không thể tải ngân hàng câu hỏi");
      }
    } catch (err) {
      console.error(err);
      message.error("Có lỗi xảy ra khi tải ngân hàng câu hỏi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQuestions();
  }, []);

  // ======================= FILTER + PAGINATION =======================

  const filtered = useMemo(() => {
    if (filterSubject === "all") return questions;
    return questions.filter((q) => q.subject === filterSubject);
  }, [questions, filterSubject]);

  const total = filtered.length;

  const paged = useMemo(() => {
    const start = (current - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, current, pageSize]);

  const handleOnChangePagi = (page, pageSizeNew) => {
    if (pageSizeNew && +pageSizeNew !== +pageSize) {
      setPageSize(+pageSizeNew);
      setCurrent(1);
    } else if (page && +page !== +current) {
      setCurrent(+page);
    }
  };

  // ======================= CREATE / EDIT =======================

  const openCreate = () => {
    setEditing(null);
    form.setFieldsValue({
      subject: subjects[0],
      difficulty: "medium",
      questionType: "MultipleChoice",
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
      questionType: row.questionType,
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
    const qType = values.questionType;

    let raw = [values.option1, values.option2, values.option3, values.option4];

    if (qType === "TrueFalse") raw = raw.slice(0, 2);

    const options = raw.map((text, idx) => ({
      optionText: text,
      isCorrect: idx === values.correctAnswer,
      optionOrder: idx + 1,
    }));

    const apiBody = {
      questionText: values.question,
      questionType: qType,
      difficultyLevel: mapDifficultyToApi(values.difficulty),
      subject: values.subject,
      options,
    };

    if (editing) {
      const res = await editQuestionAPI(editing.id, apiBody);
      if (res?.success) {
        message.success("Đã cập nhật");
        await fetchQuestions();
      } else return message.error("Cập nhật thất bại");
    } else {
      const res = await callCreateQuestionAPI(apiBody);
      if (res?.success) {
        message.success("Đã tạo câu hỏi");
        setCurrent(1); // trả về trang đầu tiên
        await fetchQuestions();
      } else return message.error("Tạo câu hỏi thất bại");
    }

    setOpenEditor(false);
    setEditing(null);
    form.resetFields();
  };

  const onDelete = async (id) => {
    const res = await deleteQuestionAPI(id);
    if (res?.success) {
      message.success("Đã xóa");
      await fetchQuestions();
    } else message.error("Xóa thất bại");
  };

  const optionCount = questionType === "TrueFalse" ? 2 : 4;
  const tfLabels = ["Đúng", "Sai"];

  // ======================= RENDER =======================

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <div>
          <Title level={4}>Ngân hàng câu hỏi</Title>
          <Text type="secondary">Quản lý câu hỏi trắc nghiệm</Text>
        </div>

        <Space>
          <Select
            value={filterSubject}
            style={{ width: 240 }}
            suffixIcon={<Filter size={16} />}
            onChange={(v) => {
              setFilterSubject(v);
              setCurrent(1);
            }}
            options={[
              { value: "all", label: "Tất cả môn học" },
              ...subjects.map((s) => ({ value: s, label: s })),
            ]}
          />

          <Button type="primary" icon={<Plus size={16} />} onClick={openCreate}>
            Thêm câu hỏi
          </Button>
        </Space>
      </div>

      {/* LIST */}
      <div className={styles.grid}>
        {loading ? (
          <div className={styles.loadingWrap}>Đang tải...</div>
        ) : paged.length === 0 ? (
          <Empty description="Không có câu hỏi" />
        ) : (
          paged.map((q) => (
            <Card key={q.id} className={styles.card} bordered>
              <div className={styles.cardHeader}>
                <div className={styles.cardMeta}>
                  <div className={styles.question}>{q.question}</div>

                  <Space size={8}>
                    <Tag>{q.subject}</Tag>
                    <Tag color={DIFFICULTY_COLORS[q.difficulty]}>
                      {DIFFICULTY_LABELS[q.difficulty]}
                    </Tag>
                    <Tag color="blue">
                      {q.questionType === "TrueFalse"
                        ? "Đúng / Sai"
                        : q.questionType === "SingleChoice"
                        ? "Một lựa chọn"
                        : "Nhiều lựa chọn"}
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
                      {isCorrect && <Tag color="success">Đáp án đúng</Tag>}
                    </div>
                  );
                })}
              </div>
            </Card>
          ))
        )}
      </div>

      {/* PAGINATION */}
      {total > 0 && (
        <div className={styles.pagination}>
          <Pagination
            current={current}
            pageSize={pageSize}
            total={total}
            showSizeChanger
            pageSizeOptions={[4, 8, 12, 20]}
            onChange={handleOnChangePagi}
            onShowSizeChange={handleOnChangePagi}
          />
        </div>
      )}

      {/* MODAL */}
      <Modal
        title={editing ? "Chỉnh sửa câu hỏi" : "Thêm câu hỏi"}
        open={openEditor}
        onCancel={() => {
          setOpenEditor(false);
          setEditing(null);
          form.resetFields();
        }}
        onOk={onSubmit}
        width={720}
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            subject: subjects[0],
            difficulty: "medium",
            questionType: "MultipleChoice",
            correctAnswer: 0,
          }}
        >
          <Form.Item
            name="subject"
            label="Môn học"
            rules={[{ required: true, message: "Chọn môn học" }]}
          >
            <Select
              placeholder="Chọn môn học"
              options={subjects.map((s) => ({ value: s, label: s }))}
            />
          </Form.Item>

          <Form.Item name="difficulty" label="Độ khó">
            <Select
              options={[
                { value: "easy", label: "Dễ" },
                { value: "medium", label: "Trung bình" },
                { value: "hard", label: "Khó" },
              ]}
            />
          </Form.Item>

          <Form.Item name="questionType" label="Loại câu hỏi">
            <Select
              options={[
                { value: "MultipleChoice", label: "Nhiều lựa chọn" },
                { value: "SingleChoice", label: "Một lựa chọn" },
                { value: "TrueFalse", label: "Đúng / Sai" },
              ]}
            />
          </Form.Item>

          <Form.Item
            name="question"
            label="Câu hỏi"
            rules={[{ required: true, message: "Nhập câu hỏi" }]}
          >
            <Input.TextArea rows={3} />
          </Form.Item>

          {/* ANSWER */}
          <div className={styles.answerBlock}>
            <div className={styles.answerHeader}>
              <Text strong>Đáp án</Text>
              <Text type="secondary" style={{ fontSize: 12 }}>
                {questionType === "TrueFalse"
                  ? "Chọn đúng / sai"
                  : "Chọn đáp án đúng"}
              </Text>
            </div>

            <Form.Item name="correctAnswer" noStyle>
              <Radio.Group className={styles.radioRow}>
                {Array.from({ length: optionCount }, (_, i) => i + 1).map(
                  (n) => (
                    <div key={n} className={styles.answerRow}>
                      <Radio value={n - 1} />
                      <Form.Item
                        name={`option${n}`}
                        rules={[{ required: true, message: "Nhập đáp án" }]}
                        className={styles.answerInput}
                      >
                        <Input
                          placeholder={
                            questionType === "TrueFalse"
                              ? tfLabels[n - 1]
                              : `Đáp án ${n}`
                          }
                        />
                      </Form.Item>
                    </div>
                  )
                )}
              </Radio.Group>
            </Form.Item>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
