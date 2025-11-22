import  { useEffect, useState } from "react";
import { Modal, Form, Input, Spin, message, notification } from "antd";
import { createUserAPI, editUserAPI } from "../../../services/api.service";

const TeacherModal = (props) => {
  const { open, onCancel, onSuccess, editingUser } = props;
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open) {
      if (editingUser) {
        form.setFieldsValue({
          email: editingUser.email,
          name: editingUser.name,
          password: "",
        });
      } else {
        form.resetFields();
      }
    }
  }, [open, editingUser, form]);

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);

      if (editingUser) {
        const res = await editUserAPI(
          editingUser.id,
          values.name,
          values.email
        );
        if (res && res.success === true) {
          message.success("Đã cập nhật giáo viên");
          onSuccess();
          onCancel();
        } else {
          notification.error({
            message: "Error",
            description: res?.message || "Có lỗi xảy ra",
          });
        }
      } else {
        const res = await createUserAPI(
          values.name,
          values.email,
          values.password,
          "Teacher"
        );

        if (res && res.success === true) {
          message.success("Đã tạo tài khoản giáo viên");
          onSuccess();
          onCancel();
        } else {
          notification.error({
            message: "Error",
            description: res?.message || "Có lỗi xảy ra",
          });
        }
      }
    } catch (err) {
      console.error("Submit teacher error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={editingUser ? "Chỉnh sửa" : "Thêm giáo viên mới"}
      open={open}
      onCancel={onCancel}
      onOk={handleSubmit}
      okText={editingUser ? "Cập nhật" : "Tạo tài khoản"}
      confirmLoading={loading}
      destroyOnClose
      maskClosable={!loading}
    >
      <Spin spinning={loading}>
        <Form
          layout="vertical"
          form={form}
          initialValues={{ email: "", name: "", password: "" }}
        >
          <Form.Item
            label="Email"
            name="email"
            rules={[
              {
                required: true,
                type: "email",
                message: "Email không hợp lệ",
              },
            ]}
          >
            <Input
              placeholder="teacher@school.com"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item
            label="Họ và tên"
            name="name"
            rules={[{ required: true, message: "Vui lòng nhập họ tên" }]}
          >
            <Input placeholder="Nguyễn Văn A" disabled={loading} />
          </Form.Item>

          {!editingUser && (
            <Form.Item
              label="Mật khẩu"
              name="password"
              rules={[{ required: true, message: "Vui lòng nhập mật khẩu" }]}
            >
              <Input.Password
                placeholder="••••••••"
                disabled={loading}
              />
            </Form.Item>
          )}
        </Form>
      </Spin>
    </Modal>
  );
}
export default TeacherModal;