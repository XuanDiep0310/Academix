import { useMemo } from "react";
import { Layout, Typography, Space } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import { Users, GraduationCap, BarChart2 } from "lucide-react";
import styles from "../../assets/styles/LayoutAdmin.module.scss";
import SiderLayout from "../Sider/SiderLayout";

const { Content, Header } = Layout;
const { Text } = Typography;

const LayoutAdmin = () => {
  const navigate = useNavigate();
  const { pathname } = useLocation();

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
    <Layout className={styles.adminWrap}>
      <SiderLayout
        brandTitle="Quản trị hệ thống"
        userName="Admin"
        menuItems={menuItems}
        selectedKeys={selectedKeys}
        onMenuClick={(e) => navigate(e.key)}
        className={styles.sider}
        showChangePassword={true}
      />

      <Layout>
        <Header className={styles.header}>
          <Space size={8}>
            <Text type="secondary">Bạn đang ở:</Text>
            <Text strong>{currentLabel}</Text>
          </Space>
        </Header>

        <Content className={styles.content}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};

export default LayoutAdmin;
