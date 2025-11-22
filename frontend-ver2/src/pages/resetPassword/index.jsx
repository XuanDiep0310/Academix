import { Button, Divider, Form, Input, notification } from "antd";
import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router";
import "../../assets/styles/login.scss";
import { resetPasswordAPI } from "../../services/api.service";

const ResetPasswordPage = () => {
  const [loading, setLoading] = useState(false);
  const [tokenValid, setTokenValid] = useState(true);
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const token = searchParams.get("token");

  useEffect(() => {
    if (!token) {
      setTokenValid(false);
    }
  }, [token]);

  const onFinish = async (values) => {
    if (!token) {
      notification.error({
        message: "Token không hợp lệ",
        description: "Đường link đặt lại mật khẩu không đúng hoặc đã hết hạn.",
      });
      return;
    }

    try {
      setLoading(true);
      const res = await resetPasswordAPI(
        token,
        values.newPassword,
        values.confirmPassword
      );
      console.log(res);

      if (res?.success === true) {
        notification.success({
          message: "Đặt lại mật khẩu thành công",
          description:
            res?.data?.message || res?.message || "Vui lòng đăng nhập lại.",
        });
        navigate("/login");
      } else {
        notification.error({
          message: "Đặt lại mật khẩu thất bại",
          description:
            res?.data?.message ||
            res?.message ||
            "Vui lòng thử lại hoặc yêu cầu link mới.",
        });
      }
    } catch (error) {
      notification.error({
        message: "Lỗi hệ thống",
        description:
          error?.response?.data?.message ||
          error?.message ||
          "Không thể đặt lại mật khẩu. Vui lòng thử lại.",
      });
    } finally {
      setLoading(false);
    }
  };

  if (!tokenValid) {
    return (
      <div className="login-page">
        <div
          style={{
            maxWidth: 800,
            margin: "0 auto",
            background: "#fff",
            padding: "10px 50px",
            borderRadius: 12,
          }}
        >
          <h2 className="login-page__title">Link không hợp lệ</h2>
          <Divider />
          <p>
            Đường link đặt lại mật khẩu không đúng hoặc đã hết hạn. Vui lòng yêu
            cầu{" "}
            <span
              className="login-page__nav"
              onClick={() => navigate("/forgot-password")}
            >
              gửi lại link mới
            </span>
            .
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="login-page">
      <Form
        style={{
          maxWidth: 800,
          margin: "0 auto",
          background: "#fff",
          padding: "10px 50px",
          borderRadius: 12,
        }}
        onFinish={onFinish}
      >
        <h2 className="login-page__title">Đặt lại mật khẩu</h2>
        <Divider />

        <Form.Item
          label="Mật khẩu mới"
          labelCol={{ span: 24 }}
          name="newPassword"
          rules={[
            { required: true, message: "Vui lòng nhập mật khẩu mới!" },
            { min: 6, message: "Mật khẩu phải có ít nhất 6 ký tự!" },
          ]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item
          label="Xác nhận mật khẩu"
          labelCol={{ span: 24 }}
          name="confirmPassword"
          dependencies={["newPassword"]}
          rules={[
            { required: true, message: "Vui lòng xác nhận mật khẩu!" },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue("newPassword") === value) {
                  return Promise.resolve();
                }
                return Promise.reject("Mật khẩu xác nhận không khớp!");
              },
            }),
          ]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item label={null}>
          <Button type="primary" htmlType="submit" loading={loading}>
            Đặt lại mật khẩu
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

export default ResetPasswordPage;
