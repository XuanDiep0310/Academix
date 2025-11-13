import { useMemo, useState } from "react";
import {
  Button,
  Modal,
  Drawer,
  Form,
  Input,
  Table,
  Tag,
  Badge,
  Typography,
  Space,
  Popconfirm,
  message,
  Pagination,
  Checkbox,
  Divider,
  Empty,
} from "antd";
import { Plus, Pencil, Trash2, Users, UserPlus } from "lucide-react";
import styles from "../../../assets/styles/ClassManagement.module.scss";

const { Title, Text } = Typography;

/* ============================ DATASET TRONG FILE ============================ */
// Teachers
const DATA_TEACHERS = [
  { id: "1", name: "Nguyễn Văn A", email: "teacher1@school.com" },
  { id: "2", name: "Trần Thị B", email: "teacher2@school.com" },
  { id: "3", name: "Lê Văn C", email: "teacher3@school.com" },
  { id: "4", name: "Phạm Thị D", email: "teacher4@school.com" },
  { id: "5", name: "Võ Văn E", email: "teacher5@school.com" },
  { id: "6", name: "Đỗ Thị F", email: "teacher6@school.com" },
  { id: "7", name: "Huỳnh Văn G", email: "teacher7@school.com" },
  { id: "8", name: "Bùi Thị H", email: "teacher8@school.com" },
  { id: "9", name: "Phan Văn I", email: "teacher9@school.com" },
  { id: "10", name: "Vũ Thị K", email: "teacher10@school.com" },
  { id: "11", name: "Mai Văn L", email: "teacher11@school.com" },
  { id: "12", name: "Hồ Thị M", email: "teacher12@school.com" },
  { id: "13", name: "Tạ Văn N", email: "teacher13@school.com" },
  { id: "14", name: "Đặng Thị O", email: "teacher14@school.com" },
  { id: "15", name: "Lý Văn P", email: "teacher15@school.com" },
];

// Students
const DATA_STUDENTS = [
  { id: "1", name: "HS 01", email: "student01@school.com" },
  { id: "2", name: "HS 02", email: "student02@school.com" },
  { id: "3", name: "HS 03", email: "student03@school.com" },
  { id: "4", name: "HS 04", email: "student04@school.com" },
  { id: "5", name: "HS 05", email: "student05@school.com" },
  { id: "6", name: "HS 06", email: "student06@school.com" },
  { id: "7", name: "HS 07", email: "student07@school.com" },
  { id: "8", name: "HS 08", email: "student08@school.com" },
  { id: "9", name: "HS 09", email: "student09@school.com" },
  { id: "10", name: "HS 10", email: "student10@school.com" },
  { id: "11", name: "HS 11", email: "student11@school.com" },
  { id: "12", name: "HS 12", email: "student12@school.com" },
  { id: "13", name: "HS 13", email: "student13@school.com" },
  { id: "14", name: "HS 14", email: "student14@school.com" },
  { id: "15", name: "HS 15", email: "student15@school.com" },
  { id: "16", name: "HS 16", email: "student16@school.com" },
  { id: "17", name: "HS 17", email: "student17@school.com" },
  { id: "18", name: "HS 18", email: "student18@school.com" },
  { id: "19", name: "HS 19", email: "student19@school.com" },
  { id: "20", name: "HS 20", email: "student20@school.com" },
  { id: "21", name: "HS 21", email: "student21@school.com" },
  { id: "22", name: "HS 22", email: "student22@school.com" },
  { id: "23", name: "HS 23", email: "student23@school.com" },
  { id: "24", name: "HS 24", email: "student24@school.com" },
  { id: "25", name: "HS 25", email: "student25@school.com" },
  { id: "26", name: "HS 26", email: "student26@school.com" },
  { id: "27", name: "HS 27", email: "student27@school.com" },
  { id: "28", name: "HS 28", email: "student28@school.com" },
  { id: "29", name: "HS 29", email: "student29@school.com" },
  { id: "30", name: "HS 30", email: "student30@school.com" },
  { id: "31", name: "HS 31", email: "student31@school.com" },
  { id: "32", name: "HS 32", email: "student32@school.com" },
  { id: "33", name: "HS 33", email: "student33@school.com" },
  { id: "34", name: "HS 34", email: "student34@school.com" },
  { id: "35", name: "HS 35", email: "student35@school.com" },
  { id: "36", name: "HS 36", email: "student36@school.com" },
  { id: "37", name: "HS 37", email: "student37@school.com" },
  { id: "38", name: "HS 38", email: "student38@school.com" },
  { id: "39", name: "HS 39", email: "student39@school.com" },
  { id: "40", name: "HS 40", email: "student40@school.com" },
  { id: "41", name: "HS 41", email: "student41@school.com" },
  { id: "42", name: "HS 42", email: "student42@school.com" },
  { id: "43", name: "HS 43", email: "student43@school.com" },
  { id: "44", name: "HS 44", email: "student44@school.com" },
  { id: "45", name: "HS 45", email: "student45@school.com" },
  { id: "46", name: "HS 46", email: "student46@school.com" },
  { id: "47", name: "HS 47", email: "student47@school.com" },
  { id: "48", name: "HS 48", email: "student48@school.com" },
  { id: "49", name: "HS 49", email: "student49@school.com" },
  { id: "50", name: "HS 50", email: "student50@school.com" },
];

