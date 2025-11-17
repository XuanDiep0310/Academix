import { useEffect, useState } from "react";
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
  Divider,
  Upload,
  Table,
  Checkbox,
} from "antd";
import {
  Plus,
  Pencil,
  Trash2,
  Filter,
  Upload as UploadIcon,
} from "lucide-react";
import styles from "../../../assets/styles/QuestionBank.module.scss";
import {
  callListQuestionBankAPI,
  callCreateQuestionAPI,
  deleteQuestionAPI,
  editQuestionAPI,
  callBulkCreateQuestionAPI,
} from "../../../services/api.service";
import * as XLSX from "xlsx";
import templateDataQues from "./templateDataQues.xlsx?url";
const { Title, Text } = Typography;

const DIFFICULTY_LABELS = { easy: "Dễ", medium: "Trung bình", hard: "Khó" };
const DIFFICULTY_COLORS = {
  easy: "default",
  medium: "processing",
  hard: "error",
};
const mapDifficultyFromExcelToApi = (raw) => {
  if (!raw) return "Medium";
  const s = String(raw).toLowerCase().trim();
  if (s === "easy" || s === "dễ") return "Easy";
  if (s === "hard" || s === "khó") return "Hard";
  return "Medium";
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
// q: cùng kiểu với 1 phần tử trong state 'questions'
const buildApiQuestionFromForm = (values) => {
  const qType = values.questionType;

  let raw = [values.option1, values.option2, values.option3, values.option4];
  if (qType === "TrueFalse") raw = raw.slice(0, 2);

  let options;

  if (qType === "MultipleChoice") {
    // nhiều đáp án đúng
    const correctArr = values.correctAnswers || [];
    const correctSet = new Set(correctArr);

    options = raw.map((text, idx) => ({
      optionText: text,
      isCorrect: correctSet.has(idx),
      optionOrder: idx + 1,
    }));
  } else {
    // SingleChoice / TrueFalse → 1 đáp án đúng
    options = raw.map((text, idx) => ({
      optionText: text,
      isCorrect: idx === values.correctAnswer,
      optionOrder: idx + 1,
    }));
  }

  return {
    questionText: values.question,
    questionType: qType,
    difficultyLevel: mapDifficultyToApi(values.difficulty),
    subject: values.subject,
    options,
  };
};

export default function QuestionBank() {
  const [questions, setQuestions] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [loading, setLoading] = useState(false);

  const [filterSubject, setFilterSubject] = useState("all");
  const [filterDifficulty, setFilterDifficulty] = useState("all");
  const [filterType, setFilterType] = useState("all");
  const [searchKeyword, setSearchKeyword] = useState("");

  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(8);
  const [total, setTotal] = useState(0);

  const [openEditor, setOpenEditor] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form] = Form.useForm();
  const questionType = Form.useWatch("questionType", form);

  // để nhập môn/chủ đề mới trong Select
  const [newSubject, setNewSubject] = useState("");

  const [bulkQuestions, setBulkQuestions] = useState([]);
  const [openImport, setOpenImport] = useState(false);
  const [importPreview, setImportPreview] = useState([]); // dữ liệu hiển thị bảng
  const [importApiQuestions, setImportApiQuestions] = useState([]); // body gửi API
  const [importLoading, setImportLoading] = useState(false);

  // ======================= FETCH API =======================
  const fetchAllSubjects = async () => {
    try {
      // gọi 1 lần, pageSize to lên cho chắc (tùy backend của bạn)
      const qs = new URLSearchParams();
      qs.set("page", "1");
      qs.set("pageSize", "10000");
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");

      const res = await callListQuestionBankAPI(qs.toString());

      if (res?.success && res.data?.questions) {
        const allQs = res.data.questions;
        const unique = Array.from(
          new Set(
            allQs
              .map((q) => q.subject)
              .filter((s) => !!s && String(s).trim() !== "")
          )
        ).map((s) => String(s).trim());

        setSubjects(unique);
      }
    } catch (err) {
      console.error("fetchAllSubjects error:", err);
    }
  };
  useEffect(() => {
    fetchAllSubjects();
  }, []);
  const fetchQuestions = async () => {
    try {
      setLoading(true);

      const qs = new URLSearchParams();
      qs.set("page", String(current));
      qs.set("pageSize", String(pageSize));
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");

      if (filterSubject && filterSubject !== "all") {
        qs.set("subject", filterSubject);
      }
      if (filterDifficulty !== "all")
        qs.set("difficulty", mapDifficultyToApi(filterDifficulty));
      if (filterType !== "all") qs.set("type", filterType);
      if (searchKeyword.trim()) qs.set("search", searchKeyword.trim());

      const res = await callListQuestionBankAPI(qs.toString());

      if (res?.success && res.data) {
        const api = res.data;

        const mapped =
          api.questions?.map((q) => {
            const opts =
              q.options || q.questionOptions || q.answerOptions || [];

            // lấy hết index đáp án đúng
            const correctIndexes = opts.reduce((acc, o, idx) => {
              if (o.isCorrect) acc.push(idx);
              return acc;
            }, []);

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
              correctAnswers: correctIndexes,
              subject: q.subject || "Chưa gán môn",
              difficulty,
              questionType: q.questionType || "SingleChoice",
              createdAt: q.createdAt,
            };
          }) || [];

        setQuestions(mapped);
        setTotal(api.totalCount ?? mapped.length);
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
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [
    current,
    pageSize,
    filterSubject,
    filterDifficulty,
    filterType,
    searchKeyword,
  ]);

  // ======================= FILTER + PAGINATION =======================

  const paged = questions;

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
      subject: undefined,
      difficulty: "medium",
      questionType: "SingleChoice",
      question: "",
      option1: "",
      option2: "",
      option3: "",
      option4: "",
      correctAnswer: 0,
      correctAnswers: [],
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

      correctAnswer:
        row.questionType === "MultipleChoice"
          ? row.correctAnswers?.[0] ?? 0
          : row.correctAnswers?.[0] ?? 0,
      correctAnswers: row.correctAnswers || [],
    });

    setOpenEditor(true);
  };

  const onSubmit = async () => {
    const values = await form.validateFields();
    const apiBody = buildApiQuestionFromForm(values);

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
        setCurrent(1); // trả về trang đầu
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
  const onAddToBulkList = async () => {
    const values = await form.validateFields();
    const apiQuestion = buildApiQuestionFromForm(values);

    setBulkQuestions((prev) => [...prev, apiQuestion]);
    message.success("Đã thêm câu hỏi vào danh sách gửi bulk");

    // reset form để nhập câu tiếp theo
    setEditing(null);
    form.setFieldsValue({
      subject: values.subject, // giữ môn học cho tiện
      difficulty: values.difficulty,
      questionType: values.questionType,
      question: "",
      option1: "",
      option2: "",
      option3: "",
      option4: "",
      correctAnswer: 0,
    });
  };
  const onSubmitBulk = async () => {
    if (!bulkQuestions.length) {
      message.warning("Chưa có câu hỏi nào trong danh sách bulk");
      return;
    }

    try {
      const res = await callBulkCreateQuestionAPI(bulkQuestions);
      console.log("Submitting bulk questions:", bulkQuestions);
      console.log("Bulk create questions response:", res);
      if (res?.success) {
        message.success(`Đã thêm ${bulkQuestions.length} câu hỏi`);
        setBulkQuestions([]);
        setCurrent(1);
        await fetchQuestions();
        setOpenEditor(false);
        form.resetFields();
      } else {
        message.error("Thêm nhiều câu hỏi thất bại");
      }
    } catch (err) {
      console.error("Bulk create questions error:", err);
      message.error("Có lỗi xảy ra khi thêm nhiều câu hỏi");
    }
  };
  // đọc file Excel user upload
  const handleExcelFile = async (file) => {
    try {
      const data = await file.arrayBuffer();
      const workbook = XLSX.read(data, { type: "array" });
      const sheetName = workbook.SheetNames[0];
      const sheet = workbook.Sheets[sheetName];
      const rows = XLSX.utils.sheet_to_json(sheet, { header: 1 });

      if (!rows || rows.length <= 1) {
        message.error("File không có dữ liệu");
        setImportPreview([]);
        setImportApiQuestions([]);
        return Upload.LIST_IGNORE;
      }

      const [, ...body] = rows;

      const preview = [];
      const apiQuestions = [];

      body.forEach((r, idx) => {
        if (!r) return;

        const [
          questionText,
          optA,
          optB,
          optC,
          optD,
          correctRaw,
          difficultyRaw,
          subject,
          questionTypeRaw,
        ] = r;

        if (!questionText) return;

        const optionArray = [optA, optB, optC, optD];
        const optionsTexts = optionArray.filter(
          (x) => x !== undefined && x !== null && String(x).trim() !== ""
        );

        if (optionsTexts.length < 2) return;

        // ----- phân tích đáp án đúng -> danh sách index -----
        let correctIndexes = [];

        if (correctRaw !== undefined && correctRaw !== null) {
          if (typeof correctRaw === "number") {
            correctIndexes = [correctRaw - 1];
          } else {
            const parts = String(correctRaw)
              .split(/[,\;/\s]+/)
              .map((p) => p.trim())
              .filter(Boolean);

            parts.forEach((p) => {
              if (/^[A-Za-z]$/.test(p)) {
                const idxLetter = p.toUpperCase().charCodeAt(0) - 65; // A->0
                correctIndexes.push(idxLetter);
              } else {
                const n = parseInt(p, 10);
                if (!Number.isNaN(n)) correctIndexes.push(n - 1);
              }
            });
          }
        }

        // lọc index hợp lệ + bỏ trùng
        correctIndexes = Array.from(
          new Set(
            correctIndexes.filter((i) => i >= 0 && i < optionsTexts.length)
          )
        );

        if (!correctIndexes.length) correctIndexes = [0];

        const difficultyLevel = mapDifficultyFromExcelToApi(difficultyRaw);

        // ----- xác định questionType -----
        let questionType;

        if (questionTypeRaw && String(questionTypeRaw).trim()) {
          const qRaw = String(questionTypeRaw).trim().toLowerCase();
          if (["tf", "truefalse", "true/false", "đúng/sai"].includes(qRaw)) {
            questionType = "TrueFalse";
          } else if (["single", "singlechoice", "1", "one"].includes(qRaw)) {
            questionType = "SingleChoice";
          } else if (
            ["multi", "multiplechoice", "nhiều", "multichoice"].includes(qRaw)
          ) {
            questionType = "MultipleChoice";
          } else {
            questionType =
              correctIndexes.length > 1 ? "MultipleChoice" : "SingleChoice";
          }
        } else {
          // không ghi type -> tự đoán theo số lượng đáp án đúng
          questionType =
            correctIndexes.length > 1 ? "MultipleChoice" : "SingleChoice";
        }

        // dữ liệu để hiển thị preview
        preview.push({
          key: idx,
          question: questionText,
          optionA: optA,
          optionB: optB,
          optionC: optC,
          optionD: optD,
          correct:
            correctRaw !== undefined && correctRaw !== null
              ? String(correctRaw)
              : correctIndexes
                  .map((i) => String.fromCharCode(65 + i))
                  .join(","),
          difficulty: difficultyLevel,
          subject,
          questionType,
        });

        // body gửi API
        apiQuestions.push({
          questionText: String(questionText).trim(),
          questionType,
          difficultyLevel,
          subject: subject ? String(subject).trim() : "Chung",
          options: optionsTexts.map((text, i) => ({
            optionText: String(text).trim(),
            isCorrect: correctIndexes.includes(i),
            optionOrder: i + 1,
          })),
        });
      });

      if (!apiQuestions.length) {
        message.error("Không đọc được câu hỏi nào từ file");
      }

      setImportPreview(preview);
      setImportApiQuestions(apiQuestions);
    } catch (err) {
      console.error("Import Excel error:", err);
      message.error("Có lỗi khi đọc file Excel");
      setImportPreview([]);
      setImportApiQuestions([]);
    }

    return Upload.LIST_IGNORE;
  };

  // gọi API bulk
  const handleConfirmImport = async () => {
    if (!importApiQuestions.length) {
      message.warning("Chưa có câu hỏi nào để import");
      return;
    }

    try {
      setImportLoading(true);
      const res = await callBulkCreateQuestionAPI(importApiQuestions);
      if (res?.success) {
        message.success(`Đã import ${importApiQuestions.length} câu hỏi`);
        setOpenImport(false);
        setImportPreview([]);
        setImportApiQuestions([]);
        setCurrent(1);
        await fetchQuestions();
      } else {
        message.error("Import câu hỏi thất bại");
      }
    } catch (err) {
      console.error("Confirm import error:", err);
      message.error("Có lỗi khi import câu hỏi");
    } finally {
      setImportLoading(false);
    }
  };

  const optionCount = questionType === "TrueFalse" ? 2 : 4;
  const tfLabels = ["Đúng", "Sai"];

  // thêm/nhập môn hoặc chủ đề mới
  const addNewSubject = () => {
    const value = newSubject.trim();
    if (!value) return;

    if (!subjects.includes(value)) {
      setSubjects((prev) => [...prev, value]);
    }

    form.setFieldsValue({ subject: value });
    setNewSubject("");
  };
  const importColumns = [
    { title: "Câu hỏi", dataIndex: "question", key: "question", width: 260 },
    { title: "A", dataIndex: "optionA", key: "optionA" },
    { title: "B", dataIndex: "optionB", key: "optionB" },
    { title: "C", dataIndex: "optionC", key: "optionC" },
    { title: "D", dataIndex: "optionD", key: "optionD" },
    { title: "Đáp án", dataIndex: "correct", key: "correct", width: 80 },
    { title: "Độ khó", dataIndex: "difficulty", key: "difficulty", width: 100 },
    { title: "Môn", dataIndex: "subject", key: "subject", width: 140 },
    {
      title: "Loại",
      dataIndex: "questionType",
      key: "questionType",
      width: 140,
    },
  ];
  // ======================= RENDER =======================

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <div className={styles.headerLeft}>
          <Title level={4} className={styles.title}>
            Ngân hàng câu hỏi
          </Title>
          <Text type="secondary">Quản lý câu hỏi trắc nghiệm</Text>
        </div>

        <div className={styles.headerRight}>
          <Space size={8} wrap>
            {/* MÔN HỌC */}
            <Select
              allowClear
              showSearch
              placeholder="Tất cả môn học"
              value={filterSubject === "all" ? undefined : filterSubject}
              style={{ minWidth: 220 }}
              suffixIcon={<Filter size={16} />}
              onChange={(value) => {
                setFilterSubject(value || "all");
                setCurrent(1);
              }}
              options={[
                { value: "all", label: "Tất cả môn học" },
                ...subjects.map((s) => ({ value: s, label: s })),
              ]}
            />

            {/* ĐỘ KHÓ */}
            <Select
              value={filterDifficulty}
              style={{ width: 140 }}
              onChange={(v) => {
                setFilterDifficulty(v);
                setCurrent(1);
              }}
              placeholder="Độ khó"
              options={[
                { value: "all", label: "Tất cả độ khó" },
                { value: "easy", label: "Dễ" },
                { value: "medium", label: "Trung bình" },
                { value: "hard", label: "Khó" },
              ]}
            />

            {/* LOẠI CÂU HỎI */}
            <Select
              value={filterType}
              style={{ width: 140 }}
              onChange={(v) => {
                setFilterType(v);
                setCurrent(1);
              }}
              placeholder="Loại câu hỏi"
              options={[
                { value: "all", label: "Tất cả loại" },
                { value: "SingleChoice", label: "Một lựa chọn" },
                { value: "MultipleChoice", label: "Nhiều lựa chọn" },
                { value: "TrueFalse", label: "Đúng / Sai" },
              ]}
            />

            {/* SEARCH */}
            <Input.Search
              allowClear
              placeholder="Tìm nội dung câu hỏi..."
              style={{ width: 260 }}
              value={searchKeyword}
              onChange={(e) => setSearchKeyword(e.target.value)}
              onSearch={() => setCurrent(1)}
            />

            {/* IMPORT + THÊM CÂU HỎI */}
            <Button
              icon={<UploadIcon size={16} />}
              onClick={() => setOpenImport(true)}
            >
              Import từ Excel
            </Button>

            <Button
              type="primary"
              icon={<Plus size={16} />}
              onClick={openCreate}
            >
              Thêm câu hỏi
            </Button>
          </Space>
        </div>
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
                  const isCorrect = q.correctAnswers?.includes(idx);
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
        footer={[
          <Button
            key="cancel"
            onClick={() => {
              setOpenEditor(false);
              setEditing(null);
              form.resetFields();
            }}
          >
            Hủy
          </Button>,

          !editing && (
            <Button key="add-bulk" onClick={onAddToBulkList}>
              Thêm vào danh sách
            </Button>
          ),

          !editing && (
            <Button
              key="submit-bulk"
              type="primary"
              disabled={!bulkQuestions.length}
              onClick={onSubmitBulk}
            >
              Gửi danh sách ({bulkQuestions.length}){" "}
            </Button>
          ),

          <Button key="save-one" type="primary" onClick={onSubmit}>
            {editing ? "Lưu thay đổi" : "Lưu 1 câu"}
          </Button>,
        ]}
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            subject: undefined,
            difficulty: "medium",
            questionType: "SingleChoice",
            correctAnswer: 0,
            correctAnswers: [],
          }}
        >
          {/* MÔN / CHỦ ĐỀ: chọn từ list hoặc nhập mới */}
          <Form.Item
            name="subject"
            label="Môn học / Chủ đề"
            rules={[{ required: true, message: "Nhập môn học hoặc chủ đề" }]}
          >
            <Select
              showSearch
              placeholder="Chọn hoặc nhập môn/chủ đề"
              options={subjects.map((s) => ({ value: s, label: s }))}
              dropdownRender={(menu) => (
                <>
                  {menu}
                  <Divider style={{ margin: "8px 0" }} />
                  <Space style={{ padding: "0 8px 4px" }}>
                    <Input
                      placeholder="Thêm môn/chủ đề mới"
                      value={newSubject}
                      onChange={(e) => setNewSubject(e.target.value)}
                      onPressEnter={addNewSubject}
                    />
                    <Button
                      type="text"
                      icon={<Plus size={14} />}
                      onClick={addNewSubject}
                    >
                      Thêm
                    </Button>
                  </Space>
                </>
              )}
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
                { value: "SingleChoice", label: "Một lựa chọn" },
                { value: "MultipleChoice", label: "Nhiều lựa chọn" },
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

          {/* ANSWER */}
          <div className={styles.answerBlock}>
            <div className={styles.answerHeader}>
              <Text strong>Đáp án</Text>
              <Text type="secondary" style={{ fontSize: 12 }}>
                {questionType === "TrueFalse"
                  ? "Chọn đúng / sai"
                  : questionType === "MultipleChoice"
                  ? "Chọn một hoặc nhiều đáp án đúng"
                  : "Chọn 1 đáp án đúng"}
              </Text>
            </div>

            {/* Nếu là SingleChoice / TrueFalse → dùng Radio (1 đáp án) */}
            {questionType !== "MultipleChoice" && (
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
            )}

            {/* Nếu là MultipleChoice → dùng Checkbox (nhiều đáp án) */}
            {questionType === "MultipleChoice" && (
              <Form.Item
                name="correctAnswers"
                rules={[
                  { required: true, message: "Chọn ít nhất 1 đáp án đúng" },
                ]}
                noStyle
              >
                <Checkbox.Group className={styles.radioRow}>
                  {Array.from({ length: optionCount }, (_, i) => i + 1).map(
                    (n) => (
                      <div key={n} className={styles.answerRow}>
                        <Checkbox value={n - 1} />
                        <Form.Item
                          name={`option${n}`}
                          rules={[{ required: true, message: "Nhập đáp án" }]}
                          className={styles.answerInput}
                        >
                          <Input placeholder={`Đáp án ${n}`} />
                        </Form.Item>
                      </div>
                    )
                  )}
                </Checkbox.Group>
              </Form.Item>
            )}
          </div>

          {!editing && bulkQuestions.length > 0 && (
            <>
              <Divider />
              <Text strong>
                Danh sách sẽ gửi bulk ({bulkQuestions.length} câu):
              </Text>
              <ul style={{ marginTop: 8, paddingLeft: 20 }}>
                {bulkQuestions.map((q, idx) => (
                  <li key={idx}>
                    {idx + 1}. {q.questionText}
                  </li>
                ))}
              </ul>
            </>
          )}
        </Form>
      </Modal>

      {/* MODAL IMPORT EXCEL */}
      <Modal
        title="Import câu hỏi từ Excel"
        open={openImport}
        onCancel={() => {
          setOpenImport(false);
          setImportPreview([]);
          setImportApiQuestions([]);
        }}
        footer={[
          <Button
            key="cancel"
            onClick={() => {
              setOpenImport(false);
              setImportPreview([]);
              setImportApiQuestions([]);
            }}
          >
            Hủy
          </Button>,
          <Button
            key="import"
            type="primary"
            loading={importLoading}
            onClick={handleConfirmImport}
            disabled={!importApiQuestions.length}
          >
            Import dữ liệu
          </Button>,
        ]}
        width={900}
        destroyOnClose
      >
        {/* Khu kéo/thả file */}
        <Upload.Dragger
          accept=".xlsx,.xls,.csv"
          multiple={false}
          showUploadList={false}
          beforeUpload={handleExcelFile}
          style={{ padding: 16 }}
        >
          <p className="ant-upload-drag-icon">
            <UploadIcon size={32} />
          </p>
          <p className="ant-upload-text">
            Nhấp hoặc kéo tệp vào khu vực này để tải lên
          </p>
          <p className="ant-upload-hint">
            Hỗ trợ: .csv, .xls, .xlsx&nbsp;
            <a
              onClick={(e) => e.stopPropagation()}
              href={templateDataQues}
              download
            >
              Tải xuống file mẫu
            </a>
          </p>
        </Upload.Dragger>

        <Divider />

        <Text strong>Dữ liệu đọc từ Excel</Text>

        <div style={{ marginTop: 12 }}>
          <Table
            size="small"
            rowKey="key"
            columns={importColumns}
            dataSource={importPreview}
            locale={{ emptyText: "No data" }}
            pagination={{
              pageSize: 5,
              showSizeChanger: false,
            }}
          />
        </div>
      </Modal>
    </div>
  );
}
