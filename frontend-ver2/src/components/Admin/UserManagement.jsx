import { useMemo, useState } from "react";
import {
  Button,
  Modal,
  Form,
  Input,
  Table,
  Tag,
  Badge,
  Typography,
  Space,
  Popconfirm,
  Pagination,
  Divider,
  Empty,
  message,
} from "antd";
import { Plus, Pencil, Trash2, Lock, Unlock, Users } from "lucide-react";
import styles from "../../assets/styles/UserManagement.module.scss";

const { Title, Text } = Typography;

/* ============================ DATASET TRONG FILE ============================ */
function generateUsers() {
  const arr = [];
  // 15 giáo viên
  for (let i = 1; i <= 15; i++) {
    arr.push({
      id: `teacher-${i}`,
      email: `teacher${i}@school.com`,
      name: `Giáo viên ${i}`,
      role: "teacher",
      status: i % 7 === 0 ? "locked" : "active",
      createdAt: `2024-0${Math.min((i % 3) + 1, 9)}-${String(i).padStart(
        2,
        "0"
      )}`,
    });
  }
  // 85 học sinh
  for (let i = 1; i <= 85; i++) {
    arr.push({
      id: `student-${i}`,
      email: `student${i}@school.com`,
      name: `Học sinh ${i}`,
      role: "student",
      status: i % 10 === 0 ? "locked" : "active",
      createdAt: `2024-0${Math.min((i % 3) + 1, 9)}-${String(i).padStart(
        2,
        "0"
      )}`,
    });
  }
  return arr;
}
/* ========================================================================== */

