import { useMemo } from "react";
import { Layout, Menu, Button, Typography, Space, message } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import {
  LogOut,
  GraduationCap,
  FileText,
  HelpCircle,
  ClipboardList,
  BarChart,
  BarChart2,
} from "lucide-react";
import styles from "../../assets/styles/LayoutTeacher.module.scss";
import { callLogout } from "../../services/api.service";

const { Sider, Content, Header } = Layout;
const { Text, Title } = Typography;

/**
 * props:
 *  - user?: { name?: string }
 *  - onLogout: () => void
 */
export default function LayoutTeacher() {
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const onLogout = async () => {
    const res = await callLogout();
    if (res.success === true) {
      localStorage.removeItem("access_token");
      cookieStore.delete("refresh_token");
      window.location.href = "/login";
      message.success("Đăng xuất thành công!");
    }
  };
  // key = path để navigate(e.key)
  const menuItems = useMemo(
    () => [
      {
        key: "/teacher",
        icon: <BarChart2 size={16} />,
        label: "Tông quan",
      },
      {
        key: "/teacher/classes",
        icon: <GraduationCap size={16} />,
        label: "Lớp học của tôi",
      },
      {
        key: "/teacher/materials",
        icon: <FileText size={16} />,
        label: "Tài liệu học tập",
      },
      {
        key: "/teacher/questions",
        icon: <HelpCircle size={16} />,
        label: "Ngân hàng câu hỏi",
      },
      {
        key: "/teacher/tests",
        icon: <ClipboardList size={16} />,
        label: "Bài kiểm tra",
      },
      {
        key: "/teacher/results",
        icon: <BarChart size={16} />,
        label: "Kết quả",
      },
    ],
    []
  );

  const activeItem =
    [...menuItems]
      .sort((a, b) => b.key.length - a.key.length)
      .find((item) => pathname.startsWith(item.key)) || menuItems[0];

  const selectedKeys = [activeItem.key];
  const currentLabel = activeItem.label;

  return (
    <Layout className={styles.wrap}>
      {/* SIDEBAR */}
      <Sider width={260} theme="light" className={styles.sider}>
        <div className={styles.siderHeader}>
          <Title level={4} className={styles.brand}>
            Giáo viên
          </Title>
          <Text type="secondary" className={styles.userName}>
            Teacher
          </Text>
        </div>

        <Menu
          mode="inline"
          selectedKeys={selectedKeys}
          onClick={(e) => navigate(e.key)}
          items={menuItems}
          className={styles.menu}
        />

        <div className={styles.siderFooter}>
          {/* Nếu bạn có ChangePassword thì đặt ở đây */}
          <Button block onClick={onLogout} icon={<LogOut size={16} />}>
            Đăng xuất
          </Button>
        </div>
      </Sider>

      {/* MAIN */}
      <Layout>
        <Header className={styles.header}>
          <Space size={8}>
            <Text type="secondary">Bạn đang ở:</Text>
            <Text strong>{currentLabel}</Text>
          </Space>
        </Header>

        <Content className={styles.content}>
          {/* Route con render ở đây */}
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
