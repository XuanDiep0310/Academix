"use client";
import { Layout, Menu, Badge } from "antd";
import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  HomeOutlined,
  TeamOutlined,
  ProfileOutlined,
  DatabaseOutlined,
  FileTextOutlined,
  FolderOpenOutlined,
  SafetyOutlined,
} from "@ant-design/icons";
import styles from "@/assets/styles/layoutTeacher.module.scss";
const { Sider } = Layout;

const TeacherSidebar = () => {
  const pathname = usePathname();

  // TODO: nối API thật để lấy 2 số dưới
  const pendingGrading = 0;
  const highRisk = 0;

  const items = [
    {
      key: "/teacher",
      icon: <HomeOutlined />,
      label: <Link href="/teacher">Trang chủ</Link>,
    },
    {
      key: "/teacher/students",
      icon: <TeamOutlined />,
      label: <Link href="/teacher/students">Quản lý học sinh</Link>,
    },
    {
      key: "/teacher/exams",
      icon: <ProfileOutlined />,
      label: <Link href="/teacher/exams">Tạo bài kiểm tra</Link>,
    },
    {
      key: "/teacher/questions",
      icon: <DatabaseOutlined />,
      label: <Link href="/teacher/questions">Ngân hàng câu hỏi</Link>,
    },
    {
      key: "/teacher/grading",
      icon: <FileTextOutlined />,
      label: (
        <Link href="/teacher/grading">
          Chấm điểm{" "}
          {pendingGrading > 0 && (
            <Badge count={pendingGrading} className={styles.badgeInline} />
          )}
        </Link>
      ),
    },
    {
      key: "/teacher/documents",
      icon: <FolderOpenOutlined />,
      label: <Link href="/teacher/documents">Tài liệu</Link>,
    },
    {
      key: "/teacher/cheating",
      icon: <SafetyOutlined />,
      label: (
        <Link href="/teacher/cheating">
          Theo dõi gian lận{" "}
          {highRisk > 0 && (
            <Badge count={highRisk} className={styles.badgeInline} />
          )}
        </Link>
      ),
    },
  ];

  return (
    <Sider width={260} className={styles.sider}>
      <div className={styles.brand}>Teacher Panel</div>
      <Menu
        theme="light"
        mode="inline"
        selectedKeys={[pathname]}
        items={items}
      />
    </Sider>
  );
};
export default TeacherSidebar;
