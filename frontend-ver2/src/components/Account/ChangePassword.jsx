import { Button, Col, Form, Input, message, notification, Row } from "antd";
import { useForm } from "antd/es/form/Form";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { callOnChangePassWord } from "../../services/api.service";

const ChangePassword = () => {
  const userData = useSelector((state) => state.account.user);
  const [isSubmit, setIsSubmit] = useState(false);
  const [form] = useForm();

  useEffect(() => {
    form.setFieldsValue(userData);
  }, [userData]);

  const onFinish = async (values) => {
    setIsSubmit(true);
    const { email, oldpass, newpass } = values;
    const _id = userData?.id;
    const res = await callOnChangePassWord(email, oldpass, newpass);
    if (res && res?.data) {
      message.success("Thay đổi mật khẩu thành công!");
      form.setFieldValue("oldpass", "");
      form.setFieldValue("newpass", "");
    } else {
      notification.error({
        message: "Đã có lỗi xảy ra!",
        description: res.message,
      });
    }
    setIsSubmit(false);
  };
  return (
    <>
      <Form
        form={form}
        style={{
          //   maxWidth: 800,
          margin: " 0 auto",
          background: "#fff",
          borderRadius: "12px",
        }}
        onFinish={onFinish}
        layout="vertical"
        autoComplete="off"
      >
        <Row>
          <Col span={14}>
            <Row gutter={[10, 10]}>
              <Col span={24}>
                <Form.Item
                  label="Email"
                  labelCol={{ span: 24 }}
                  name="email"
                  rules={[{ required: true, message: "Vui lòng nhập Email!" }]}
                >
                  <Input disabled={true} />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Form.Item
                  label="Mật khẩu hiện tại"
                  labelCol={{ span: 24 }}
                  name="oldpass"
                  rules={[
                    {
                      required: true,
                      message: "Vui lòng nhập mật khẩu hiện tại!",
                    },
                  ]}
                >
                  <Input.Password placeholder="Nhập mật khẩu hiện tại" />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Form.Item
                  label="Mật khẩu Mới"
                  labelCol={{ span: 24 }}
                  name="newpass"
                  rules={[
                    {
                      required: true,
                      message: "Vui lòng nhập mật khẩu mới!",
                    },
                  ]}
                >
                  <Input.Password placeholder="Nhập mật khẩu mới" />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Button onClick={() => form.submit()} loading={isSubmit}>
                  Xác nhận
                </Button>
              </Col>
            </Row>
          </Col>
        </Row>
      </Form>
    </>
  );
};
export default ChangePassword;
