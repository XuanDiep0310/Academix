import { Card, Typography, Tag, Progress } from "antd";
import { Users, BookOpen } from "lucide-react";
import styles from "../../assets/styles/StudentClassList.module.scss";

const { Title, Text } = Typography;

/** ================== BASE DATA (no API) ================== */
const CLASSES = [
  {
    id: "1",
    name: "Toán cao cấp 1",
    code: "MATH101",
    teacherName: "Nguyễn Văn A",
    progress: 65,
    materialsCount: 12,
    testsCount: 3,
  },
  {
    id: "2",
    name: "Lập trình C++",
    code: "CS102",
    teacherName: "Trần Văn B",
    progress: 45,
    materialsCount: 8,
    testsCount: 2,
  },
];
/** ======================================================== */

export function StudentClassList() {
  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <Title level={4} className={styles.title}>
          Lớp học của tôi
        </Title>
        <Text type="secondary">Các lớp học bạn đang tham gia</Text>
      </div>

      {/* Grid */}
      <div className={styles.grid}>
        {CLASSES.map((c) => (
          <Card key={c.id} className={styles.card} bordered>
            <div className={styles.cardHeader}>
              <div className={styles.meta}>
                <div className={styles.cardTitle}>{c.name}</div>
                <Tag className={styles.tagCode}>{c.code}</Tag>
              </div>
            </div>

            <div className={styles.section}>
              <div className={styles.rowBetween}>
                <Text type="secondary">Tiến độ học tập</Text>
                <Text strong>{c.progress}%</Text>
              </div>
              <Progress percent={c.progress} size="small" />
            </div>

            <div className={styles.inline}>
              <Users size={16} />
              <span>Giáo viên: {c.teacherName}</span>
            </div>

            <div className={styles.footer}>
              <div className={styles.inlineMuted}>
                <BookOpen size={16} />
                <span>{c.materialsCount} tài liệu</span>
              </div>
              <div className={styles.inlineMuted}>
                <span>{c.testsCount} bài kiểm tra</span>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </div>
  );
}
