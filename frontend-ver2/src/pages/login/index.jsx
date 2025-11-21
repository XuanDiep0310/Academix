// src/pages/login/index.jsx
import { Button, Divider, Form, Input, notification } from "antd";
import { useNavigate } from "react-router";
import "../../assets/styles/login.scss";
import { useState } from "react";
import { loginUserAPI } from "../../services/api.service";
import { useDispatch } from "react-redux";
import { doLoginAction } from "../../redux/account/accountSlice";

const LoginPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const dispatch = useDispatch();

  const onFinish = async (values) => {
    try {
      setLoading(true);
      const res = await loginUserAPI(values.email, values.password);

      if (res?.success === true) {
        // lưu token
        localStorage.setItem("access_token", res.data.accessToken);
        cookieStore.set("refresh_token", res.data.refreshToken);

        // lưu user vào redux
        dispatch(doLoginAction(res.data.user));

        notification.success({ message: "Đăng nhập thành công!" });

        const role = res?.data?.user?.role;
        let redirectPath = "/";

        switch (role) {
          case "Admin":
            redirectPath = "/admin";
            break;
          case "Teacher":
            redirectPath = "/teacher";
            break;
          case "Student":
            redirectPath = "/student";
            break;
          default:
            redirectPath = "/";
            break;
        }
        navigate(redirectPath);
      } else {
        notification.error({
          message: "Đăng nhập lỗi!!!",
          description: res?.message || "Sai email hoặc mật khẩu.",
        });
      }
    } catch (error) {
      notification.error({
        message: "Đăng nhập lỗi!!!",
        description:
          error?.response?.data?.message ||
          error?.message ||
          "Có lỗi xảy ra, vui lòng thử lại.",
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
        initialValues={{ remember: true }}
        onFinish={onFinish}
      >
        <h2 className="login-page__title">Đăng nhập</h2>
        <Divider />
        <Form.Item
          label="Email"
          labelCol={{ span: 24 }}
          name="email"
          rules={[{ required: true, message: "Vui lòng nhập email!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Mật khẩu"
          labelCol={{ span: 24 }}
          name="password"
          rules={[{ required: true, message: "Vui lòng nhập mật khẩu!" }]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item label={null}>
          <Button type="primary" htmlType="submit" loading={loading}>
            Đăng nhập
          </Button>
        </Form.Item>

        <Divider />

        <p>
          Quên mật khẩu ?
          <span
            className="login-page__nav"
            onClick={() => navigate("/forgot-password")}
          >
            Lấy lại mật khẩu
          </span>
        </p>
      </Form>
    </div>
  );
};

export default LoginPage;
