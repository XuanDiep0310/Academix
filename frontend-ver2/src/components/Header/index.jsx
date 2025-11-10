import { Layout, Button, Dropdown, Space, Typography } from "antd";
import {
  BookOutlined,
  UserOutlined,
  LogoutOutlined,
  FormOutlined,
  CheckSquareOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons";
const { Header } = Layout;
const { Text } = Typography;
export default function AppHeader({
  userName,
  currentRole,
  onRoleChange,
  onLogout,
  currentPage,
  onNavigate,
}) {
  const teacherMenu = [
    {
      key: "questions",
      label: "Quản lý câu hỏi",
      icon: <UnorderedListOutlined />,
    },
    { key: "create-test", label: "Tạo bài kiểm tra", icon: <FormOutlined /> },
    { key: "grading", label: "Chấm bài", icon: <CheckSquareOutlined /> },
  ];

  const studentMenu = [
    {
      key: "test-list",
      label: "Danh sách bài kiểm tra",
      icon: <UnorderedListOutlined />,
    },
  ];

  const items = (currentRole === "teacher" ? teacherMenu : studentMenu).map(
    (i) => ({
      key: i.key,
      label: (
        <Button
          type={currentPage === i.key ? "primary" : "text"}
          icon={i.icon}
          onClick={() => onNavigate(i.key)}
        >
          {i.label}
        </Button>
      ),
    })
  );

  const roleMenu = {
    items: [
      {
        key: "teacher",
        label: "Chuyển sang Giáo viên",
        onClick: () => onRoleChange("teacher"),
      },
      {
        key: "student",
        label: "Chuyển sang Học sinh",
        onClick: () => onRoleChange("student"),
      },
    ],
  };

  return (
    <Header style={{ background: "#fff", borderBottom: "1px solid #e5e7eb" }}>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
        }}
      >
        <Space size={12}>
          <div
            className="flex-center"
            style={{
              width: 40,
              height: 40,
              background: "#1677ff",
              borderRadius: 8,
            }}
          >
            <BookOutlined style={{ color: "#fff", fontSize: 20 }} />
          </div>
          <div>
            <Text strong style={{ color: "#0f172a" }}>
              Hệ thống kiểm tra trực tuyến
            </Text>
            <div style={{ color: "#1677ff", fontSize: 12 }}>
              {currentRole === "teacher" ? "Giáo viên" : "Học sinh"}
            </div>
          </div>
        </Space>

        <Space size={8}>
          {items.map((i) => (
            <span key={i.key}>{i.label}</span>
          ))}

          <Dropdown menu={roleMenu} placement="bottomRight" trigger={["click"]}>
            <Button type="text" icon={<UserOutlined />}>
              {userName}
            </Button>
          </Dropdown>

          <Button type="text" icon={<LogoutOutlined />} onClick={onLogout}>
            Đăng xuất
          </Button>
        </Space>
      </div>
    </Header>
  );
}
