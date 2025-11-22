import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import {
  Card,
  Typography,
  Tag,
  Button,
  Drawer,
  Table,
  Badge,
  Space,
  message,
  Empty,
  Skeleton,
} from "antd";
import { Users, BookOpen, Tag as TagIcon, Eye } from "lucide-react";
import styles from "../../../assets/styles/ClassList.module.scss";
import {
  callListMyClassesAPI,
  callListStudentOnClassesAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;

export default function ClassList() {
  const user = useSelector((state) => state.account.user);
  const teacherId = user?.userId;

  const [classes, setClasses] = useState([]);
  const [loadingClasses, setLoadingClasses] = useState(false);

  const [selected, setSelected] = useState(null);
  const [open, setOpen] = useState(false);

  const [students, setStudents] = useState([]);
  const [loadingStudents, setLoadingStudents] = useState(false);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // --- API Calls ---
  const fetchMyClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();

      if (res && res.success === true) {
        const apiClasses = Array.isArray(res.data) ? res.data : [];

        const mapped = apiClasses.map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
          description: c.description,
          isActive: c.isActive,
          joinedAt: c.joinedAt,
          myRole: c.myRole,
          studentCount: c.studentCount ?? 0,
          teacherCount: c.teacherCount ?? 0,
        }));
        setClasses(mapped);
      } else {
        message.error("Không thể tải danh sách lớp học của bạn");
      }
    } catch (err) {
      console.error("fetchMyClasses error:", err);
      message.error("Có lỗi xảy ra khi tải danh sách lớp học");
    } finally {
      setLoadingClasses(false);
    }
  };

  useEffect(() => {
    if (teacherId) {
      fetchMyClasses();
    }
  }, [teacherId]);

  const handleOpenStudents = async (cls) => {
    setSelected(cls);
    setOpen(true);
    setStudents([]);
    setPage(1);

    try {
      setLoadingStudents(true);
      const res = await callListStudentOnClassesAPI(cls.id);
      if (res && res.success && Array.isArray(res.data)) {
        const mapped = res.data.map((m) => ({
          id: m.userId,
          name: m.fullName,
          email: m.email,
          // Giả định trạng thái là 'active' hoặc 'inactive'
          status: Math.random() > 0.1 ? "active" : "inactive",
        }));
        setStudents(mapped);
      } else {
        setStudents([]);
      }
    } catch (err) {
      message.error("Không thể tải danh sách học sinh");
    } finally {
      setLoadingStudents(false);
    }
  };

  // ================== COLUMNS TABLE HỌC SINH ==================
  const columns = useMemo(
    () => [
      {
        title: "STT",
        dataIndex: "index",
        key: "index",
        width: 70,
        render: (_v, _r, i) => (page - 1) * pageSize + i + 1,
      },
      { title: "Họ và tên", dataIndex: "name", key: "name", ellipsis: true },
      { title: "Email", dataIndex: "email", key: "email", ellipsis: true },
      {
        title: "Trạng thái",
        dataIndex: "status",
        key: "status",
        width: 150,
        render: (st) =>
          st === "active" ? (
            // Màu xanh lá cho Hoạt động
            <Badge status="success" text="Hoạt động" />
          ) : (
            // Màu xám cho Không hoạt động
            <Badge status="default" text="Không hoạt động" />
          ),
      },
    ],
    [page, pageSize]
  );

  return (
    <div className={styles.wrap}>
      {/* Phần Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            <BookOpen
              size={24}
              style={{ marginRight: 8, verticalAlign: "middle" }}
            />
            Lớp học của tôi
          </Title>
          <Text type="secondary">Danh sách các lớp bạn đang giảng dạy</Text>
        </div>
      </div>

      {/* Lưới Thẻ Lớp học */}
      <div className={styles.grid}>
        {loadingClasses ? (
          // Hiển thị Skeleton khi đang tải
          Array(3)
            .fill(0)
            .map((_, i) => (
              <Card key={i} className={styles.card} bordered>
                <Skeleton active paragraph={{ rows: 2 }} />
              </Card>
            ))
        ) : classes.length === 0 ? (
          <div className={styles.emptyContainer}>
            <Empty description="Bạn chưa có lớp nào" />
          </div>
        ) : (
          // Hiển thị danh sách lớp học
          classes.map((cls) => (
            <Card key={cls.id} className={styles.card} hoverable bordered>
              <div className={styles.cardHeader}>
                <div>
                  {/* Tiêu đề lớp học dùng màu Blue chủ đạo */}
                  <Title level={5} className={styles.cardTitle}>
                    {cls.name}
                  </Title>
                  <Space size={8} className={styles.metaLine}>
                    {/* Tag mã lớp dùng màu Blue */}
                    <Tag icon={<TagIcon size={12} />} color="blue">
                      {cls.code}
                    </Tag>
                    {/* Tag trạng thái lớp dùng màu Xanh lá hoặc Đỏ */}
                    <Tag color={cls.isActive ? "green" : "red"}>
                      {cls.isActive ? "Đang hoạt động" : "Đã khóa"}
                    </Tag>
                  </Space>
                </div>
              </div>

              <Text type="secondary" className={styles.description}>
                {cls.description || "Chưa có mô tả chi tiết."}
              </Text>

              <div className={styles.cardBody}>
                <div className={styles.statLine}>
                  {/* Icon học sinh dùng màu Blue */}
                  <Users size={16} color="#1890ff" />
                  <span>
                    Tổng cộng:
                    <Text strong style={{ marginLeft: 4, color: "#0050b3" }}>
                      {cls.studentCount} học sinh
                    </Text>
                  </span>
                </div>

                {/* Button dùng màu Blue chủ đạo */}
                <Button
                  type="primary"
                  block
                  onClick={() => handleOpenStudents(cls)}
                  loading={loadingStudents && selected?.id === cls.id}
                  icon={<Eye size={16} />}
                >
                  Xem danh sách học sinh
                </Button>
              </div>
            </Card>
          ))
        )}
      </div>

      {/* Drawer danh sách học sinh */}
      <Drawer
        title={
          <Space direction="vertical" size={0}>
            <Text strong style={{ fontSize: "1.1em", color: "#0050b3" }}>
              Danh sách học sinh
            </Text>
            {selected?.name && (
              <Text style={{ fontSize: "1.4em", color: "#1890ff" }} strong>
                {selected.name}
              </Text>
            )}
            {selected?.code && (
              <Text type="secondary">
                Mã lớp: <Tag color="blue">{selected.code}</Tag>
              </Text>
            )}
          </Space>
        }
        open={open}
        onClose={() => {
          setOpen(false);
          setSelected(null);
        }}
        width={720}
        destroyOnClose={true}
      >
        <Table
          rowKey="id"
          dataSource={students}
          columns={columns}
          loading={loadingStudents}
          locale={{ emptyText: "Chưa có học sinh trong lớp" }}
          pagination={{
            current: page,
            pageSize,
            total: students.length,
            showSizeChanger: true,
            pageSizeOptions: ["10", "20", "50"],
            onChange: (p, ps) => {
              setPage(p);
              setPageSize(ps);
            },
          }}
        />
      </Drawer>
    </div>
  );
}
