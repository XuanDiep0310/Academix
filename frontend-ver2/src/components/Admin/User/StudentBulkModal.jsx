import { useState } from "react";
import { Modal, Input, Typography, Divider, Spin, message, notification } from "antd";
import { callBulkCreateUser } from "../../../services/api.service";

const { Text } = Typography;

const StudentBulkModal = (props) => {
  const { open, onCancel, onSuccess } = props;
  const [bulkText, setBulkText] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async () => {
    setLoading(true);
    const lines = bulkText
      .split("\n")
      .map((l) => l.trim())
      .filter(Boolean);

    const apiUsers = [];

    lines.forEach((line) => {
      const [email, fullName, password] = line.split(",").map((p) => p?.trim());
      if (email && fullName && password) {
        apiUsers.push({
          email,
          fullName,
          password,
          role: "Student",
        });
      }
    });

    if (!apiUsers.length) {
      message.error("Không có dòng hợp lệ (định dạng: email,họ tên,mật khẩu)");
      setLoading(false);
      return;
    }

    try {
      const res = await callBulkCreateUser({ users: apiUsers });
      if (res && res.success === true) {
        message.success(res.message || "Thêm học sinh thành công");
        setBulkText("");
        onSuccess();
        onCancel();
      } else {
        notification.error({
          message: "Error",
          description: res?.message || "Có lỗi xảy ra",
        });
      }
    } catch (error) {
      console.error("Bulk create error:", error);
      message.error("Có lỗi xảy ra khi tạo tài khoản");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title="Thêm học sinh hàng loạt"
      open={open}
      onCancel={onCancel}
      onOk={handleSubmit}
      okText="Thêm học sinh"
      confirmLoading={loading}
      destroyOnClose
      maskClosable={!loading}
    >
      <Spin spinning={loading}>
        <Text type="secondary">
          Nhập mỗi dòng theo định dạng: <Text code>email,họ tên,mật khẩu</Text>
        </Text>
        <Divider />
        <Input.TextArea
          rows={10}
          value={bulkText}
          onChange={(e) => setBulkText(e.target.value)}
          disabled={loading}
          placeholder={
            "student1@school.com,Nguyễn Văn A,password123\n" +
            "student2@school.com,Trần Thị B,password456\n" +
            "student3@school.com,Lê Văn C,password789"
          }
        />
      </Spin>
    </Modal>
  );
} 
export default StudentBulkModal;