// Classes
const DATA_CLASSES = [
  {
    id: "c1",
    name: "Lập trình C cơ bản",
    code: "CS101",
    description: "Nhập môn lập trình với C",
    teacherIds: ["1"],
    studentIds: ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10"],
    createdAt: "2024-01-10",
  },
  {
    id: "c2",
    name: "Toán cao cấp A",
    code: "MTH201",
    description: "Giải tích & đại số tuyến tính",
    teacherIds: ["2", "3"],
    studentIds: ["5", "6", "7", "8", "11", "12", "13", "14", "15"],
    createdAt: "2024-02-05",
  },
  {
    id: "c3",
    name: "Vật lý đại cương",
    code: "PHY110",
    description: "Cơ học – Nhiệt – Điện",
    teacherIds: ["4"],
    studentIds: ["16", "17", "18", "19", "20", "21", "22"],
    createdAt: "2024-03-12",
  },
  {
    id: "c4",
    name: "Hóa học đại cương",
    code: "CHE101",
    description: "Cấu tạo chất & phản ứng",
    teacherIds: ["5", "6"],
    studentIds: [
      "9",
      "10",
      "11",
      "23",
      "24",
      "25",
      "26",
      "27",
      "28",
      "29",
      "30",
    ],
    createdAt: "2024-04-02",
  },
  {
    id: "c5",
    name: "Cấu trúc dữ liệu & Giải thuật",
    code: "CS204",
    description: "Array, LinkedList, Stack/Queue, Tree, Graph…",
    teacherIds: ["7"],
    studentIds: ["31", "32", "33", "34", "35", "36", "37", "38"],
    createdAt: "2024-05-20",
  },
  {
    id: "c6",
    name: "Cơ sở dữ liệu",
    code: "DB101",
    description: "SQL căn bản, mô hình ER, chuẩn hoá",
    teacherIds: ["8", "9"],
    studentIds: [
      "12",
      "13",
      "14",
      "15",
      "39",
      "40",
      "41",
      "42",
      "43",
      "44",
      "45",
    ],
    createdAt: "2024-06-18",
  },
  {
    id: "c7",
    name: "Lập trình Web",
    code: "WEB201",
    description: "HTML/CSS/JS cơ bản",
    teacherIds: ["10"],
    studentIds: ["18", "19", "20", "21", "22", "23", "24"],
    createdAt: "2024-08-01",
  },
  {
    id: "c8",
    name: "Xác suất – Thống kê",
    code: "STA210",
    description: "Biến ngẫu nhiên, ước lượng, kiểm định",
    teacherIds: ["11", "12"],
    studentIds: [
      "25",
      "26",
      "27",
      "28",
      "29",
      "30",
      "46",
      "47",
      "48",
      "49",
      "50",
    ],
    createdAt: "2024-09-09",
  },
];
/* ========================================================================== */

