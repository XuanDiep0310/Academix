import { useMemo } from "react";
import { Layout, Menu, Button, Typography, Space } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import {
  LogOut,
  GraduationCap,
  FileText,
  HelpCircle,
  ClipboardList,
  BarChart,
} from "lucide-react";
import styles from "../../assets/styles/LayoutTeacher.module.scss";

const { Sider, Content, Header } = Layout;
const { Text, Title } = Typography;

/**
 * props:
 *  - user?: { name?: string }
 *  - onLogout: () => void
 */
export default function LayoutTeacher({ user, onLogout }) {
  const navigate = useNavigate();
  const { pathname } = useLocation();

  // key = path để navigate(e.key)
  const menuItems = useMemo(
    () => [
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

  const selectedKeys = [
    menuItems.find((i) => pathname.startsWith(i.key))?.key || menuItems[0]?.key,
  ];
  const currentLabel =
    menuItems.find((i) => pathname.startsWith(i.key))?.label ||
    "Lớp học của tôi";

  return (
    <Layout className={styles.wrap}>
      {/* SIDEBAR */}
      <Sider width={260} theme="light" className={styles.sider}>
        <div className={styles.siderHeader}>
          <Title level={4} className={styles.brand}>
            Giáo viên
          </Title>
          <Text type="secondary" className={styles.userName}>
            {user?.name || "Teacher"}
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
