import { useState } from "react";
import {
  Layout,
  Menu,
  Button,
  Typography,
  Modal,
  Form,
  Input,
  message,
} from "antd";
import { LogOut, Key } from "lucide-react";
import { callLogout, callOnChangePassWord } from "../../services/api.service";
import styles from "../../assets/styles/SiderLayout.module.scss";
const { Sider } = Layout;
const { Text, Title } = Typography;

/**
 * CommonSidebar - Component Sidebar chung cho Admin, Teacher, Student
 *
 * @param {Object} props
 * @param {string} props.brandTitle - Tiêu đề hiển thị (VD: "Quản trị hệ thống", "Giáo viên", "Học sinh")
 * @param {string} props.userName - Tên người dùng hiển thị
 * @param {Array} props.menuItems - Danh sách menu items { key, icon, label }
 * @param {Array} props.selectedKeys - Keys được chọn
 * @param {Function} props.onMenuClick - Callback khi click menu item
 * @param {string} props.className - Class name cho sider
 * @param {boolean} props.showChangePassword - Hiển thị nút đổi mật khẩu (mặc định: true)
 */ const SiderLayout = ({
  brandTitle = "Hệ thống",
  userName = "User",
  menuItems = [],
  selectedKeys = [],
  onMenuClick = () => {},
  className = "",
  showChangePassword = true,
}) => {
  const [form] = Form.useForm();
  const [isChangePasswordModalOpen, setIsChangePasswordModalOpen] =
    useState(false);
  const [isChangingPassword, setIsChangingPassword] = useState(false);

  const handleLogout = async () => {
    try {
      const res = await callLogout();
      if (res.success === true) {
        localStorage.removeItem("access_token");
        if (typeof cookieStore !== "undefined") {
          cookieStore.delete("refresh_token");
        }
        window.location.href = "/login";
        message.success("Đăng xuất thành công!");
      }
    } catch (error) {
      console.error("Logout error:", error);
      message.error("Đăng xuất thất bại!");
    }
  };

  const handleOpenChangePassword = () => {
    setIsChangePasswordModalOpen(true);
  };

  const handleCancelChangePassword = () => {
    setIsChangePasswordModalOpen(false);
    form.resetFields();
  };

  const handleChangePassword = async (values) => {
    try {
      setIsChangingPassword(true);

      const res = await callOnChangePassWord(
        values.oldPassword,
        values.newPassword,
        values.confirmPassword
      );

      if (res && res.success === true) {
        message.success("Đổi mật khẩu thành công!");
        setIsChangePasswordModalOpen(false);
        form.resetFields();
      } else {
        message.error(res?.message || "Đổi mật khẩu thất bại!");
      }
    } catch (error) {
      console.error("Change password error:", error);
      message.error("Có lỗi xảy ra khi đổi mật khẩu!");
    } finally {
      setIsChangingPassword(false);
    }
  };

  return (
    <>
      {/* Sider (Container Flexbox chính) */}
      <Sider
        width={260}
        theme="light"
        className={`${className} ${styles.sidebar}`} // Áp dụng class Flexbox
      >
        {/* 1. Header (Cố định - flex-shrink: 0) */}
        <div className={styles.sidebarHeader}>
          <Title
            level={4}
            style={{ margin: 0, marginBottom: 4, color: "#0050b3" }}
          >
            {brandTitle}
          </Title>
          <Text type="secondary">{userName}</Text>
        </div>

        {/* 2. Menu Wrapper (Phần cuộn - flex-grow: 1, overflow: auto) */}
        <div className={styles.sidebarMenuWrapper}>
          <Menu
            mode="inline"
            selectedKeys={selectedKeys}
            onClick={onMenuClick}
            items={menuItems}
            style={{ borderRight: 0 }}
          />
        </div>

        {/* 3. Footer Actions (Cố định - flex-shrink: 0) */}
        <div className={styles.sidebarFooter}>
          {showChangePassword && (
            <Button
              block
              onClick={handleOpenChangePassword}
              icon={<Key size={16} />}
              style={{ marginBottom: 8 }}
            >
              Đổi mật khẩu
            </Button>
          )}
          <Button block onClick={handleLogout} icon={<LogOut size={16} />}>
            Đăng xuất
          </Button>
        </div>
      </Sider>

      {/* Modal đổi mật khẩu */}
      <Modal
        title="Đổi mật khẩu"
        open={isChangePasswordModalOpen}
        onCancel={handleCancelChangePassword}
        footer={null}
        width={480}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleChangePassword}
          autoComplete="off"
        >
          <Form.Item
            label="Mật khẩu hiện tại"
            name="oldPassword"
            rules={[
              { required: true, message: "Vui lòng nhập mật khẩu hiện tại!" },
            ]}
          >
            <Input.Password placeholder="Nhập mật khẩu hiện tại" />
          </Form.Item>

          <Form.Item
            label="Mật khẩu mới"
            name="newPassword"
            rules={[
              { required: true, message: "Vui lòng nhập mật khẩu mới!" },
              { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
              {
                pattern:
                  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]/,
                message:
                  "Mật khẩu phải chứa chữ hoa, chữ thường, số và ký tự đặc biệt!",
              },
            ]}
          >
            <Input.Password placeholder="Nhập mật khẩu mới" />
          </Form.Item>

          <Form.Item
            label="Xác nhận mật khẩu mới"
            name="confirmPassword"
            dependencies={["newPassword"]}
            rules={[
              { required: true, message: "Vui lòng xác nhận mật khẩu mới!" },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("newPassword") === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(
                    new Error("Mật khẩu xác nhận không khớp!")
                  );
                },
              }),
            ]}
          >
            <Input.Password placeholder="Nhập lại mật khẩu mới" />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0, marginTop: 24 }}>
            <div
              style={{ display: "flex", gap: 8, justifyContent: "flex-end" }}
            >
              <Button onClick={handleCancelChangePassword}>Hủy</Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={isChangingPassword}
              >
                Đổi mật khẩu
              </Button>
            </div>
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
};

export default SiderLayout;
