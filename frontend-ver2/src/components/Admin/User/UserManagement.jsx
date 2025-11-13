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
  Spin,
} from "antd";
import { Plus, Pencil, Trash2, Lock, Unlock, Users, Eye } from "lucide-react";
import * as XLSX from "xlsx";
import { Upload } from "antd";

import styles from "../../../assets/styles/UserManagement.module.scss";
import {
  callBulkCreateUser,
  callListUserAPI,
  createUserAPI,
  deleteUserAPI,
  editUserAPI,
  editUserStatusAPI,
} from "../../../services/api.service";
import UserDetail from "./UserDetail";
import moment from "moment";
import UserImportModal from "./data/UserImportModal";

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
  const [isTeacherSubmitting, setIsTeacherSubmitting] = useState(false);
  const [teacherForm] = Form.useForm();

  const [openStudentBulk, setOpenStudentBulk] = useState(false);
  const [bulkText, setBulkText] = useState("");
  const [isBulkSubmitting, setIsBulkSubmitting] = useState(false);

  // Modal chi tiết user
  const [userDetail, setUserDetail] = useState(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  // Import Excel
  const [isImportOpen, setIsImportOpen] = useState(false);

  const openUserDetail = (row) => {
    setUserDetail(row);
    setIsDetailOpen(true);
  };

  /* ======================= CALL API DANH SÁCH USER ======================= */
  const fetchUsers = async () => {
    try {
      const params = new URLSearchParams();
      params.append("page", String(current));
      params.append("pageSize", String(pageSize));

      if (q.trim()) {
        params.append("search", q.trim());
      }

      const query = params.toString();
      const res = await callListUserAPI(query);
      setLoading(true);
      delay(500);
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

  useEffect(() => {
    fetchUsers();
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
    try {
      const values = await teacherForm.validateFields();
      setIsTeacherSubmitting(true);

      if (editingUser) {
        // TODO: gọi API update giáo viên
        // await updateTeacherApi(editingUser.id, values)
        const res = await editUserAPI(
          editingUser.id,
          values.name,
          values.email
        );
        if (res && res.success === true) {
          await delay(700);
          message.success("Đã cập nhật giáo viên");
          setOpenTeacherModal(false);
          setEditingUser(null);
          await fetchUsers();
        }
      } else {
        const res = await createUserAPI(
          values.name,
          values.email,
          values.password,
          "Teacher"
        );

        if (res && res.success === true) {
          const mapped = mapApiUserToRow(res.data);
          await delay(700);

          setUsers((prev) => [mapped, ...prev]);
          setCurrent(1);
          message.success("Đã tạo tài khoản giáo viên");

          setOpenTeacherModal(false);
          setEditingUser(null);
          teacherForm.resetFields();
          await fetchUsers();
        } else {
          notification.error({
            message: "Error",
            description:
              JSON.stringify(res?.message) ||
              "Có lỗi xảy ra khi tạo tài khoản giáo viên",
          });
        }
      }
    } catch (err) {
      // nếu lỗi validate form thì bỏ qua
      if (err?.errorFields) return;
      notification.error({
        message: "Error",
        description: "Có lỗi xảy ra khi xử lý tài khoản giáo viên",
      });
    } finally {
      setIsTeacherSubmitting(false);
    }
  };

  /* --------------------- Học sinh: tạo hàng loạt --------------------- */
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

  const toggleStatus = async (row) => {
    console.log("Toggling status for", row);
    const res = await editUserStatusAPI(
      row.id,
      row.name,
      row.email,
      row.status === "active" ? false : true
    );
    if (res && res.success === true) {
      message.success(
        `Đã ${row.status === "active" ? "khóa" : "mở khóa"} tài khoản`
      );
      await fetchUsers();
    }
  };

  const deleteUser = async (id) => {
    const res = await deleteUserAPI(id);
    if (res && res.success === true) {
      message.success("Đã xóa tài khoản");
      await fetchUsers();
    } else {
      notification.error({
        message: "Error",
        description:
          JSON.stringify(res?.message) || "Có lỗi xảy ra khi xóa tài khoản",
      });
    }
  };

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
            onClick={() => toggleStatus(row)}
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
  const handleExportExcel = () => {
    if (!users || users.length === 0) {
      notification.warning({
        message: "Không có dữ liệu",
        description: "Hiện chưa có tài khoản nào để xuất Excel",
      });
      return;
    }

    // map dữ liệu cho gọn, chỉ những cột bạn muốn
    const data = users.map((u) => ({
      Email: u.email,
      "Họ và tên": u.name,
      Role: u.role,
      Trạng_thái: u.status,
    }));

    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Users");

    XLSX.writeFile(workbook, "users.xlsx");
  };
  const handleImportExcel = async (file) => {
    try {
      const reader = new FileReader();
      reader.onload = async (e) => {
        const data = e.target?.result;
        if (!data) return;

        const workbook = XLSX.read(data, { type: "binary" });
        const firstSheetName = workbook.SheetNames[0];
        const worksheet = workbook.Sheets[firstSheetName];

        const json = XLSX.utils.sheet_to_json(worksheet);

        // Kỳ vọng file có cột: Email, Họ và tên, Mật khẩu, Role (Teacher/Student)
        const apiUsers = json
          .map((row) => ({
            email: row["Email"]?.toString().trim(),
            fullName: row["Họ và tên"]?.toString().trim(),
            password: row["Mật khẩu"]?.toString().trim(),
            role: row["Role"]?.toString().trim() || "Student",
          }))
          .filter((u) => u.email && u.fullName && u.password);

        if (apiUsers.length === 0) {
          notification.error({
            message: "File không hợp lệ",
            description:
              "Không tìm thấy dòng nào có đủ Email / Họ và tên / Mật khẩu",
          });
          return;
        }

        // Gọi API bulk (dùng API bạn đã có)
        const res = await callBulkCreateUser({ users: apiUsers });

        if (res && res.success) {
          notification.success({
            message: "Import thành công",
            description: res.message || "Đã tạo tài khoản từ file Excel",
          });
          setCurrent(1);
          await fetchUsers();
        } else {
          notification.error({
            message: "Import thất bại",
            description:
              JSON.stringify(res?.message) ||
              "Có lỗi xảy ra khi import file Excel",
          });
        }
      };

      reader.readAsBinaryString(file);
    } catch (err) {
      console.error(err);
      notification.error({
        message: "Lỗi",
        description: "Không thể đọc file Excel",
      });
    }

    // ngăn Upload auto gửi lên server
    return false;
  };
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
            <Button onClick={handleExportExcel}>Xuất Excel</Button>

            <Button onClick={() => setIsImportOpen(true)}>
              Thêm bằng Excel
            </Button>
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
            loading={{
              spinning: loading,
              tip: "Đang tải danh sách tài khoản...",
            }}
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
          title={editingUser ? "Chỉnh sữa" : "Thêm giáo viên mới"}
          open={openTeacherModal}
          onCancel={() => !isTeacherSubmitting && setOpenTeacherModal(false)}
          onOk={submitTeacher}
          okText={editingUser ? "Cập nhật" : "Tạo tài khoản"}
          confirmLoading={isTeacherSubmitting} // 👈 loading ở nút
          destroyOnClose
          maskClosable={!isTeacherSubmitting} // hạn chế click ra ngoài khi đang submit
        >
          <Spin spinning={isTeacherSubmitting}>
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
                <Input
                  placeholder="teacher@school.com"
                  disabled={isTeacherSubmitting}
                />
              </Form.Item>

              <Form.Item
                label="Họ và tên"
                name="name"
                rules={[{ required: true, message: "Vui lòng nhập họ tên" }]}
              >
                <Input
                  placeholder="Nguyễn Văn A"
                  disabled={isTeacherSubmitting}
                />
              </Form.Item>

              {!editingUser && (
                <Form.Item
                  label="Mật khẩu"
                  name="password"
                  rules={[
                    { required: true, message: "Vui lòng nhập mật khẩu" },
                  ]}
                >
                  <Input.Password
                    placeholder="••••••••"
                    disabled={isTeacherSubmitting}
                  />
                </Form.Item>
              )}
            </Form>
          </Spin>
        </Modal>

        {/* Modal: Thêm học sinh hàng loạt */}
        <Modal
          title="Thêm học sinh hàng loạt"
          open={openStudentBulk}
          onCancel={() => !isBulkSubmitting && setOpenStudentBulk(false)}
          onOk={submitStudentBulk}
          okText="Thêm học sinh"
          confirmLoading={isBulkSubmitting}
          destroyOnClose
        >
          <Spin spinning={isBulkSubmitting}>
            <Text type="secondary">
              Nhập mỗi dòng theo định dạng:{" "}
              <Text code>email,họ tên,mật khẩu</Text>
            </Text>
            <Divider />
            <Input.TextArea
              rows={10}
              value={bulkText}
              onChange={(e) => setBulkText(e.target.value)}
              disabled={isBulkSubmitting}
              placeholder={
                "student1@school.com,Nguyễn Văn A,password123\n" +
                "student2@school.com,Trần Thị B,password456\n" +
                "student3@school.com,Lê Văn C,password789"
              }
            />
          </Spin>
        </Modal>
      </div>
      <UserImportModal
        open={isImportOpen}
        onClose={() => setIsImportOpen(false)}
        fetchUsers={fetchUsers}
        setCurrent={setCurrent}
      />

      <UserDetail
        userDetail={userDetail}
        setUserDetail={setUserDetail}
        isDetailOpen={isDetailOpen}
        setIsDetailOpen={setIsDetailOpen}
      />
    </>
  );
}