export default function UserManagement() {
  // State dữ liệu cục bộ
  const [users, setUsers] = useState(() => generateUsers());

  // UI state
  const [q, setQ] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 10;

  // Modal/Forms
  const [openTeacherModal, setOpenTeacherModal] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [teacherForm] = Form.useForm();

  const [openStudentBulk, setOpenStudentBulk] = useState(false);
  const [bulkText, setBulkText] = useState("");

  // Tìm kiếm client-side
  const filtered = useMemo(() => {
    if (!q.trim()) return users;
    const key = q.toLowerCase();
    return users.filter(
      (u) =>
        u.name.toLowerCase().includes(key) ||
        u.email.toLowerCase().includes(key) ||
        u.role.toLowerCase().includes(key) ||
        u.status.toLowerCase().includes(key)
    );
  }, [users, q]);

  const total = filtered.length;
  const dataSource = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  /* --------------------- Giáo viên: tạo/sửa --------------------- */
  const openCreateTeacher = () => {
    setEditingUser(null);
    teacherForm.resetFields();
    setOpenTeacherModal(true);
  };

  const openEditTeacher = (row) => {
    if (row.role !== "teacher") return;
    setEditingUser(row);
    teacherForm.setFieldsValue({
      email: row.email,
      name: row.name,
      password: "",
    });
    setOpenTeacherModal(true);
  };

  const submitTeacher = async () => {
    const values = await teacherForm.validateFields();
    if (editingUser) {
      // cập nhật local
      setUsers((prev) =>
        prev.map((u) =>
          u.id === editingUser.id
            ? { ...u, email: values.email, name: values.name }
            : u
        )
      );
      message.success("Đã cập nhật giáo viên");
    } else {
      const newUser = {
        id: `teacher-${Date.now()}`,
        email: values.email,
        name: values.name,
        role: "teacher",
        status: "active",
        createdAt: new Date().toISOString().split("T")[0],
      };
      setUsers((prev) => [newUser, ...prev]);
      setPage(1);
      message.success("Đã tạo tài khoản giáo viên");
    }
    setOpenTeacherModal(false);
    setEditingUser(null);
    teacherForm.resetFields();
  };

  /* --------------------- Học sinh: thêm hàng loạt --------------------- */
  const submitStudentBulk = async () => {
    const lines = bulkText
      .split("\n")
      .map((l) => l.trim())
      .filter(Boolean);

    const payload = [];
    lines.forEach((line, idx) => {
      const [email, name, password] = line.split(",").map((p) => p?.trim());
      if (email && name && password) {
        payload.push({
          id: `student-${Date.now()}-${idx}`,
          email,
          name,
          role: "student",
          status: "active",
          createdAt: new Date().toISOString().split("T")[0],
        });
      }
    });

    if (!payload.length) {
      message.error("Không có dòng hợp lệ (định dạng: email,họ tên,mật khẩu)");
      return;
    }

    setUsers((prev) => [...payload, ...prev]);
    setPage(1);
    setOpenStudentBulk(false);
    setBulkText("");
    message.success(`Đã thêm ${payload.length} học sinh`);
  };

  /* --------------------- Khóa/Mở khóa & Xóa --------------------- */
  const toggleStatus = (id) => {
    setUsers((prev) =>
      prev.map((u) =>
        u.id === id
          ? { ...u, status: u.status === "active" ? "locked" : "active" }
          : u
      )
    );
  };

  const deleteUser = (id) => {
    setUsers((prev) => prev.filter((u) => u.id !== id));
    message.success("Đã xóa tài khoản");
  };

  /* --------------------- Cột bảng --------------------- */
  const columns = [
    { title: "Họ tên", dataIndex: "name", key: "name" },
    { title: "Email", dataIndex: "email", key: "email" },
    {
      title: "Vai trò",
      dataIndex: "role",
      key: "role",
      render: (role) =>
        role === "teacher" ? (
          <Tag color="blue">Giáo viên</Tag>
        ) : (
          <Tag>Học sinh</Tag>
        ),
      width: 130,
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      render: (st) =>
        st === "active" ? (
          <Badge status="success" text="Hoạt động" />
        ) : (
          <Badge status="error" text="Đã khóa" />
        ),
      width: 140,
    },
    { title: "Ngày tạo", dataIndex: "createdAt", key: "createdAt", width: 140 },
    {
      title: "Thao tác",
      key: "actions",
      align: "right",
      render: (_, row) => (
        <Space>
          {row.role === "teacher" && (
            <Button
              size="small"
              type="primary"
              ghost
              icon={<Pencil size={16} />}
              onClick={() => openEditTeacher(row)}
            >
              Sửa
            </Button>
          )}

          <Button
            size="small"
            onClick={() => toggleStatus(row.id)}
            icon={
              row.status === "active" ? (
                <Lock size={16} />
              ) : (
                <Unlock size={16} />
              )
            }
          >
            {row.status === "active" ? "Khóa" : "Mở khóa"}
          </Button>

          <Popconfirm
            title={
              <>
                Xóa tài khoản <strong>{row.name}</strong>?
              </>
            }
            okText="Xóa"
            cancelText="Hủy"
            onConfirm={() => deleteUser(row.id)}
          >
            <Button size="small" danger icon={<Trash2 size={16} />}>
              Xóa
            </Button>
          </Popconfirm>
        </Space>
      ),
      width: 240,
    },
  ];

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div className={styles.headerLeft}>
          <Title level={4} className={styles.title}>
            Quản lý tài khoản
          </Title>
          <Text type="secondary">Quản lý tài khoản giáo viên và học sinh</Text>
        </div>

        <Space wrap>
          <Input
            allowClear
            placeholder="Tìm theo tên/email/role/status..."
            value={q}
            onChange={(e) => {
              setQ(e.target.value);
              setPage(1);
            }}
            style={{ width: 280 }}
          />

          <Button
            icon={<Users size={16} />}
            onClick={() => setOpenStudentBulk(true)}
          >
            Thêm học sinh
          </Button>

          <Button
            type="primary"
            icon={<Plus size={16} />}
            onClick={openCreateTeacher}
          >
            Thêm giáo viên
          </Button>
        </Space>
      </div>

      {/* Table */}
      <div className={styles.tableCard}>
        <Table
          rowKey="id"
          dataSource={dataSource}
          columns={columns}
          pagination={false}
          locale={{ emptyText: <Empty description="Chưa có người dùng" /> }}
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

      {/* Modal: Thêm/Sửa giáo viên */}
      <Modal
        title={editingUser ? "Chỉnh sửa giáo viên" : "Thêm giáo viên mới"}
        open={openTeacherModal}
        onCancel={() => setOpenTeacherModal(false)}
        onOk={submitTeacher}
        okText={editingUser ? "Cập nhật" : "Tạo tài khoản"}
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={teacherForm}
          initialValues={{ email: "", name: "", password: "" }}
        >
          <Form.Item
            label="Email"
            name="email"
            rules={[
              { required: true, type: "email", message: "Email không hợp lệ" },
            ]}
          >
            <Input placeholder="teacher@school.com" />
          </Form.Item>
          <Form.Item
            label="Họ và tên"
            name="name"
            rules={[{ required: true, message: "Vui lòng nhập họ tên" }]}
          >
            <Input placeholder="Nguyễn Văn A" />
          </Form.Item>
          {!editingUser && (
            <Form.Item
              label="Mật khẩu"
              name="password"
              rules={[{ required: true, message: "Vui lòng nhập mật khẩu" }]}
            >
              <Input.Password placeholder="••••••••" />
            </Form.Item>
          )}
        </Form>
      </Modal>

      {/* Modal: Thêm học sinh hàng loạt */}
      <Modal
        title="Thêm học sinh hàng loạt"
        open={openStudentBulk}
        onCancel={() => setOpenStudentBulk(false)}
        onOk={submitStudentBulk}
        okText="Thêm học sinh"
        destroyOnClose
      >
        <Text type="secondary">
          Nhập mỗi dòng theo định dạng: <Text code>email,họ tên,mật khẩu</Text>
        </Text>
        <Divider />
        <Input.TextArea
          rows={10}
          value={bulkText}
          onChange={(e) => setBulkText(e.target.value)}
          placeholder={
            "student1@school.com,Nguyễn Văn A,password123\nstudent2@school.com,Trần Thị B,password456\nstudent3@school.com,Lê Văn C,password789"
          }
        />
      </Modal>
    </div>
  );
}
