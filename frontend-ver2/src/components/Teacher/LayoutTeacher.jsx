import { useMemo } from "react";
import { Layout, Typography, Space } from "antd";
import { useLocation, useNavigate, Outlet } from "react-router";
import {
  GraduationCap,
  FileText,
  HelpCircle,
  ClipboardList,
  BarChart,
  BarChart2,
} from "lucide-react";
import styles from "../../assets/styles/LayoutTeacher.module.scss";
import SiderLayout from "../Sider/SiderLayout";

const { Content, Header } = Layout;
const { Text } = Typography;

export default function LayoutTeacher() {
  const navigate = useNavigate();
  const { pathname } = useLocation();

  const menuItems = useMemo(
    () => [
      {
        key: "/teacher",
        icon: <BarChart2 size={16} />,
        label: "Tổng quan",
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
      <SiderLayout
        brandTitle="Giáo viên"
        userName="Teacher"
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
}
