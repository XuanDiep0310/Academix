"use client";

import React, { useMemo, useState } from "react";
import {
  Card,
  Button,
  Table,
  Input,
  Modal,
  Form,
  Avatar,
  Dropdown,
  Tag,
  message,
} from "antd";
import type { MenuProps } from "antd";
import { Search, UserPlus, Mail, MoreVertical } from "lucide-react";
import styles from "@/assets/styles/TeacherManagement.module.scss";

// mock
const teachers = [
  { id: "t1", name: "Nguyễn Văn A", email: "a@school.edu", subject: "Toán" },
  { id: "t2", name: "Trần Thị B", email: "b@school.edu", subject: "Vật lý" },
  { id: "t3", name: "Lê Văn C", email: "c@school.edu", subject: "Hóa học" },
];
const classByTeacher: Record<string, { classes: number; students: number }> = {
  t1: { classes: 3, students: 96 },
  t2: { classes: 2, students: 64 },
  t3: { classes: 2, students: 60 },
};

export default function TeacherManagementPage() {
  const [q, setQ] = useState("");
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm();

  const data = useMemo(
    () =>
      teachers.filter((t) =>
        (t.name + t.email).toLowerCase().includes(q.toLowerCase())
      ),
    [q]
  );

  const columns = [
    {
      title: "Giáo viên",
      dataIndex: "name",
      render: (_: any, row: any) => (
        <div className={styles.flex}>
          <Avatar>
            {row.name
              .split(" ")
              .map((n: string) => n[0])
              .join("")
              .slice(0, 2)}
          </Avatar>
          <span>{row.name}</span>
        </div>
      ),
    },
    {
      title: "Email",
      dataIndex: "email",
      render: (v: string) => (
        <span className={styles.muted}>
          <Mail size={14} /> {v}
        </span>
      ),
    },
    {
      title: "Môn dạy",
      dataIndex: "subject",
      render: (v: string) => <Tag>{v}</Tag>,
    },
    {
      title: "Lớp học",
      dataIndex: "id",
      render: (id: string) => classByTeacher[id]?.classes ?? 0,
      width: 110,
    },
    {
      title: "Học sinh",
      dataIndex: "id",
      render: (id: string) => classByTeacher[id]?.students ?? 0,
      width: 110,
    },
    {
      title: "Thao tác",
      key: "act",
      align: "right" as const,
      render: () => {
        const items: MenuProps["items"] = [
          { key: "1", label: "Xem chi tiết" },
          { key: "2", label: "Gửi email" },
          { key: "3", label: "Chỉnh sửa" },
          {
            key: "4",
            label: <span style={{ color: "#ff4d4f" }}>Vô hiệu hóa</span>,
          },
        ];
        return (
          <Dropdown menu={{ items }} trigger={["click"]}>
            <Button type="text" icon={<MoreVertical size={16} />} />
          </Dropdown>
        );
      },
      width: 120,
    },
  ];

  const onAdd = async () => {
    try {
      await form.validateFields();
      message.success("Đã thêm giáo viên (mock)");
      setOpen(false);
      form.resetFields();
    } catch {}
  };

  return (
    <div className={styles.wrap}>
      <div className={styles.head}>
        <h1>Quản lý Giáo viên</h1>
        <Button
          type="primary"
          icon={<UserPlus size={16} />}
          onClick={() => setOpen(true)}
        >
          Thêm giáo viên
        </Button>
      </div>

      <Card className={styles.card}>
        <div className={styles.search}>
          <Search size={16} />
          <Input
            placeholder="Tìm kiếm theo tên hoặc email…"
            value={q}
            onChange={(e) => setQ(e.target.value)}
          />
        </div>
      </Card>

      <Card className={styles.card}>
        <Table
          rowKey="id"
          columns={columns as any}
          dataSource={data}
          pagination={{ pageSize: 8 }}
        />
      </Card>

      <Modal
        title="Thêm giáo viên mới"
        open={open}
        onCancel={() => setOpen(false)}
        onOk={onAdd}
        okText="Thêm"
      >
        <Form form={form} layout="vertical">
          <Form.Item name="name" label="Họ và tên" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item
            name="email"
            label="Email"
            rules={[{ required: true, type: "email" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="subject"
            label="Môn dạy"
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
