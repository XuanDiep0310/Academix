import { useMemo } from "react";
import { Layout, Menu, Button, Typography, Space } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import { LogOut, Users, GraduationCap, Settings } from "lucide-react";
import styles from "../../assets/styles/LayoutAdmin.module.scss";

const { Sider, Content, Header } = Layout;
const { Text, Title } = Typography;

/**
 * props:
 *  - user?: { name?: string }
 *  - onLogout: () => void
 */
const LayoutAdmin = ({ user, onLogout }) => {
  const navigate = useNavigate();
  const { pathname } = useLocation();

  // Khai báo menu DÙNG ICON lucide-react, key = PATH
  const menuItems = useMemo(
    () => [
      {
        key: "/admin/users",
        icon: <Users size={16} />,
        label: "Quản lý tài khoản",
      },
      {
        key: "/admin/classes",
        icon: <GraduationCap size={16} />,
        label: "Quản lý lớp học",
      },
      {
        key: "/admin/settings",
        icon: <Settings size={16} />,
        label: "Cài đặt",
      },
    ],
    []
  );

  // Xác định mục đang chọn theo URL hiện tại
  const selectedKeys = [
    menuItems.find((i) => pathname.startsWith(i.key))?.key || menuItems[0]?.key,
  ];

  // Header breadcrumb ngắn theo path
  const currentLabel =
    menuItems.find((i) => pathname.startsWith(i.key))?.label ||
    "Quản lý tài khoản";

  return (
    <Layout className={styles.adminWrap}>
      {/* SIDEBAR */}
      <Sider width={260} theme="light" className={styles.sider}>
        <div className={styles.siderHeader}>
          <Title level={4} className={styles.brand}>
            Quản trị hệ thống
          </Title>
          <Text type="secondary" className={styles.userName}>
            {user?.name || "Admin"}
          </Text>
        </div>

        <Menu
          mode="inline"
          selectedKeys={selectedKeys}
          onClick={(e) => navigate(e.key)} // điều hướng theo key (là path)
          items={menuItems}
          className={styles.menu}
        />

        <div className={styles.siderFooter}>
          {/* Nếu có ChangePassword thì để ở đây */}
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
          {/* Mọi màn hình con hiển thị qua Outlet */}
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};
export default LayoutAdmin;
