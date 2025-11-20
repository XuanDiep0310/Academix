// ==================== LayoutStudent.jsx ====================
import { useMemo } from "react";
import { Outlet, useLocation, useNavigate } from "react-router";
import { Layout, Typography, Space } from "antd";
import {
  GraduationCap,
  FileText,
  ClipboardCheck,
  BarChart,
} from "lucide-react";
import styles from "../../assets/styles/LayoutStudent.module.scss";
import SiderLayout from "../Sider/SiderLayout";

const { Content, Header } = Layout;
const { Text } = Typography;

export default function LayoutStudent() {
  const navigate = useNavigate();
  const location = useLocation();

  const items = useMemo(
    () => [
      {
        key: "classes",
        label: "Lớp học của tôi",
        icon: <GraduationCap size={16} />,
        path: "/student",
      },
      {
        key: "materials",
        label: "Tài liệu học tập",
        icon: <FileText size={16} />,
        path: "/student/materials",
      },
      {
        key: "tests",
        label: "Bài kiểm tra",
        icon: <ClipboardCheck size={16} />,
        path: "/student/tests",
      },
      {
        key: "results",
        label: "Kết quả của tôi",
        icon: <BarChart size={16} />,
        path: "/student/results",
      },
    ],
    []
  );

  const selectedKey = useMemo(() => {
    if (location.pathname.startsWith("/student/materials")) return "materials";
    if (location.pathname.startsWith("/student/tests")) return "tests";
    if (location.pathname.startsWith("/student/results")) return "results";
    return "classes";
  }, [location.pathname]);

  const onMenuClick = ({ key }) => {
    const found = items.find((i) => i.key === key);
    if (found) navigate(found.path);
  };

  const breadcrumbTitle = {
    classes: "Lớp học của tôi",
    materials: "Tài liệu học tập",
    tests: "Bài kiểm tra",
    results: "Kết quả của tôi",
  }[selectedKey];

  return (
    <Layout className={styles.wrapper}>
      <SiderLayout
        brandTitle="Học sinh"
        userName="Student"
        menuItems={items.map((i) => ({
          key: i.key,
          icon: i.icon,
          label: i.label,
        }))}
        selectedKeys={[selectedKey]}
        onMenuClick={onMenuClick}
        className={styles.sider}
        showChangePassword={true}
      />

      <Layout>
        <Header className={styles.header}>
          <Space size={8}>
            <Text type="secondary">Bạn đang ở:</Text>
            <Text strong>{breadcrumbTitle}</Text>
          </Space>
        </Header>

        <Content className={styles.content}>
          <div className={styles.panel}>
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
}
