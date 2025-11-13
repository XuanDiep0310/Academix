import { useEffect, useState } from "react";
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
  Divider,
  Empty,
  message,
  notification,
} from "antd";
import { Plus, Pencil, Trash2, Lock, Unlock, Users, Eye } from "lucide-react";

import styles from "../../../assets/styles/UserManagement.module.scss";
import {
  callBulkCreateUser,
  callListUserAPI,
} from "../../../services/api.service";
import UserDetail from "./UserDetail";
import moment from "moment";

const { Title, Text } = Typography;

// Hàm map user từ API về dạng dùng cho UI
const mapApiUserToRow = (u) => {
  return {
    id: u.userId, // dùng userId của API làm rowKey
    email: u.email,
    name: u.fullName,
    role:
      u.role === "Teacher"
        ? "teacher"
        : u.role === "Student"
        ? "student"
        : u.role?.toLowerCase(),
    status: u.isActive ? "active" : "locked",
    createdAt: u.createdAt, // "2025-11-13"
    updateAt: u.updatedAt, // "2025-11-13"
  };
};

export default function UserManagement() {
  // Dữ liệu từ API
  const [users, setUsers] = useState([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);

  // UI state
  const [q, setQ] = useState("");

  // Phân trang
  const [pageSize, setPageSize] = useState(10);
  const [current, setCurrent] = useState(1);

  // Modal/Forms
  const [openTeacherModal, setOpenTeacherModal] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [teacherForm] = Form.useForm();

  const [openStudentBulk, setOpenStudentBulk] = useState(false);
  const [bulkText, setBulkText] = useState("");
  const [isBulkSubmitting, setIsBulkSubmitting] = useState(false);

  // Modal chi tiết user
  const [userDetail, setUserDetail] = useState(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const openUserDetail = (row) => {
    setUserDetail(row);
    setIsDetailOpen(true);
  };

  /* ======================= CALL API DANH SÁCH USER ======================= */
  const fetchUsers = async () => {
    try {
      setLoading(true);

      const params = new URLSearchParams();
      params.append("page", String(current));
      params.append("pageSize", String(pageSize));

      if (q.trim()) {
        params.append("search", q.trim());
      }

      const query = params.toString();
      const res = await callListUserAPI(query);

      const data = res.data; // <-- chính là object bạn chụp hình

      const items = data.users || [];
      const totalItems = data.totalCount || 0;

      const mapped = items.map(mapApiUserToRow);
      setUsers(mapped);
      setTotal(totalItems);

      // nếu muốn sync ngược lại từ API (không bắt buộc)
      if (data.page && data.page !== current) {
        setCurrent(data.page);
      }
      if (data.pageSize && data.pageSize !== pageSize) {
        setPageSize(data.pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Không tải được danh sách người dùng");
    } finally {
      setLoading(false);
    }
  };

  // Gọi API khi current / pageSize / q thay đổi
  useEffect(() => {
    fetchUsers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [current, pageSize, q]);

  /* ======================= HANDLE PHÂN TRANG TABLE ======================= */
  const handleOnChangePagi = (pagination, filters, sorter) => {
    if (
      pagination &&
      pagination.pageSize &&
      +pagination.pageSize !== +pageSize
    ) {
      setPageSize(+pagination.pageSize);
      setCurrent(1); // đổi size thì nhảy về trang 1
    }

    if (pagination && pagination.current && +pagination.current !== +current) {
      setCurrent(+pagination.current);
    }

    // Nếu sau này backend có sort thì xử lý thêm ở đây
    // if (sorter && sorter.order) {
    //   const q =
    //     sorter.order === "ascend"
    //       ? `sort=${sorter.field}`
    //       : `sort=-${sorter.field}`;
    //   if (q) setSortQuery(q);
    // }
  };

  /* --------------------- Giáo viên: tạo/sửa --------------------- */
  const openCreateTeacher = () => {
    setEditingUser(null);
    teacherForm.resetFields();
    setOpenTeacherModal(true);
  };

  const openEditUser = (row) => {
    // KHÔNG check role nữa
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
      // TODO: gọi API update giáo viên
      // await updateTeacherApi(editingUser.id, values)

      // Tạm thời cập nhật local cho đẹp UI
      setUsers((prev) =>
        prev.map((u) =>
          u.id === editingUser.id
            ? { ...u, email: values.email, name: values.name }
            : u
        )
      );
      message.success("Đã cập nhật giáo viên");
    } else {
      // TODO: gọi API tạo giáo viên
      // const created = await createTeacherApi(values)
      // const mapped = mapApiUserToRow(created)

      // Tạm thời giả lập thêm local
      const fakeCreated = {
        userId: Date.now(),
        email: values.email,
        fullName: values.name,
        role: "Teacher",
        isActive: true,
        createdAt: new Date().toISOString(),
      };
      const mapped = mapApiUserToRow(fakeCreated);

      setUsers((prev) => [mapped, ...prev]);
      setCurrent(1);
      message.success("Đã tạo tài khoản giáo viên");
    }

    setOpenTeacherModal(false);
    setEditingUser(null);
    teacherForm.resetFields();
  };

  /* --------------------- Học sinh: thêm hàng loạt --------------------- */
  const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));
  const submitStudentBulk = async () => {
    setIsBulkSubmitting(true);
    const lines = bulkText
      .split("\n")
      .map((l) => l.trim())
      .filter(Boolean);

    const apiUsers = [];

    lines.forEach((line) => {
      const [email, fullName, password] = line.split(",").map((p) => p?.trim());
      if (email && fullName && password) {
        apiUsers.push({
          email,
          fullName,
          password,
          role: "Student",
        });
      }
    });

    if (!apiUsers.length) {
      message.error("Không có dòng hợp lệ (định dạng: email,họ tên,mật khẩu)");
      return;
    }

    try {
      // Gọi API bulk
      const res = await callBulkCreateUser({ users: apiUsers });
      if (res && res.success === true) {
        await delay(800);
        setBulkText("");
        setOpenStudentBulk(false);
        notification.success({
          message: "Success",
          description: `${res.message || ""}`,
        });
        // Về trang 1 và load lại danh sách từ server
        setCurrent(1);
        await fetchUsers();
      } else {
        await delay(800);
        notification.error({
          message: "Error",
          description: `${
            JSON.stringify(res.message) ||
            "Có lỗi xảy ra khi tạo tài khoản học sinh"
          }`,
        });
      }
    } catch (error) {
      console.error(error);
      const msgFromServer =
        error?.response?.data?.message ||
        error?.response?.data?.error ||
        "Có lỗi xảy ra khi tạo tài khoản học sinh";
      message.error(msgFromServer);
    } finally {
      setIsBulkSubmitting(false);
    }
  };

  /* --------------------- Khóa/Mở khóa & Xóa --------------------- */
  const toggleStatus = (id) => {
    // TODO: gọi API toggle active/locked
    setUsers((prev) =>
      prev.map((u) =>
        u.id === id
          ? { ...u, status: u.status === "active" ? "locked" : "active" }
          : u
      )
    );
  };

  const deleteUser = (id) => {
    // TODO: gọi API delete user
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
      render: (role) => {
        const roleMap = {
          teacher: { label: "Giáo viên", color: "geekblue" },
          student: { label: "Học sinh", color: "green" },
          admin: { label: "Quản trị", color: "volcano" },
        };

        const r = roleMap[role] || { label: role, color: "default" };

        return <Tag color={r.color}>{r.label}</Tag>;
      },
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
    {
      title: "Ngày tạo",
      dataIndex: "createdAt",
      key: "createdAt",
      render: (_, row) => moment(row.createdAt).format("DD-MM-YYYY"),
      width: 140,
    },
    {
      title: "Thao tác",
      key: "actions",
      align: "right",
      width: 300,
      render: (_, row) => (
        <Space>
          {/* Chi tiết */}
          <Button
            size="small"
            type="default"
            onClick={() => openUserDetail(row)}
          >
            Chi tiết
          </Button>

          {/* Sửa (GV + HS luôn) */}
          <Button
            size="small"
            type="primary"
            ghost
            icon={<Pencil size={16} />}
            onClick={() => openEditUser(row)} // nhớ đổi hàm như mình nói ở tin trước
          >
            Sửa
          </Button>

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
    },
  ];

  return (
    <>
      <div className={styles.wrap}>
        {/* Header */}
        <div className={styles.header}>
          <div className={styles.headerLeft}>
            <Title level={4} className={styles.title}>
              Quản lý tài khoản
            </Title>
            <Text type="secondary">
              Quản lý tài khoản giáo viên và học sinh
            </Text>
          </div>

          <Space wrap>
            <Input
              allowClear
              placeholder="Tìm theo tên/email/role/status..."
              value={q}
              onChange={(e) => {
                setQ(e.target.value);
                setCurrent(1);
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
            dataSource={users}
            columns={columns}
            loading={loading}
            locale={{ emptyText: <Empty description="Chưa có người dùng" /> }}
            onChange={handleOnChangePagi}
            pagination={{
              current,
              pageSize,
              total,
              showSizeChanger: true,
              pageSizeOptions: [5, 10, 20, 50],
              showTotal: (total, range) =>
                `${range[0]}-${range[1]} trên ${total} tài khoản`,
            }}
            scroll={{ x: 900 }}
            size="middle"
            sticky
          />
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
                {
                  required: true,
                  type: "email",
                  message: "Email không hợp lệ",
                },
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
          confirmLoading={isBulkSubmitting}
          destroyOnClose
        >
          <Text type="secondary">
            Nhập mỗi dòng theo định dạng:{" "}
            <Text code>email,họ tên,mật khẩu</Text>
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
      <UserDetail
        userDetail={userDetail}
        setUserDetail={setUserDetail}
        isDetailOpen={isDetailOpen}
        setIsDetailOpen={setIsDetailOpen}
      />
    </>
  );
}
