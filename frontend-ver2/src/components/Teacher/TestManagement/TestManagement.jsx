import { useEffect, useMemo, useState } from "react";
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
  InputNumber,
  DatePicker,
  Select,
  message,
  Empty,
} from "antd";
import dayjs from "dayjs";
import { Plus, Clock, Users, Calendar, Pencil, Trash2 } from "lucide-react";
import styles from "../../../assets/styles/TestManagement.module.scss";
import {
  callListMyClassesAPI,
  callListExamsByClassAPI,
  callCreateExamAPI,
  callUpdateExamAPI,
  callDeleteExamAPI,
  callPublishExamAPI,
  callListQuestionBankAPI,
  callGetExamQuestionsAPI,
  callUpsertExamQuestionsAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;

/* ========================= CONST ========================= */

const STATUS_LABELS = {
  Draft: "Bản nháp",
  Published: "Đã công bố",
  Closed: "Đã đóng",
};

const STATUS_COLORS = {
  Draft: "default",
  Published: "success",
  Closed: "default",
};

/* ========================= COMPONENT ========================= */

export default function TestManagement() {
  /* ---------- Lớp học của tôi ---------- */
  const [classes, setClasses] = useState([]);
  const [loadingClasses, setLoadingClasses] = useState(false);
  const [selectedClassId, setSelectedClassId] = useState(null);

  /* ---------- Exams ---------- */
  const [exams, setExams] = useState([]);
  const [loadingExams, setLoadingExams] = useState(false);
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(6);
  const [total, setTotal] = useState(0);

  /* ---------- Modal create/edit ---------- */
  const [openEditor, setOpenEditor] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form] = Form.useForm();

  /* ---------- Ngân hàng câu hỏi ---------- */
  const [bankQuestions, setBankQuestions] = useState([]);
  const [loadingBank, setLoadingBank] = useState(false);
  const [bankSubjectFilter, setBankSubjectFilter] = useState("all");

  const filteredBankQuestions = useMemo(() => {
    if (bankSubjectFilter === "all") return bankQuestions;
    return bankQuestions.filter((q) => q.subject === bankSubjectFilter);
  }, [bankQuestions, bankSubjectFilter]);

  const bankSubjects = useMemo(
    () =>
      Array.from(new Set(bankQuestions.map((q) => q.subject).filter(Boolean))),
    [bankQuestions]
  );

  /* ========================= FETCH LỚP HỌC ========================= */

  const fetchMyClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();
      if (res && res.success && Array.isArray(res.data)) {
        const mapped = res.data.map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
        }));
        setClasses(mapped);
        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].id);
        }
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchMyClasses error:", err);
      message.error("Có lỗi khi tải danh sách lớp học");
    } finally {
      setLoadingClasses(false);
    }
  };

  useEffect(() => {
    fetchMyClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  /* ========================= FETCH EXAMS ========================= */

  const fetchExams = async () => {
    if (!selectedClassId) return;
    try {
      setLoadingExams(true);
      const qs = new URLSearchParams();
      qs.set("page", String(current));
      qs.set("pageSize", String(pageSize));
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");

      const res = await callListExamsByClassAPI(selectedClassId, qs.toString());

      if (res && res.success && res.data) {
        const api = res.data;
        const mapped =
          api.exams?.map((e) => ({
            id: e.examId,
            title: e.title,
            description: e.description,
            classId: e.classId,
            className: e.className,
            duration: e.duration,
            totalMarks: e.totalMarks,
            startTime: e.startTime,
            endTime: e.endTime,
            createdAt: e.createdAt,
            isPublished: !!e.isPublished,
            status: e.status || (e.isPublished ? "Published" : "Draft"),
            questionCount: e.questionCount ?? 0,
          })) || [];

        setExams(mapped);
        setTotal(api.totalCount ?? mapped.length);
      } else {
        message.error("Không thể tải danh sách bài kiểm tra");
      }
    } catch (err) {
      console.error("fetchExams error:", err);
      message.error("Có lỗi khi tải bài kiểm tra");
    } finally {
      setLoadingExams(false);
    }
  };

  useEffect(() => {
    fetchExams();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId, current, pageSize]);

  /* ========================= QUESTION BANK ========================= */

  const fetchQuestionBank = async () => {
    try {
      setLoadingBank(true);
      const query = "page=1&pageSize=1000&sortBy=UpdatedAt&sortOrder=desc";
      const res = await callListQuestionBankAPI(query);

      if (res && res.success && res.data) {
        const api = res.data;
        const mapped =
          api.questions?.map((q) => ({
            id: q.questionId,
            text: q.questionText || q.content || "",
            subject: q.subject || "Chưa gán môn",
            questionType: q.questionType || "MultipleChoice",
          })) || [];
        setBankQuestions(mapped);
      } else {
        message.error("Không thể tải ngân hàng câu hỏi");
      }
    } catch (err) {
      console.error("fetchQuestionBank error:", err);
      message.error("Có lỗi khi tải ngân hàng câu hỏi");
    } finally {
      setLoadingBank(false);
    }
  };

  const fetchExamQuestions = async (examId) => {
    if (!selectedClassId || !examId) return;
    try {
      const res = await callGetExamQuestionsAPI(selectedClassId, examId);
      if (res && res.success && Array.isArray(res.data)) {
        const ids = res.data.map((x) => x.questionId);
        form.setFieldsValue({
          questionIds: ids,
        });
      }
    } catch (err) {
      console.error("fetchExamQuestions error:", err);
      message.error("Không thể tải câu hỏi của bài kiểm tra");
    }
  };

  /* ========================= PAGINATION ========================= */

  const handleOnChangePagi = (page, pageSizeNew) => {
    if (pageSizeNew && +pageSizeNew !== +pageSize) {
      setPageSize(+pageSizeNew);
      setCurrent(1);
    } else if (page && +page !== +current) {
      setCurrent(+page);
    }
  };

  /* ========================= CRUD EXAM ========================= */

  const openCreate = async () => {
    if (!selectedClassId) {
      message.warning("Vui lòng chọn lớp trước khi tạo bài kiểm tra");
      return;
    }
    setEditing(null);
    setBankSubjectFilter("all");

    form.setFieldsValue({
      title: "",
      description: "",
      duration: 45,
      totalMarks: 0,
      timeRange: null,
      questionIds: [],
    });

    setOpenEditor(true);
    await fetchQuestionBank();
  };

  const openEdit = async (exam) => {
    setEditing(exam);
    setBankSubjectFilter("all");

    form.setFieldsValue({
      title: exam.title,
      description: exam.description,
      duration: exam.duration,
      totalMarks: exam.totalMarks,
      timeRange: [
        exam.startTime ? dayjs(exam.startTime) : null,
        exam.endTime ? dayjs(exam.endTime) : null,
      ],
      questionIds: [],
    });

    setOpenEditor(true);

    // load bank + câu hỏi của bài kiểm tra
    await fetchQuestionBank();
    await fetchExamQuestions(exam.id);
  };

  const onSubmit = async () => {
    if (!selectedClassId) {
      message.error("Thiếu classId");
      return;
    }

    const values = await form.validateFields();
    const [start, end] = values.timeRange || [];
    const questionIds = values.questionIds || [];

    const totalMarks = questionIds.length; // mỗi câu 1 điểm cho dễ

    const questionsPayload = questionIds.map((qId, index) => ({
      questionId: qId,
      questionOrder: index + 1,
      marks: 1,
    }));

    const bodyExam = {
      title: values.title,
      description: values.description,
      duration: Number(values.duration),
      totalMarks,
      startTime: start ? start.toISOString() : null,
      endTime: end ? end.toISOString() : null,
    };

    try {
      if (editing) {
        // 1) update info bài kiểm tra
        const resExam = await callUpdateExamAPI(
          selectedClassId,
          editing.id,
          bodyExam
        );
        if (!resExam || !resExam.success) {
          message.error(
            resExam?.message || "Cập nhật thông tin bài kiểm tra thất bại"
          );
          return;
        }

        // 2) set lại danh sách câu hỏi
        const resQ = await callUpsertExamQuestionsAPI(
          selectedClassId,
          editing.id,
          {
            questions: questionsPayload,
          }
        );

        if (!resQ || !resQ.success) {
          message.error(resQ?.message || "Cập nhật danh sách câu hỏi thất bại");
          return;
        }

        message.success("Đã cập nhật bài kiểm tra");
      } else {
        // tạo mới: backend cho phép gửi luôn questions trong body
        const res = await callCreateExamAPI(selectedClassId, {
          ...bodyExam,
          questions: questionsPayload,
        });

        if (!res || !res.success) {
          message.error(res?.message || "Tạo bài kiểm tra thất bại");
          return;
        }

        message.success("Đã tạo bài kiểm tra mới");
        setCurrent(1);
      }

      setOpenEditor(false);
      setEditing(null);
      form.resetFields();
      await fetchExams();
    } catch (err) {
      console.error("submit exam error:", err);
      message.error("Có lỗi khi lưu bài kiểm tra");
    }
  };

  const onDelete = async (examId) => {
    if (!selectedClassId) return;
    try {
      const res = await callDeleteExamAPI(selectedClassId, examId);
      if (res && res.success) {
        message.success("Đã xóa bài kiểm tra");
        await fetchExams();
      } else {
        message.error(res?.message || "Xóa bài kiểm tra thất bại");
      }
    } catch (err) {
      console.error("delete exam error:", err);
      message.error("Có lỗi khi xóa bài kiểm tra");
    }
  };

  const onPublish = async (examId) => {
    if (!selectedClassId) return;
    try {
      const res = await callPublishExamAPI(selectedClassId, examId);
      if (res && res.success) {
        message.success("Đã công bố bài kiểm tra");
        await fetchExams();
      } else {
        message.error(res?.message || "Công bố bài kiểm tra thất bại");
      }
    } catch (err) {
      console.error("publish exam error:", err);
      message.error("Có lỗi khi công bố bài kiểm tra");
    }
  };

  const formatDateTimeVN = (iso) =>
    iso ? dayjs(iso).format("DD/MM/YYYY HH:mm") : "-";

  /* ========================= RENDER ========================= */

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Quản lý bài kiểm tra
          </Title>
          <Text type="secondary">
            Tạo và quản lý bài kiểm tra trắc nghiệm cho các lớp bạn dạy
          </Text>
        </div>

        <Space>
          <Select
            loading={loadingClasses}
            value={selectedClassId ?? undefined}
            onChange={(v) => {
              setSelectedClassId(v);
              setCurrent(1);
            }}
            placeholder="Chọn lớp"
            style={{ width: 260 }}
            options={classes.map((c) => ({
              value: c.id,
              label: `${c.name} (${c.code})`,
            }))}
          />
          <Button
            type="primary"
            icon={<Plus size={16} />}
            onClick={openCreate}
            disabled={!selectedClassId}
          >
            Tạo bài kiểm tra
          </Button>
        </Space>
      </div>

      {/* List exams */}
      <div className={styles.grid}>
        {loadingExams ? (
          <div className={styles.loadingWrap}>
            <Text type="secondary">Đang tải danh sách bài kiểm tra...</Text>
          </div>
        ) : exams.length === 0 ? (
          <Empty description="Chưa có bài kiểm tra" />
        ) : (
          exams.map((t) => (
            <Card key={t.id} className={styles.card} bordered>
              <div className={styles.cardHeader}>
                <div className={styles.meta}>
                  <div className={styles.cardTitle}>{t.title}</div>
                  <Space size={6} wrap>
                    <Tag>{t.className}</Tag>
                    <Tag color={STATUS_COLORS[t.status] || "default"}>
                      {STATUS_LABELS[t.status] || t.status}
                    </Tag>
                  </Space>
                </div>

                <Space>
                  <Button
                    size="small"
                    icon={<Pencil size={16} />}
                    onClick={() => openEdit(t)}
                  >
                    Sửa
                  </Button>
                  <Button
                    size="small"
                    danger
                    icon={<Trash2 size={16} />}
                    onClick={() => onDelete(t.id)}
                  >
                    Xóa
                  </Button>
                  {!t.isPublished && (
                    <Button
                      size="small"
                      type="primary"
                      onClick={() => onPublish(t.id)}
                    >
                      Công bố
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
                  <span>{t.questionCount} câu hỏi</span>
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
      {total > 0 && (
        <div className={styles.pagination}>
          <Pagination
            current={current}
            pageSize={pageSize}
            total={total}
            showSizeChanger
            pageSizeOptions={[4, 6, 10, 20]}
            onChange={handleOnChangePagi}
            onShowSizeChange={handleOnChangePagi}
          />
        </div>
      )}

      {/* Modal create / edit */}
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
        width={800}
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            title: "",
            description: "",
            duration: 45,
            totalMarks: 0,
            timeRange: null,
            questionIds: [],
          }}
        >
          <Form.Item label="Lớp học">
            <Input
              value={classes.find((c) => c.id === selectedClassId)?.name || "—"}
              disabled
            />
          </Form.Item>

          <Form.Item
            label="Tiêu đề"
            name="title"
            rules={[{ required: true, message: "Nhập tiêu đề" }]}
          >
            <Input placeholder="VD: Kiểm tra giữa kỳ - Chương 1" />
          </Form.Item>

          <Form.Item label="Mô tả" name="description">
            <Input.TextArea rows={3} placeholder="Mô tả ngắn về bài kiểm tra" />
          </Form.Item>

          <Form.Item
            label="Thời lượng (phút)"
            name="duration"
            rules={[{ required: true, message: "Nhập thời lượng làm bài" }]}
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
            style={{ display: "block", marginTop: -8, marginBottom: 12 }}
          >
            Học sinh chỉ có thể làm bài trong khoảng thời gian này
          </Text>

          {/* CHỌN CÂU HỎI – dùng cho cả create & edit */}
          <div style={{ marginBottom: 8 }}>
            <Space wrap>
              <Text strong>Chọn câu hỏi cho bài kiểm tra</Text>
              <Select
                size="small"
                style={{ width: 220 }}
                value={bankSubjectFilter}
                onChange={setBankSubjectFilter}
                options={[
                  { value: "all", label: "Tất cả môn" },
                  ...bankSubjects.map((s) => ({ value: s, label: s })),
                ]}
                placeholder="Lọc theo môn"
              />
            </Space>
          </div>

          <Form.Item name="questionIds">
            <Select
              mode="multiple"
              placeholder="Chọn câu hỏi..."
              loading={loadingBank}
              optionFilterProp="label"
              showSearch
              maxTagCount="responsive"
              style={{ width: "100%" }}
              options={filteredBankQuestions.map((q) => ({
                value: q.id,
                label: `[${q.subject}] ${q.text}`,
              }))}
            />
          </Form.Item>

          <Text type="secondary" style={{ display: "block" }}>
            Thứ tự câu hỏi trong đề sẽ theo thứ tự bạn chọn trong danh sách
            trên. Mỗi câu mặc định 1 điểm (totalMarks = số câu hỏi).
          </Text>
        </Form>
      </Modal>
    </div>
  );
}
