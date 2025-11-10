import { useMemo, useState } from "react";
import {
  Button,
  Card,
  Typography,
  Tag,
  Space,
  Pagination,
  Modal,
  Form,
  Input,
  Select,
  InputNumber,
  DatePicker,
  Checkbox,
  message,
  Empty,
} from "antd";
import dayjs from "dayjs";
import { Plus, Clock, Users, Calendar, Pencil } from "lucide-react";
import styles from "../../assets/styles/TestManagement.module.scss";

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;

/* ========================= BASE DATA (no API) ========================= */
const SUBJECTS = [
  "Toán cao cấp 1",
  "Đại số tuyến tính",
  "Giải tích",
  "Xác suất thống kê",
  "Lập trình C++",
];

const STATUS_LABELS = {
  draft: "Bản nháp",
  published: "Đã công bố",
  closed: "Đã đóng",
};
const STATUS_COLORS = {
  draft: "default",
  published: "success",
  closed: "default",
};

function generateMockQuestionsForTest() {
  const subjects = SUBJECTS;
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
  const questions = [];
  subjects.forEach((subject, subjectIdx) => {
    for (let i = 1; i <= 20; i++) {
      questions.push({
        id: `${subjectIdx}-${i}`,
        question: `[${subject}] ${templates[i % templates.length]} (Câu ${i})`,
        subject,
      });
    }
  });
  return questions;
}

function generateMockTests() {
  const tests = [];
  const subjects = SUBJECTS;
  const statuses = ["draft", "published", "closed"];
  for (let i = 1; i <= 25; i++) {
    const subject = subjects[i % subjects.length];
    tests.push({
      id: String(i),
      title: `Kiểm tra ${subject} - Chương ${Math.floor(i / 5) + 1}`,
      classId: String((i % 3) + 1),
      className: `Lớp ${subject}`,
      duration: 30 + (i % 6) * 15, // phút
      questionIds: [`${i % 5}-1`, `${i % 5}-2`],
      status: statuses[i % 3], // draft | published | closed
      createdAt: `2024-0${Math.min((i % 3) + 1, 9)}-${String(
        (i % 28) + 1
      ).padStart(2, "0")}`,
      startTime: `2024-03-${String((i % 28) + 1).padStart(2, "0")}T08:00:00`,
      endTime: `2024-03-${String((i % 28) + 5).padStart(2, "0")}T23:59:00`,
      subject,
    });
  }
  return tests;
}

const ALL_QUESTIONS = generateMockQuestionsForTest();
/* ===================================================================== */

