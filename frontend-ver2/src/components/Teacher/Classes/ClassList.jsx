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
} from "antd";
import { Users } from "lucide-react";
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
    fetchMyClasses();
  }, [teacherId]);

  const handleOpenStudents = async (cls) => {
    setSelected(cls);
    setOpen(true);
    setStudents([]);
    try {
      setLoadingStudents(true);
      const res = await callListStudentOnClassesAPI(cls.id);
      if (res && res.success && Array.isArray(res.data)) {
        const mapped = res.data.map((m) => ({
          id: m.userId,
          name: m.fullName,
          email: m.email,
          // API chưa có status, tạm cho "active"
          status: "active",
        }));
        setStudents(mapped);
      } else {
        setStudents([]);
      }
    } catch (err) {
      console.error("fetch students error:", err);
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
        render: (_v, _r, i) => i + 1,
      },
      { title: "Họ và tên", dataIndex: "name", key: "name" },
      { title: "Email", dataIndex: "email", key: "email" },
      {
        title: "Trạng thái",
        dataIndex: "status",
        key: "status",
        width: 150,
        render: (st) =>
          st === "active" ? (
            <Badge status="success" text="Hoạt động" />
          ) : (
            <Badge status="default" text="Không hoạt động" />
          ),
      },
    ],
    []
  );

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Lớp học của tôi
          </Title>
          <Text type="secondary">Danh sách các lớp bạn đang giảng dạy</Text>
        </div>
      </div>

      <div className={styles.grid}>
        {loadingClasses ? (
          <div style={{ width: "100%", textAlign: "center", marginTop: 32 }}>
            <Text type="secondary">Đang tải danh sách lớp...</Text>
          </div>
        ) : classes.length === 0 ? (
          <div style={{ width: "100%", marginTop: 32 }}>
            <Empty description="Bạn chưa có lớp nào" />
          </div>
        ) : (
          classes.map((cls) => (
            <Card key={cls.id} className={styles.card} bordered>
              <div className={styles.cardHeader}>
                <div>
                  <Title level={5} style={{ margin: 0 }}>
                    {cls.name}
                  </Title>
                  <Space size={8} className={styles.metaLine}>
                    <Tag>{cls.code}</Tag>
                  </Space>
                </div>
              </div>

              <div className={styles.cardBody}>
                <div className={styles.statLine}>
                  <Users size={16} />
                  <span>{cls.studentCount} học sinh</span>
                </div>

                <Button
                  block
                  onClick={() => handleOpenStudents(cls)}
                  disabled={loadingStudents && selected?.id === cls.id}
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
            <Text strong>
              Danh sách học sinh
              {selected?.name ? ` - ${selected.name}` : ""}
            </Text>
            {selected?.code && (
              <Text type="secondary">Mã lớp: {selected.code}</Text>
            )}
          </Space>
        }
        open={open}
        onClose={() => setOpen(false)}
        width={720}
      >
        <Table
          rowKey="id"
          dataSource={students}
          columns={columns}
          loading={loadingStudents}
          locale={{ emptyText: "Chưa có học sinh trong lớp" }}
          pagination={{ pageSize: 10 }}
        />
      </Drawer>
    </div>
  );
}
