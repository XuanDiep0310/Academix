import { useMemo, useState } from "react";
import {
  Card,
  Typography,
  Tag,
  Button,
  Drawer,
  Table,
  Badge,
  Space,
} from "antd";
import { Users } from "lucide-react";
import styles from "../../assets/styles/ClassList.module.scss";

const { Title, Text } = Typography;

/* ===================== DATASET (có sẵn trong file) ===================== */
const CLASSES = [
  {
    id: "1",
    name: "Toán cao cấp 1",
    code: "MATH101",
    studentCount: 35,
    students: [
      {
        id: "1",
        name: "Trần Thị C",
        email: "student@school.com",
        status: "active",
      },
      {
        id: "2",
        name: "Lê Văn D",
        email: "student2@school.com",
        status: "active",
      },
      {
        id: "3",
        name: "Nguyễn Văn E",
        email: "student3@school.com",
        status: "inactive",
      },
    ],
  },
  {
    id: "2",
    name: "Đại số tuyến tính",
    code: "MATH102",
    studentCount: 28,
    students: [
      {
        id: "1",
        name: "Trần Thị C",
        email: "student@school.com",
        status: "active",
      },
      {
        id: "4",
        name: "Phạm Thị F",
        email: "student4@school.com",
        status: "active",
      },
    ],
  },
];
/* ====================================================================== */

export default function ClassList() {
  const [selected, setSelected] = useState(null);
  const [open, setOpen] = useState(false);

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
        {CLASSES.map((cls) => (
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
                onClick={() => {
                  setSelected(cls);
                  setOpen(true);
                }}
              >
                Xem danh sách học sinh
              </Button>
            </div>
          </Card>
        ))}
      </div>

      <Drawer
        title={
          <Space direction="vertical" size={0}>
            <Text strong>Danh sách học sinh - {selected?.name}</Text>
            <Text type="secondary">Mã lớp: {selected?.code}</Text>
          </Space>
        }
        open={open}
        onClose={() => setOpen(false)}
        width={720}
      >
        <Table
          rowKey="id"
          dataSource={selected?.students || []}
          columns={columns}
          pagination={{ pageSize: 10 }}
        />
      </Drawer>
    </div>
  );
}
