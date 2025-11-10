import { Button, Divider, Form, Input, message, notification } from "antd";
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
    setLoading(true);
    const res = await loginUserAPI(values.email, values.password);

    if (res?.data) {
      localStorage.setItem("access_token", res.data.access_token);
      dispatch(doLoginAction(res.data.user));
      notification.success({ message: "Đăng nhập thành công!" });
      navigate("/");
    } else {
      notification.error({
        message: "Đăng nhập lỗi!!!",
        description: res.message,
      });
    }
    setLoading(false);
  };
  return (
    <>
      <div className="login-page">
        <Form
          style={{
            maxWidth: 800,
            margin: " 0 auto",
            background: "#fff",
            padding: " 10px 50px",
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
            Chưa có tài khoản ?
            <span
              className="login-page__nav"
              onClick={() => navigate("/register")}
            >
              {" "}
              Đăng ký
            </span>
          </p>
        </Form>
      </div>
    </>
  );
};
export default LoginPage;
