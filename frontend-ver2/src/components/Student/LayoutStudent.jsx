import { useMemo } from "react";
import { Outlet, useLocation, useNavigate } from "react-router";
import { Layout, Menu, Button, Typography, Space, message } from "antd";
import {
  LogOut,
  GraduationCap,
  FileText,
  ClipboardCheck,
  BarChart,
} from "lucide-react";
import styles from "../../assets/styles/LayoutStudent.module.scss";
import { callLogout } from "../../services/api.service";

const { Sider, Content, Header } = Layout;
const { Text, Title } = Typography;

export default function LayoutStudent() {
  const navigate = useNavigate();
  const location = useLocation();

  // map key ↔ route path
  const items = useMemo(
    () => [
      {
        key: "classes",
        label: "Lớp học của tôi",
        icon: <GraduationCap size={16} />,
        path: "/student/classes",
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

  // chọn menu theo URL
  const selectedKey = useMemo(() => {
    if (location.pathname.startsWith("/student/materials")) return "materials";
    if (location.pathname.startsWith("/student/tests")) return "tests";
    if (location.pathname.startsWith("/student/results")) return "results";
    return "classes"; // /student hoặc /student/
  }, [location.pathname]);

  const onMenuClick = ({ key }) => {
    const found = items.find((i) => i.key === key);
    if (found) navigate(found.path);
  };

  const userName = "Student"; // có thể lấy từ context/store của bạn
  const handleLogout = async () => {
    const res = await callLogout();
    if (res.success === true) {
      localStorage.removeItem("access_token");
      cookieStore.delete("refresh_token");
      window.location.href = "/login";
      message.success("Đăng xuất thành công!");
    }
  };

  const breadcrumbTitle = {
    classes: "Lớp học của tôi",
    materials: "Tài liệu học tập",
    tests: "Bài kiểm tra",
    results: "Kết quả của tôi",
  }[selectedKey];

  return (
    <Layout className={styles.wrapper}>
      {/* SIDEBAR */}
      <Sider width={260} theme="light" className={styles.sider}>
        <div className={styles.siderHeader}>
          <Title level={4} className={styles.brand}>
            Học sinh
          </Title>
          <Text type="secondary" className={styles.userName}>
            {userName}
          </Text>
        </div>

        <Menu
          mode="inline"
          selectedKeys={[selectedKey]}
          onClick={onMenuClick}
          items={items.map((i) => ({
            key: i.key,
            icon: i.icon,
            label: i.label,
          }))}
          className={styles.menu}
        />

        <div className={styles.siderFooter}>
          <Button block onClick={handleLogout} icon={<LogOut size={16} />}>
            Đăng xuất
          </Button>
        </div>
      </Sider>

      {/* MAIN */}
      <Layout>
        <Header className={styles.header}>
          <Space size={8}>
            <Text type="secondary">Bạn đang ở:</Text>
            <Text strong>{breadcrumbTitle}</Text>
          </Space>
        </Header>

        <Content className={styles.content}>
          {/* Route children render ở đây */}
          <div className={styles.panel}>
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
}
