"use client";
import React, { useMemo, useState } from "react";
import styles from "@/assets/styles/StudentManagement.module.scss";
import {
  Card,
  Input,
  Select,
  Button,
  Modal,
  Form,
  Table,
  Tag,
  Dropdown,
  MenuProps,
  Avatar,
  Space,
  message,
} from "antd";
import { UserPlus, FileDown, Mail, MoreVertical } from "lucide-react";
import { mockStudents, mockClasses, mockExamResults } from "@/data/seed";

export default function StudentManagementPage() {
  const [q, setQ] = useState("");
  const [cls, setCls] = useState<"all" | string>("all");
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm();

  const data = useMemo(() => {
    return mockStudents.filter((s) => {
      const hit = (s.name + s.email).toLowerCase().includes(q.toLowerCase());
      const ok = cls === "all" || s.classId === cls;
      return hit && ok;
    });
  }, [q, cls]);

  const classMap = useMemo(
    () => Object.fromEntries(mockClasses.map((c) => [c.id, c.name])),
    []
  );
  const stats = (studentId: string) => {
    const results = mockExamResults.filter((r) => r.studentId === studentId);
    const avg = results.length
      ? results.reduce((a, r) => a + (r.score / r.totalPoints) * 100, 0) /
        results.length
      : 0;
    return { exams: results.length, avg };
  };

  const columns = [
    {
      title: "Học sinh",
      dataIndex: "name",
      key: "name",
      render: (_: any, row: any) => (
        <Space>
          <Avatar>
            {row.name
              .split(" ")
              .map((n: string) => n[0])
              .join("")
              .slice(0, 2)}
          </Avatar>
          <span>{row.name}</span>
        </Space>
      ),
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      render: (v: string) => (
        <span className={styles.muted}>
          <Mail size={14} /> {v}
        </span>
      ),
    },
    {
      title: "Lớp",
      dataIndex: "classId",
      key: "classId",
      render: (v: string) => <Tag>{classMap[v]}</Tag>,
    },
    {
      title: "Bài kiểm tra",
      key: "exams",
      render: (_: any, row: any) => stats(row.id).exams,
      width: 120,
    },
    {
      title: "Điểm TB",
      key: "avg",
      render: (_: any, row: any) => {
        const avg = stats(row.id).avg;
        if (!avg) return <span className={styles.muted}>Chưa có</span>;
        const color = avg >= 80 ? "blue" : avg >= 65 ? "gold" : "red";
        return <Tag color={color}>{avg.toFixed(1)}</Tag>;
      },
      width: 120,
    },
    {
      title: "Thao tác",
      key: "act",
      align: "right" as const,
      render: () => {
        const items: MenuProps["items"] = [
          { key: "1", label: "Xem chi tiết" },
          { key: "2", label: "Gửi email" },
          {
            key: "3",
            label: <span style={{ color: "#ff4d4f" }}>Xóa khỏi lớp</span>,
          },
        ];
        return (
          <Dropdown menu={{ items }}>
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
      message.success("Đã thêm học sinh (mock)");
      setOpen(false);
      form.resetFields();
    } catch {}
  };

  return (
    <div className={styles.wrap}>
      <div className={styles.head}>
        <h1>Quản lý học sinh</h1>
        <div className={styles.actions}>
          <Button
            icon={<FileDown size={16} />}
            onClick={() => message.info("Xuất Excel (mock)")}
          >
            Xuất Excel
          </Button>
          <Button
            type="primary"
            icon={<UserPlus size={16} />}
            onClick={() => setOpen(true)}
          >
            Thêm học sinh
          </Button>
        </div>
      </div>

      <Card className={styles.card}>
        <div className={styles.filters}>
          <Input
            placeholder="Tìm theo tên hoặc email…"
            value={q}
            onChange={(e) => setQ(e.target.value)}
          />
          <Select
            value={cls}
            onChange={setCls}
            options={[
              { value: "all", label: "Tất cả lớp" },
              ...mockClasses.map((c) => ({ value: c.id, label: c.name })),
            ]}
            style={{ minWidth: 220 }}
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
        title="Thêm học sinh mới"
        open={open}
        onCancel={() => setOpen(false)}
        onOk={onAdd}
        okText="Thêm"
      >
        <Form form={form} layout="vertical">
          <Form.Item label="Họ và tên" name="name" rules={[{ required: true }]}>
            <Input placeholder="Nguyễn Văn A" />
          </Form.Item>
          <Form.Item
            label="Email"
            name="email"
            rules={[{ required: true, type: "email" }]}
          >
            <Input placeholder="student@example.com" />
          </Form.Item>
          <Form.Item
            label="Lớp"
            name="classId"
            initialValue={mockClasses[0]?.id}
          >
            <Select
              options={mockClasses.map((c) => ({ value: c.id, label: c.name }))}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
