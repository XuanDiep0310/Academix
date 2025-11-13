import { useEffect, useMemo, useState } from "react";
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
  Checkbox,
  Divider,
  Empty,
  notification,
} from "antd";
import { Plus, Pencil, Trash2, Users, UserPlus } from "lucide-react";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import {
  callListClassAPI,
  callListTeacherAPI,
  callListStudentAPI,
  createClassAPI,
  deleteClassAPI,
  callAddTeachersToClassAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;

const MAX_TEACHERS = 2;
const MAX_STUDENTS = 100;

export default function ClassManagement() {
  // ======================= STATE =======================
  // Lớp học (từ API)
  const [classes, setClasses] = useState([]);

  // Giáo viên / Học sinh (từ API /api/Users?role=...)
  const [teachers, setTeachers] = useState([]);
  const [students, setStudents] = useState([]);

  // UI list lớp
  const [loading, setLoading] = useState(false);
  const [q, setQ] = useState("");
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  // Form tạo/sửa lớp
  const [form] = Form.useForm();
  const [openEditor, setOpenEditor] = useState(false);
  const [editingClass, setEditingClass] = useState(null);

  // Modal thành viên
  const [openMembers, setOpenMembers] = useState(false);
  const [managingClass, setManagingClass] = useState(null);

  // Drawer chọn GV/HS
  const [openTeacherDrawer, setOpenTeacherDrawer] = useState(false);
  const [openStudentDrawer, setOpenStudentDrawer] = useState(false);
  const [selectedTeachers, setSelectedTeachers] = useState([]);
  const [selectedStudents, setSelectedStudents] = useState([]);

  // ======================= CALL API =======================

  // Lấy danh sách lớp có phân trang
  const fetchClasses = async () => {
    try {
      setLoading(true);
      const query = `page=${current}&pageSize=${pageSize}&sortBy=CreatedAt&sortOrder=desc`;
      const res = await callListClassAPI(query);
      if (res && res.success === true) {
        const apiData = res.data;

        // Map dữ liệu API -> shape local để giữ logic cũ
        const mapped = (apiData.classes || []).map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
          description: c.description,
          createdBy: c.createdBy,
          createdByName: c.createdByName,
          isActive: c.isActive,
          createdAt: c.createdAt,
          updatedAt: c.updatedAt,
          // Quan hệ GV/HS tạm thời quản lý local (API chưa cung cấp)
          teacherIds: [],
          studentIds: [],
          // giữ lại count từ API nếu muốn show ở nơi khác
          teacherCountApi: c.teacherCount,
          studentCountApi: c.studentCount,
        }));

        setClasses(mapped);
        setTotal(apiData.totalCount || mapped.length);
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchClasses error:", err);
      message.error("Có lỗi xảy ra khi tải danh sách lớp học");
    } finally {
      setLoading(false);
    }
  };

  // Lấy giáo viên (role = Teacher)
  const fetchTeachers = async () => {
    try {
      const res = await callListTeacherAPI();
      if (res && res.success === true) {
        const mapped =
          res.data.users?.map((u) => ({
            id: u.userId,
            name: u.fullName,
            email: u.email,
          })) || [];
        setTeachers(mapped);
      }
    } catch (err) {
      console.error("fetchTeachers error:", err);
    }
  };

  // Lấy học sinh (role = Student)
  const fetchStudents = async () => {
    try {
      const res = await callListStudentAPI();
      if (res && res.success === true) {
        const mapped =
          res.data.users?.map((u) => ({
            id: u.userId,
            name: u.fullName,
            email: u.email,
          })) || [];
        setStudents(mapped);
      }
    } catch (err) {
      console.error("fetchStudents error:", err);
    }
  };

  // Gọi API khi vào trang + khi đổi current/pageSize
  useEffect(() => {
    fetchClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [current, pageSize]);

  // Chỉ gọi 1 lần cho list GV/HS
  useEffect(() => {
    fetchTeachers();
    fetchStudents();
  }, []);

  // ======================= FILTER CLIENT-SIDE =======================
  const filtered = useMemo(() => {
    if (!q?.trim()) return classes;
    const key = q.toLowerCase();
    return classes.filter(
      (c) =>
        c.name?.toLowerCase().includes(key) ||
        c.code?.toLowerCase().includes(key) ||
        c.description?.toLowerCase().includes(key)
    );
  }, [classes, q]);

  // ======================= HELPER =======================
  const getTeacher = (id) => teachers.find((t) => String(t.id) === String(id));
  const getStudent = (id) => students.find((s) => String(s.id) === String(id));

  // ======================= CRUD LỚP (LOCAL + TODO API) =======================
  const openCreate = () => {
    setEditingClass(null);
    form.resetFields();
    setOpenEditor(true);
  };

  const openEdit = (row) => {
    setEditingClass(row);
    form.setFieldsValue({
      className: row.name,
      description: row.description,
      isActive: row.isActive,
    });
    setOpenEditor(true);
  };

  const handleSubmit = async () => {
    const values = await form.validateFields();
    try {
      if (editingClass) {
        console.log("Editing class with values:", values);
      } else {
        console.log("Creating class with values:", values);
        const res = await createClassAPI(
          values.name,
          values.code,
          values.description
        );
        if (res && res.success === true) {
          message.success("Đã tạo lớp học thành công");
          setCurrent(1);
          fetchClasses();
        } else {
          notification.error({
            message: "Tạo lớp học thất bại",
            description: res.message || "Có lỗi xảy ra khi tạo lớp học",
          });
          return;
        }
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
      const res = await deleteClassAPI(id);
      if (res && res.success === true) {
        message.success("Đã xóa lớp học");
        fetchClasses();
      } else {
        notification.error({
          message: "Xóa lớp học thất bại",
          description: res.message || "Có lỗi xảy ra khi xóa lớp học",
        });
        return;
      }
    } catch {
      message.error("Xóa thất bại");
    }
  };

  // ======================= THÀNH VIÊN (LOCAL DEMO) =======================
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
      message.success("Đã xóa giáo viên khỏi lớp (local)");
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
      message.success("Đã xóa học sinh khỏi lớp (local)");
    } catch {
      message.error("Thao tác thất bại");
    }
  };

  // Drawer chọn GV/HS
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
      const res = await callAddTeachersToClassAPI(
        managingClass.id,
        selectedTeachers
      );
      console.log("Add teachers API response:", res);
      if (res && res.success === true) {
        message.success("Đã cập nhật giáo viên cho lớp");
        setOpenTeacherDrawer(false);
        await fetchClasses();
      } else {
        notification.error({
          message: "Cập nhật giáo viên thất bại",
          description: res.message || "Có lỗi xảy ra khi cập nhật giáo viên",
        });
        return;
      }
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
      message.success("Đã cập nhật học sinh (local)");
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

  // ======================= PAGINATION HANDLER (giống Users) =======================
  const handleOnChangePagi = (pagination, filters, sorter) => {
    if (
      pagination &&
      pagination.pageSize &&
      +pagination.pageSize !== +pageSize
    ) {
      setPageSize(+pagination.pageSize);
      setCurrent(1); // đổi size thì về trang 1
    }

    if (pagination && pagination.current && +pagination.current !== +current) {
      setCurrent(+pagination.current);
    }

    // nếu sau này cần sort server-side, xử lý thêm ở đây
  };

  // ======================= COLUMNS =======================
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
          <Badge count={`${row.teacherCountApi}/${MAX_TEACHERS}`} />
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
          <Badge count={`${row.studentCountApi}/${MAX_STUDENTS}`} />
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
    {
      title: "Ngày tạo",
      dataIndex: "createdAt",
      key: "createdAt",
      width: 180,
      render: (val) => (
        <Text type="secondary">
          {val ? new Date(val).toLocaleString() : "--"}
        </Text>
      ),
    },
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

  // ======================= RENDER =======================
  return (
    <div className={styles.wrapper}>
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Quản lý lớp học
          </Title>
          <Text type="secondary">
            Kết nối API lớp học + quản lý giáo viên / học sinh trong lớp (demo
            local)
          </Text>
        </div>

        <Space>
          <Input
            allowClear
            placeholder="Tìm kiếm tên/mã lớp..."
            value={q}
            onChange={(e) => {
              setQ(e.target.value);
              setCurrent(1);
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
          dataSource={filtered}
          columns={columns}
          loading={{
            spinning: loading,
            tip: "Đang tải danh sách lớp học...",
          }}
          locale={{ emptyText: <Empty description="Chưa có dữ liệu" /> }}
          onChange={handleOnChangePagi}
          pagination={{
            current,
            pageSize,
            total,
            showSizeChanger: true,
            pageSizeOptions: [5, 10, 20, 50],
            showTotal: (total, range) =>
              `${range[0]}-${range[1]} trên ${total} lớp học`,
          }}
          scroll={{ x: 900 }}
          size="middle"
          sticky
        />
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
