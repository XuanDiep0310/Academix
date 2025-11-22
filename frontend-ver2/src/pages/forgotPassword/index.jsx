// src/pages/ForgotPasswordPage.jsx
import { Button, Divider, Form, Input, notification } from "antd";
import { useState } from "react";
import { useNavigate } from "react-router";
import "../../assets/styles/login.scss";
import { forgotPasswordAPI } from "../../services/api.service";

const ForgotPasswordPage = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const onFinish = async (values) => {
    try {
      setLoading(true);
      const res = await forgotPasswordAPI(values.email);

      if (res?.success === true) {
        notification.success({
          message: "Gửi email thành công",
          description:
            res?.message ||
            res?.data ||
            "Link đặt lại mật khẩu đã được gửi vào email của bạn.",
        });
        // quay lại màn hình login
        navigate("/login");
      } else {
        notification.error({
          message: "Gửi email thất bại",
          description:
            res?.message || res?.data || "Có lỗi xảy ra, vui lòng thử lại sau.",
        });
      }
    } catch (error) {
      notification.error({
        message: "Lỗi hệ thống",
        description:
          error?.response?.data?.message ||
          error?.message ||
          "Không thể gửi email, vui lòng thử lại.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <Form
        style={{
          maxWidth: 800,
          margin: "0 auto",
          background: "#fff",
          padding: "10px 50px",
          borderRadius: "12px",
        }}
        onFinish={onFinish}
      >
        <h2 className="login-page__title">Quên mật khẩu</h2>
        <Divider />
        <Form.Item
          label="Email"
          labelCol={{ span: 24 }}
          name="email"
          rules={[
            { required: true, message: "Vui lòng nhập email!" },
            { type: "email", message: "Email không hợp lệ!" },
          ]}
        >
          <Input placeholder="Nhập email đã đăng ký" />
        </Form.Item>

        <Form.Item label={null}>
          <Button type="primary" htmlType="submit" loading={loading}>
            Gửi link đặt lại mật khẩu
          </Button>
        </Form.Item>

        <Divider />
        <p>
          <span className="login-page__nav" onClick={() => navigate("/login")}>
            &lt; Quay lại đăng nhập
          </span>
        </p>
      </Form>
    </div>
  );
};

export default ForgotPasswordPage;
