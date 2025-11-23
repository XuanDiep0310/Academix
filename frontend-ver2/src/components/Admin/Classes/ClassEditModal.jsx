import { useEffect, useState } from "react";
import { Modal, Form, Input, message, notification } from "antd";
import { createClassAPI, editClassesAPI } from "../../../services/api.service";

const ClassEditModal  = (props) => {
  const { open, onCancel, onSuccess, editingClass } = props;
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open) {
      if (editingClass) {
        form.setFieldsValue({
          name: editingClass.name,
          description: editingClass.description,
        });
      } else {
        form.resetFields();
      }
    }
  }, [open, editingClass, form]);

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setLoading(true);

      if (editingClass) {
        const res = await editClassesAPI(
          editingClass.id,
          values.name,
          values.description
        );
        if (res && res.success === true) {
          message.success("Đã cập nhật lớp học thành công");
          onSuccess();
          onCancel();
        } else {
          notification.error({
            message: "Cập nhật lớp học thất bại",
            description: res.message || "Có lỗi xảy ra",
          });
        }
      } else {
        const res = await createClassAPI(
          values.name,
          values.code,
          values.description
        );
        if (res && res.success === true) {
          message.success("Đã tạo lớp học thành công");
          onSuccess();
          onCancel();
        } else {
          notification.error({
            message: "Tạo lớp học thất bại",
            description: res.message || "Có lỗi xảy ra",
          });
        }
      }
    } catch (error) {
      console.error("Submit error:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={editingClass ? "Chỉnh sửa lớp học" : "Tạo lớp học mới"}
      open={open}
      onCancel={onCancel}
      onOk={handleSubmit}
      okText={editingClass ? "Cập nhật" : "Tạo lớp"}
      confirmLoading={loading}
      maskClosable={!loading}
      destroyOnClose
    >
      <Form
        layout="vertical"
        form={form}
        disabled={loading}
        initialValues={{ name: "", code: "", description: "" }}
      >
        <Form.Item
          label="Tên lớp học"
          name="name"
          rules={[{ required: true, message: "Vui lòng nhập tên lớp" }]}
        >
          <Input placeholder="VD: Lập trình 1" />
        </Form.Item>

        {!editingClass && (
          <Form.Item
            label="Mã lớp"
            name="code"
            rules={[{ required: true, message: "Vui lòng nhập mã lớp" }]}
          >
            <Input placeholder="VD: CS101" />
          </Form.Item>
        )}

        <Form.Item label="Mô tả" name="description">
          <Input.TextArea rows={3} placeholder="Mô tả ngắn..." />
        </Form.Item>
      </Form>
    </Modal>
  );
}
export default ClassEditModal;