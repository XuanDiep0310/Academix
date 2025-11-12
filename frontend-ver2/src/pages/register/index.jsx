import { Button, Divider, Form, Input, notification } from "antd";
import { useNavigate } from "react-router";
import "../../assets/styles/register.scss";
import { useState } from "react";
import { registerUserAPI } from "../../services/api.service";
const RegisterPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values) => {
    console.log(values.fullName);
    setLoading(true);
    const res = await registerUserAPI(
      values.fullName,
      values.email,
      values.password,
      values.phone
    );
    setLoading(false);
    if (res?.data?._id) {
      notification.success({
        message: "Đăng ký",
        description: "Đăng kí người dùng thành công",
      });
      navigate("/login");
    } else {
      notification.error({
        message: "Lỗi Đăng ký",
        description: res.message,
      });
    }
  };

  return (
    <div className="register-page">
      <Form
        // labelCol={{ span: 8 }}
        // wrapperCol={{ span: 16 }}
        style={{
          maxWidth: 800,
          margin: " 0 auto",
          background: "#fff",
          padding: " 10px 50px",
          borderRadius: "12px",
        }}
        initialValues={{ remember: true }}
        onFinish={onFinish}
        // autoComplete="off"
      >
        <h2 className="register-page__title">Đăng kí thông tin mới</h2>
        <Divider />
        <Form.Item
          label="Họ tên"
          labelCol={{ span: 24 }}
          name="fullName"
          rules={[{ required: true, message: "Vui lòng nhập họ tên!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Email"
          labelCol={{ span: 24 }}
          name="email"
          rules={[{ required: true, message: "Vui lòng nhập email!" }]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Số điện thoại"
          labelCol={{ span: 24 }}
          name="phone"
          rules={[
            {
              required: true,
              pattern: new RegExp(/\d+/g),
              message: "Vui lòng nhập Số điện thoại!",
            },
          ]}
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
            Đăng ký
          </Button>
        </Form.Item>
        <Divider />
        <p>
          Đã có tài khoản ?
          <span
            className="register-page__nav"
            onClick={() => navigate("/login")}
          >
            {" "}
            Đăng nhập
          </span>
        </p>
      </Form>
    </div>
  );
};
export default RegisterPage;
