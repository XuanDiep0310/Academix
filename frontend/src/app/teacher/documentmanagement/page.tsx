"use client";

import React, { useState } from "react";
import styles from "@/assets/styles/DocumentManagement.module.scss";
import {
  Card,
  Button,
  Tag,
  Dropdown,
  Modal,
  Form,
  Input,
  Select,
  message,
} from "antd";
import {
  Upload,
  FileText,
  Play,
  MoreVertical,
  Edit,
  Trash2,
} from "lucide-react";
import { mockDocuments, mockClasses } from "@/data/seed";

export default function DocumentManagementPage() {
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm();

  const iconFor = (t: string) =>
    t === "pdf" ? (
      <FileText className={styles.pdf} />
    ) : t === "video" ? (
      <Play className={styles.video} />
    ) : (
      <FileText />
    );

  const onUpload = async () => {
    const v = await form.validateFields();
    if (!v.title) return message.error("Nhập tiêu đề");
    message.success("Đã tải lên (mock)");
    setOpen(false);
    form.resetFields();
  };

  return (
    <div className={styles.wrap}>
      <div className={styles.head}>
        <h1>Quản lý tài liệu</h1>
        <Button
          type="primary"
          icon={<Upload size={16} />}
          onClick={() => setOpen(true)}
        >
          Tải lên tài liệu
        </Button>
      </div>

      <div className={styles.list}>
        {mockDocuments.map((doc) => {
          const cls = mockClasses.find((c) => c.id === doc.classId);
          const menu = {
            items: [
              {
                key: "1",
                icon: <Edit size={14} />,
                label: "Cập nhật phiên bản mới",
              },
              { key: "2", label: "Xem chi tiết" },
              {
                key: "3",
                icon: <Trash2 size={14} />,
                label: <span style={{ color: "#ff4d4f" }}>Xóa</span>,
              },
            ],
          };
          return (
            <Card key={doc.id} className={styles.cardItem}>
              <div className={styles.itemRow}>
                <div className={styles.iconBox}>{iconFor(doc.type)}</div>
                <div className={styles.meta}>
                  <div className={styles.title}>{doc.title}</div>
                  <div className={styles.tags}>
                    <Tag>{cls?.name}</Tag>
                    <Tag color="blue">
                      {doc.type === "pdf"
                        ? "PDF"
                        : doc.type === "video"
                        ? "Video"
                        : "Bài viết"}
                    </Tag>
                    {doc.version > 1 && (
                      <Tag color="processing">Phiên bản {doc.version}</Tag>
                    )}
                  </div>
                  <div className={styles.sub}>
                    Tải lên:{" "}
                    {new Date(doc.uploadedAt).toLocaleDateString("vi-VN")} •{" "}
                    {doc.uploadedBy}
                  </div>
                </div>
                <Dropdown menu={menu} trigger={["click"]}>
                  <Button type="text" icon={<MoreVertical size={16} />} />
                </Dropdown>
              </div>
            </Card>
          );
        })}
      </div>

      <Modal
        title="Tải lên tài liệu mới"
        open={open}
        onCancel={() => setOpen(false)}
        onOk={onUpload}
        okText="Tải lên"
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{ type: "pdf", classId: mockClasses[0]?.id }}
        >
          <Form.Item label="Tiêu đề" name="title" rules={[{ required: true }]}>
            <Input placeholder="Ví dụ: Giáo trình Đại số 12" />
          </Form.Item>
          <Form.Item label="Loại tài liệu" name="type">
            <Select
              options={[
                { value: "pdf", label: "PDF" },
                { value: "video", label: "Video" },
                { value: "article", label: "Bài viết" },
              ]}
            />
          </Form.Item>
          <Form.Item label="Lớp học" name="classId">
            <Select
              options={mockClasses.map((c) => ({ value: c.id, label: c.name }))}
            />
          </Form.Item>
          <div className={styles.dropzone}>
            <Upload size={16} /> Kéo thả tệp vào đây hoặc click để chọn
            <div className={styles.dropHint}>
              PDF, Video, hoặc tài liệu khác
            </div>
          </div>
        </Form>
      </Modal>
    </div>
  );
}
