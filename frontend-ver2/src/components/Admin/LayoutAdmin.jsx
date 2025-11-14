import { useMemo } from "react";
import { Layout, Menu, Button, Typography, Space, message } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import {
  LogOut,
  Users,
  GraduationCap,
  Settings,
  BarChart2,
} from "lucide-react";
import styles from "../../assets/styles/LayoutAdmin.module.scss";
import { callLogout } from "../../services/api.service";

const { Sider, Content, Header } = Layout;
const { Text, Title } = Typography;

const LayoutAdmin = () => {
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
  // Khai báo menu DÙNG ICON lucide-react, key = PATH
  const menuItems = useMemo(
    () => [
      {
        key: "/admin",
        icon: <BarChart2 size={16} />,
        label: "Tổng quan",
      },
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
      // {
      //   key: "/admin/settings",
      //   icon: <Settings size={16} />,
      //   label: "Cài đặt",
      // },
    ],
    []
  );

  const activeItem =
    [...menuItems]
      .sort((a, b) => b.key.length - a.key.length) // key dài hơn ưu tiên trước
      .find((item) => pathname.startsWith(item.key)) || menuItems[0];

  const selectedKeys = [activeItem.key];
  const currentLabel = activeItem.label;

  return (
    <Layout className={styles.adminWrap}>
      {/* SIDEBAR */}
      <Sider width={260} theme="light" className={styles.sider}>
        <div className={styles.siderHeader}>
          <Title level={4} className={styles.brand}>
            Quản trị hệ thống
          </Title>
          <Text type="secondary" className={styles.userName}>
            Admin
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
