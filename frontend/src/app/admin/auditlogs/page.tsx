"use client";

import React from "react";
import { Card, Table, Tag } from "antd";
import styles from "@/assets/styles/AuditLogs.module.scss";

const logs = [
  {
    id: "1",
    time: "2025-11-01T14:30:00",
    user: "admin@test.com",
    role: "admin",
    action: "UPDATE_SYSTEM_CONFIG",
    desc: "Bật AI Grading cho gói Premium",
    status: "success",
  },
  {
    id: "2",
    time: "2025-11-01T10:15:00",
    user: "org@test.com",
    role: "organization",
    action: "UPGRADE_SUBSCRIPTION",
    desc: "Nâng Basic → Premium",
    status: "success",
  },
  {
    id: "5",
    time: "2025-10-30T11:00:00",
    user: "admin@test.com",
    role: "admin",
    action: "SUSPEND_ORGANIZATION",
    desc: "Tạm ngưng tổ chức THPT Trần Phú",
    status: "warning",
  },
];

export default function AuditLogsPage() {
  const columns = [
    {
      title: "Thời gian",
      dataIndex: "time",
      render: (v: string) => new Date(v).toLocaleString("vi-VN"),
    },
    { title: "Người dùng", dataIndex: "user" },
    {
      title: "Vai trò",
      dataIndex: "role",
      render: (v: string) => <Tag>{v}</Tag>,
    },
    {
      title: "Hành động",
      dataIndex: "action",
      render: (v: string) => <code>{v}</code>,
    },
    { title: "Mô tả", dataIndex: "desc" },
    {
      title: "Trạng thái",
      dataIndex: "status",
      render: (v: string) => (
        <Tag color={v === "success" ? "green" : "orange"}>
          {v === "success" ? "Thành công" : "Cảnh báo"}
        </Tag>
      ),
    },
  ];
  return (
    <div className={styles.wrap}>
      <h1>Audit Logs</h1>
      <Card className={styles.card}>
        <Table rowKey="id" columns={columns as any} dataSource={logs} />
      </Card>
    </div>
  );
}