export default function TestManagement() {
  const [tests, setTests] = useState(() => generateMockTests());
  const [page, setPage] = useState(1);
  const pageSize = 6;

  const [openEditor, setOpenEditor] = useState(false);
  const [editing, setEditing] = useState(null);

  // form
  const [form] = Form.useForm();
  const subjectValue = Form.useWatch("subject", form);

  // questions filtered by subject
  const filteredQuestions = useMemo(() => {
    const subj = subjectValue || SUBJECTS[0];
    return ALL_QUESTIONS.filter((q) => q.subject === subj);
  }, [subjectValue]);

  // pagination
  const total = tests.length;
  const current = useMemo(() => {
    const start = (page - 1) * pageSize;
    return tests.slice(start, start + pageSize);
  }, [tests, page]);

  /* ----------------------------- CRUD actions ----------------------------- */
  const openCreate = () => {
    setEditing(null);
    form.setFieldsValue({
      title: "",
      classId: "1",
      subject: SUBJECTS[0],
      duration: 45,
      timeRange: null,
      questionIds: [],
    });
    setOpenEditor(true);
  };

  const openEdit = (t) => {
    setEditing(t);
    form.setFieldsValue({
      title: t.title,
      classId: t.classId,
      subject: t.subject,
      duration: t.duration,
      timeRange: [dayjs(t.startTime), dayjs(t.endTime)],
      questionIds: t.questionIds,
    });
    setOpenEditor(true);
  };

  const onSubmit = async () => {
    const values = await form.validateFields();
    const [start, end] = values.timeRange || [];
    const payload = {
      id: editing?.id || String(Date.now()),
      title: values.title,
      classId: values.classId,
      className:
        values.classId === "1" ? "Toán cao cấp 1" : "Đại số tuyến tính", // demo hiển thị
      duration: Number(values.duration),
      questionIds: values.questionIds || [],
      status: editing?.status || "draft",
      createdAt: editing?.createdAt || new Date().toISOString().split("T")[0],
      startTime: start ? start.toISOString() : "",
      endTime: end ? end.toISOString() : "",
      subject: values.subject,
    };

    if (editing) {
      setTests((prev) => prev.map((x) => (x.id === editing.id ? payload : x)));
      message.success("Đã cập nhật bài kiểm tra");
    } else {
      setTests((prev) => [payload, ...prev]);
      setPage(1);
      message.success("Đã tạo bài kiểm tra mới");
    }
    setOpenEditor(false);
    setEditing(null);
    form.resetFields();
  };

  const publishTest = (id) => {
    setTests((prev) =>
      prev.map((x) => (x.id === id ? { ...x, status: "published" } : x))
    );
    message.success("Đã công bố bài kiểm tra");
  };

  const closeTest = (id) => {
    setTests((prev) =>
      prev.map((x) => (x.id === id ? { ...x, status: "closed" } : x))
    );
    message.success("Đã đóng bài kiểm tra");
  };

  const formatDateTimeVN = (iso) =>
    iso ? dayjs(iso).format("DD/MM/YYYY HH:mm") : "-";

  /* -------------------------------- RENDER -------------------------------- */
  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Quản lý bài kiểm tra
          </Title>
          <Text type="secondary">Tạo và quản lý bài kiểm tra trắc nghiệm</Text>
        </div>
        <Button type="primary" icon={<Plus size={16} />} onClick={openCreate}>
          Tạo bài kiểm tra
        </Button>
      </div>

      {/* Cards list */}
      <div className={styles.grid}>
        {current.length === 0 ? (
          <Empty description="Chưa có bài kiểm tra" />
        ) : (
          current.map((t) => (
            <Card key={t.id} className={styles.card} bordered>
              <div className={styles.cardHeader}>
                <div className={styles.meta}>
                  <div className={styles.cardTitle}>{t.title}</div>
                  <Space size={6} wrap>
                    <Tag>{t.className}</Tag>
                    <Tag>{t.subject}</Tag>
                    <Tag color={STATUS_COLORS[t.status]}>
                      {STATUS_LABELS[t.status]}
                    </Tag>
                  </Space>
                </div>

                <Space>
                  {t.status === "draft" && (
                    <>
                      <Button
                        size="small"
                        icon={<Pencil size={16} />}
                        onClick={() => openEdit(t)}
                      >
                        Sửa
                      </Button>
                      <Button
                        size="small"
                        type="primary"
                        onClick={() => publishTest(t.id)}
                      >
                        Công bố
                      </Button>
                    </>
                  )}
                  {t.status === "published" && (
                    <Button size="small" onClick={() => closeTest(t.id)}>
                      Đóng bài
                    </Button>
                  )}
                </Space>
              </div>

              <div className={styles.bodyGrid}>
                <div className={styles.inline}>
                  <Clock size={16} />
                  <span>{t.duration} phút</span>
                </div>
                <div className={styles.inline}>
                  <Users size={16} />
                  <span>{t.questionIds.length} câu hỏi</span>
                </div>
                <div className={styles.inlineWide}>
                  <Calendar size={16} />
                  <div>
                    <div>Bắt đầu: {formatDateTimeVN(t.startTime)}</div>
                    <div>Kết thúc: {formatDateTimeVN(t.endTime)}</div>
                  </div>
                </div>
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

      {/* Modal create/edit */}
      <Modal
        title={editing ? "Chỉnh sửa bài kiểm tra" : "Tạo bài kiểm tra mới"}
        open={openEditor}
        onCancel={() => {
          setOpenEditor(false);
          setEditing(null);
          form.resetFields();
        }}
        onOk={onSubmit}
        okText={editing ? "Cập nhật" : "Tạo bài kiểm tra"}
        width={720}
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            title: "",
            classId: "1",
            subject: SUBJECTS[0],
            duration: 45,
            timeRange: null,
            questionIds: [],
          }}
        >
          <Form.Item
            label="Tiêu đề"
            name="title"
            rules={[{ required: true, message: "Nhập tiêu đề" }]}
          >
            <Input placeholder="VD: Kiểm tra giữa kỳ - Chương 1" />
          </Form.Item>

          <Form.Item
            label="Lớp học"
            name="classId"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: "1", label: "Toán cao cấp 1" },
                { value: "2", label: "Đại số tuyến tính" },
                { value: "3", label: "Giải tích" },
              ]}
            />
          </Form.Item>

          <Form.Item
            label="Môn học"
            name="subject"
            rules={[{ required: true }]}
          >
            <Select options={SUBJECTS.map((s) => ({ value: s, label: s }))} />
          </Form.Item>
          <Text
            type="secondary"
            style={{ display: "block", marginTop: -8, marginBottom: 8 }}
          >
            Câu hỏi sẽ được lọc theo môn học đã chọn
          </Text>

          <Form.Item
            label="Thời lượng (phút)"
            name="duration"
            rules={[{ required: true }]}
          >
            <InputNumber min={1} style={{ width: "100%" }} />
          </Form.Item>

          <Form.Item
            label="Khoảng thời gian làm bài"
            name="timeRange"
            rules={[
              { required: true, message: "Chọn thời gian bắt đầu/kết thúc" },
            ]}
          >
            <RangePicker
              showTime
              format="DD/MM/YYYY HH:mm"
              style={{ width: "100%" }}
            />
          </Form.Item>
          <Text
            type="secondary"
            style={{ display: "block", marginTop: -8, marginBottom: 8 }}
          >
            Học sinh chỉ có thể làm bài trong khoảng thời gian này
          </Text>

          <Form.Item
            label={`Chọn câu hỏi (${
              form.getFieldValue("subject") || SUBJECTS[0]
            })`}
            name="questionIds"
          >
            <div className={styles.questionBox}>
              {filteredQuestions.length === 0 ? (
                <Text type="secondary">Chưa có câu hỏi cho môn học này</Text>
              ) : (
                <Checkbox.Group style={{ width: "100%" }}>
                  {filteredQuestions.map((q) => (
                    <div key={q.id} className={styles.qRow}>
                      <Form.Item noStyle shouldUpdate>
                        {() => (
                          <Checkbox
                            value={q.id}
                            checked={(
                              form.getFieldValue("questionIds") || []
                            ).includes(q.id)}
                            onChange={(e) => {
                              const curr =
                                form.getFieldValue("questionIds") || [];
                              if (e.target.checked)
                                form.setFieldValue("questionIds", [
                                  ...curr,
                                  q.id,
                                ]);
                              else
                                form.setFieldValue(
                                  "questionIds",
                                  curr.filter((x) => x !== q.id)
                                );
                            }}
                          >
                            {q.question}
                          </Checkbox>
                        )}
                      </Form.Item>
                    </div>
                  ))}
                </Checkbox.Group>
              )}
            </div>
          </Form.Item>

          <div className={styles.selectedCount}>
            Đã chọn: {(form.getFieldValue("questionIds") || []).length} câu hỏi
          </div>
        </Form>
      </Modal>
    </div>
  );
}