const MAX_TEACHERS = 2;
const MAX_STUDENTS = 100;

export default function ClassManagement() {
  // State nội bộ (không API)
  const [classes, setClasses] = useState(DATA_CLASSES);
  const [teachers] = useState(DATA_TEACHERS);
  const [students] = useState(DATA_STUDENTS);

  // UI
  const [q, setQ] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const [form] = Form.useForm();
  const [openEditor, setOpenEditor] = useState(false);
  const [editingClass, setEditingClass] = useState(null);

  const [openMembers, setOpenMembers] = useState(false);
  const [managingClass, setManagingClass] = useState(null);

  const [openTeacherDrawer, setOpenTeacherDrawer] = useState(false);
  const [openStudentDrawer, setOpenStudentDrawer] = useState(false);
  const [selectedTeachers, setSelectedTeachers] = useState([]);
  const [selectedStudents, setSelectedStudents] = useState([]);

  const filtered = useMemo(() => {
    if (!q?.trim()) return classes;
    const key = q.toLowerCase();
    return classes.filter(
      (c) =>
        c.name.toLowerCase().includes(key) ||
        c.code.toLowerCase().includes(key) ||
        c.description?.toLowerCase().includes(key)
    );
  }, [classes, q]);

  const total = filtered.length;
  const currentData = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  const getTeacher = (id) => teachers.find((t) => String(t.id) === String(id));
  const getStudent = (id) => students.find((s) => String(s.id) === String(id));

  /* --------------------- CRUD lớp học (local) --------------------- */
  const openCreate = () => {
    setEditingClass(null);
    form.resetFields();
    setOpenEditor(true);
  };

  const openEdit = (row) => {
    setEditingClass(row);
    form.setFieldsValue({
      name: row.name,
      code: row.code,
      description: row.description,
    });
    setOpenEditor(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    try {
      if (editingClass) {
        setClasses((prev) =>
          prev.map((c) => (c.id === editingClass.id ? { ...c, ...values } : c))
        );
        message.success("Đã cập nhật lớp học");
      } else {
        const newClass = {
          id: String(Date.now()),
          ...values,
          teacherIds: [],
          studentIds: [],
          createdAt: new Date().toISOString().split("T")[0],
        };
        setClasses((prev) => [newClass, ...prev]);
        setPage(1);
        message.success("Đã tạo lớp học");
      }
      setOpenEditor(false);
      setEditingClass(null);
      form.resetFields();
    } catch {
      message.error("Lưu thất bại");
    }
  };

  const handleDelete = async (id) => {
    try {
      setClasses((prev) => prev.filter((c) => c.id !== id));
      message.success("Đã xóa lớp học");
    } catch {
      message.error("Xóa thất bại");
    }
  };

  /* --------------------- Thành viên (local) --------------------- */
  const openMembersModal = (row) => {
    setManagingClass(row);
    setOpenMembers(true);
  };

  const removeTeacher = async (teacherId) => {
    try {
      const next = (managingClass.teacherIds || []).filter(
        (x) => String(x) !== String(teacherId)
      );
      setManagingClass((prev) => ({ ...prev, teacherIds: next }));
      setClasses((prev) =>
        prev.map((c) =>
          c.id === managingClass.id ? { ...c, teacherIds: next } : c
        )
      );
      message.success("Đã xóa giáo viên khỏi lớp");
    } catch {
      message.error("Thao tác thất bại");
    }
  };

  const removeStudent = async (studentId) => {
    try {
      const next = (managingClass.studentIds || []).filter(
        (x) => String(x) !== String(studentId)
      );
      setManagingClass((prev) => ({ ...prev, studentIds: next }));
      setClasses((prev) =>
        prev.map((c) =>
          c.id === managingClass.id ? { ...c, studentIds: next } : c
        )
      );
      message.success("Đã xóa học sinh khỏi lớp");
    } catch {
      message.error("Thao tác thất bại");
    }
  };

  /* --------------------- Drawer chọn GV/HS --------------------- */
  const openTeacherPicker = (row) => {
    setManagingClass(row);
    setSelectedTeachers(row.teacherIds || []);
    setOpenTeacherDrawer(true);
  };

  const openStudentPicker = (row) => {
    setManagingClass(row);
    setSelectedStudents(row.studentIds || []);
    setOpenStudentDrawer(true);
  };

  const saveTeacherChanges = async () => {
    if (selectedTeachers.length > MAX_TEACHERS) {
      message.error(`Tối đa ${MAX_TEACHERS} giáo viên mỗi lớp`);
      return;
    }
    try {
      setClasses((prev) =>
        prev.map((c) =>
          c.id === managingClass.id ? { ...c, teacherIds: selectedTeachers } : c
        )
      );
      setManagingClass((prev) => ({ ...prev, teacherIds: selectedTeachers }));
      setOpenTeacherDrawer(false);
      message.success("Đã cập nhật giáo viên");
    } catch {
      message.error("Cập nhật thất bại");
    }
  };

  const saveStudentChanges = async () => {
    if (selectedStudents.length > MAX_STUDENTS) {
      message.error(`Tối đa ${MAX_STUDENTS} học sinh mỗi lớp`);
      return;
    }
    try {
      setClasses((prev) =>
        prev.map((c) =>
          c.id === managingClass.id ? { ...c, studentIds: selectedStudents } : c
        )
      );
      setManagingClass((prev) => ({ ...prev, studentIds: selectedStudents }));
      setOpenStudentDrawer(false);
      message.success("Đã cập nhật học sinh");
    } catch {
      message.error("Cập nhật thất bại");
    }
  };

  const toggleTeacher = (id) => {
    setSelectedTeachers((prev) => {
      const s = String(id);
      if (prev.map(String).includes(s))
        return prev.filter((x) => String(x) !== s);
      if (prev.length >= MAX_TEACHERS) {
        message.error(`Tối đa ${MAX_TEACHERS} giáo viên mỗi lớp`);
        return prev;
      }
      return [...prev, id];
    });
  };

  const toggleStudent = (id) => {
    setSelectedStudents((prev) => {
      const s = String(id);
      if (prev.map(String).includes(s))
        return prev.filter((x) => String(x) !== s);
      if (prev.length >= MAX_STUDENTS) {
        message.error(`Tối đa ${MAX_STUDENTS} học sinh mỗi lớp`);
        return prev;
      }
      return [...prev, id];
    });
  };

  /* --------------------- Columns --------------------- */
  const columns = [
    {
      title: "Tên lớp",
      dataIndex: "name",
      key: "name",
      render: (text) => <Text strong>{text}</Text>,
    },
    {
      title: "Mã lớp",
      dataIndex: "code",
      key: "code",
      render: (code) => <Tag>{code}</Tag>,
      width: 120,
    },
    {
      title: "Giáo viên",
      key: "teachers",
      render: (_, row) => (
        <Space>
          <Badge count={`${(row.teacherIds || []).length}/${MAX_TEACHERS}`} />
          <Button
            type="text"
            size="small"
            onClick={() => openTeacherPicker(row)}
            icon={<UserPlus size={16} />}
          >
            Thêm
          </Button>
        </Space>
      ),
      width: 160,
    },
    {
      title: "Học sinh",
      key: "students",
      render: (_, row) => (
        <Space>
          <Badge count={`${(row.studentIds || []).length}/${MAX_STUDENTS}`} />
          <Button
            type="text"
            size="small"
            onClick={() => openStudentPicker(row)}
            icon={<UserPlus size={16} />}
          >
            Thêm
          </Button>
        </Space>
      ),
      width: 170,
    },
    { title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 140 },
    {
      title: "Thao tác",
      key: "actions",
      align: "right",
      render: (_, row) => (
        <Space>
          <Button
            size="small"
            type="default"
            icon={<Users size={16} />}
            onClick={() => openMembersModal(row)}
          />
          <Button
            size="small"
            type="primary"
            ghost
            icon={<Pencil size={16} />}
            onClick={() => openEdit(row)}
          />
          <Popconfirm
            title="Xóa lớp học?"
            okText="Xóa"
            cancelText="Hủy"
            onConfirm={() => handleDelete(row.id)}
          >
            <Button size="small" danger icon={<Trash2 size={16} />} />
          </Popconfirm>
        </Space>
      ),
      width: 160,
    },
  ];

  return (
    <div className={styles.wrapper}>
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Quản lý lớp học
          </Title>
          <Text type="secondary">UI + dataset nội bộ (không API)</Text>
        </div>

        <Space>
          <Input
            allowClear
            placeholder="Tìm kiếm tên/mã lớp..."
            value={q}
            onChange={(e) => {
              setQ(e.target.value);
              setPage(1);
            }}
            style={{ width: 260 }}
          />
          <Button
            type="primary"
            icon={<Plus size={16} />}
            onClick={openCreate}
            className={styles.createBtn}
          >
            Tạo lớp học
          </Button>
        </Space>
      </div>

      <div className={styles.tableCard}>
        <Table
          rowKey="id"
          dataSource={currentData}
          columns={columns}
          pagination={false}
          locale={{ emptyText: <Empty description="Chưa có dữ liệu" /> }}
        />

        <div className={styles.pagination}>
          <Pagination
            current={page}
            pageSize={pageSize}
            total={total}
            showSizeChanger={false}
            onChange={(p) => setPage(p)}
          />
        </div>
      </div>

      {/* Modal: tạo/sửa */}
      <Modal
        title={editingClass ? "Chỉnh sửa lớp học" : "Tạo lớp học mới"}
        open={openEditor}
        onCancel={() => setOpenEditor(false)}
        onOk={handleSubmit}
        okText={editingClass ? "Cập nhật" : "Tạo lớp"}
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{ name: "", code: "", description: "" }}
        >
          <Form.Item
            label="Tên lớp học"
            name="name"
            rules={[{ required: true, message: "Vui lòng nhập tên lớp" }]}
          >
            <Input placeholder="VD: Lập trình 1" />
          </Form.Item>
          <Form.Item
            label="Mã lớp"
            name="code"
            rules={[{ required: true, message: "Vui lòng nhập mã lớp" }]}
          >
            <Input placeholder="VD: CS101" />
          </Form.Item>
          <Form.Item label="Mô tả" name="description">
            <Input.TextArea rows={3} placeholder="Mô tả ngắn..." />
          </Form.Item>
        </Form>
      </Modal>

      {/* Modal: Thành viên */}
      <Modal
        title={
          <Space direction="vertical" size={0}>
            <Text strong>Thành viên lớp</Text>
            <Text type="secondary">{managingClass?.name}</Text>
          </Space>
        }
        open={openMembers}
        onCancel={() => {
          setOpenMembers(false);
          setManagingClass(null);
        }}
        footer={null}
        width={900}
        destroyOnClose
      >
        <div className={styles.membersWrap}>
          {/* Giáo viên */}
          <div className={styles.memberBlock}>
            <div className={styles.memberHeader}>
              <Title level={5} style={{ margin: 0 }}>
                Giáo viên ({(managingClass?.teacherIds || []).length}/
                {MAX_TEACHERS})
              </Title>
              <Button
                size="middle"
                icon={<UserPlus size={16} />}
                onClick={() => openTeacherPicker(managingClass)}
              >
                Thêm giáo viên
              </Button>
            </div>
            <Divider style={{ margin: "12px 0" }} />
            <Table
              size="small"
              rowKey={(r) => r}
              dataSource={managingClass?.teacherIds || []}
              pagination={false}
              columns={[
                {
                  title: "Họ tên",
                  render: (id) => <span>{getTeacher(id)?.name}</span>,
                },
                {
                  title: "Email",
                  render: (id) => <span>{getTeacher(id)?.email}</span>,
                },
                {
                  title: "Thao tác",
                  align: "right",
                  render: (id) => (
                    <Popconfirm
                      title="Xóa giáo viên khỏi lớp?"
                      okText="Xóa"
                      cancelText="Hủy"
                      onConfirm={() => removeTeacher(id)}
                    >
                      <Button type="text" danger>
                        Xóa
                      </Button>
                    </Popconfirm>
                  ),
                },
              ]}
              locale={{ emptyText: "Chưa có giáo viên" }}
            />
          </div>

          {/* Học sinh */}
          <div className={styles.memberBlock}>
            <div className={styles.memberHeader}>
              <Title level={5} style={{ margin: 0 }}>
                Học sinh ({(managingClass?.studentIds || []).length}/
                {MAX_STUDENTS})
              </Title>
              <Button
                size="middle"
                icon={<UserPlus size={16} />}
                onClick={() => openStudentPicker(managingClass)}
              >
                Thêm học sinh
              </Button>
            </div>
            <Divider style={{ margin: "12px 0" }} />
            <div className={styles.studentTable}>
              <Table
                size="small"
                rowKey={(r) => r}
                dataSource={managingClass?.studentIds || []}
                pagination={{ pageSize: 8 }}
                columns={[
                  {
                    title: "Họ tên",
                    render: (id) => <span>{getStudent(id)?.name}</span>,
                  },
                  {
                    title: "Email",
                    render: (id) => <span>{getStudent(id)?.email}</span>,
                  },
                  {
                    title: "Thao tác",
                    align: "right",
                    render: (id) => (
                      <Popconfirm
                        title="Xóa học sinh khỏi lớp?"
                        okText="Xóa"
                        cancelText="Hủy"
                        onConfirm={() => removeStudent(id)}
                      >
                        <Button type="text" danger>
                          Xóa
                        </Button>
                      </Popconfirm>
                    ),
                  },
                ]}
                locale={{ emptyText: "Chưa có học sinh" }}
              />
            </div>
          </div>
        </div>
      </Modal>

      {/* Drawer: Giáo viên */}
      <Drawer
        title="Thêm giáo viên"
        open={openTeacherDrawer}
        onClose={() => setOpenTeacherDrawer(false)}
        extra={
          <Space>
            <Text type="secondary">
              Đã chọn: {selectedTeachers.length}/{MAX_TEACHERS}
            </Text>
            <Button type="primary" onClick={saveTeacherChanges}>
              Lưu thay đổi
            </Button>
          </Space>
        }
        width={420}
      >
        <div className={styles.pickList}>
          {teachers.map((t) => (
            <label key={t.id} className={styles.pickRow}>
              <Checkbox
                checked={selectedTeachers.map(String).includes(String(t.id))}
                onChange={() => toggleTeacher(t.id)}
              />
              <div className={styles.pickMeta}>
                <span>{t.name}</span>
                <small>{t.email}</small>
              </div>
            </label>
          ))}
        </div>
      </Drawer>

      {/* Drawer: Học sinh */}
      <Drawer
        title="Thêm học sinh"
        open={openStudentDrawer}
        onClose={() => setOpenStudentDrawer(false)}
        extra={
          <Space>
            <Text type="secondary">
              Đã chọn: {selectedStudents.length}/{MAX_STUDENTS}
            </Text>
            <Button type="primary" onClick={saveStudentChanges}>
              Lưu thay đổi
            </Button>
          </Space>
        }
        width={420}
      >
        <div className={styles.pickList}>
          {students.map((s) => (
            <label key={s.id} className={styles.pickRow}>
              <Checkbox
                checked={selectedStudents.map(String).includes(String(s.id))}
                onChange={() => toggleStudent(s.id)}
              />
              <div className={styles.pickMeta}>
                <span>{s.name}</span>
                <small>{s.email}</small>
              </div>
            </label>
          ))}
        </div>
      </Drawer>
    </div>
  );
}
