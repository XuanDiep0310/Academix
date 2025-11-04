"use client";

import React from "react";
import { Card, Table, Tag, Button } from "antd";
import styles from "@/assets/styles/SubscriptionManagement.module.scss";

const subs = [
  {
    id: "s1",
    org: "THPT A",
    plan: "premium",
    users: "180/200",
    price: 5000000,
    end: "2025-12-31",
    status: "active",
  },
  {
    id: "s2",
    org: "THPT B",
    plan: "basic",
    users: "75/100",
    price: 1200000,
    end: "2025-11-30",
    status: "active",
  },
];

export default function SubscriptionManagementPage() {
  const columns = [
    { title: "Tổ chức", dataIndex: "org" },
    {
      title: "Gói",
      dataIndex: "plan",
      render: (v: string) => <Tag color="blue">{v.toUpperCase()}</Tag>,
    },
    { title: "Người dùng", dataIndex: "users" },
    {
      title: "Giá",
      dataIndex: "price",
      render: (v: number) => `${v.toLocaleString("vi-VN")} VNĐ`,
    },
    {
      title: "Hết hạn",
      dataIndex: "end",
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      render: (v: string) => (
        <Tag color={v === "active" ? "green" : "red"}>
          {v === "active" ? "Hoạt động" : "Hết hạn"}
        </Tag>
      ),
    },
    {
      title: "Thao tác",
      key: "act",
      align: "right" as const,
      render: () => <Button type="link">Chi tiết</Button>,
    },
  ];
  return (
    <div className={styles.wrap}>
      <h1>Quản lý Subscriptions</h1>
      <Card className={styles.card} title="Tất cả gói">
        <Table rowKey="id" columns={columns as any} dataSource={subs} />
      </Card>
    </div>
  );
}
