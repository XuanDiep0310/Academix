"use client";

import React from "react";
import { Card, Button, Table, Tag, Statistic } from "antd";
import { Download, FileText } from "lucide-react";
import styles from "@/assets/styles/FinancePanel.module.scss";

// mock
const payments = [
  {
    id: "p1",
    date: "2025-01-01",
    amount: 5_000_000,
    status: "completed",
    invoice: "INV-0001",
  },
  {
    id: "p2",
    date: "2025-06-01",
    amount: 5_000_000,
    status: "pending",
    invoice: null,
  },
  {
    id: "p3",
    date: "2025-07-15",
    amount: 5_000_000,
    status: "failed",
    invoice: null,
  },
];

export default function FinancePanelPage() {
  const totalPaid = payments
    .filter((p) => p.status === "completed")
    .reduce((s, p) => s + p.amount, 0);

  const columns = [
    {
      title: "Ngày thanh toán",
      dataIndex: "date",
      render: (v: string) => new Date(v).toLocaleDateString("vi-VN"),
    },
    { title: "Mô tả", render: () => "Thanh toán gói dịch vụ Premium" },
    {
      title: "Số hóa đơn",
      dataIndex: "invoice",
      render: (v: string | null) =>
        v ? (
          <span className={styles.flex}>
            <FileText size={14} /> {v}
          </span>
        ) : (
          <span className={styles.muted}>Chưa có</span>
        ),
    },
    {
      title: "Số tiền",
      dataIndex: "amount",
      render: (v: number) => `${v.toLocaleString("vi-VN")} VNĐ`,
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      render: (v: string) => (
        <Tag
          color={v === "completed" ? "green" : v === "pending" ? "gold" : "red"}
        >
          {v === "completed"
            ? "Đã thanh toán"
            : v === "pending"
            ? "Đang xử lý"
            : "Thất bại"}
        </Tag>
      ),
    },
    {
      title: "Thao tác",
      key: "act",
      align: "right" as const,
      render: (_: any, row: any) =>
        row.invoice ? <Button icon={<Download size={16} />} /> : null,
    },
  ];

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <h1>Quản lý Tài chính</h1>
        <p>Lịch sử thanh toán và hóa đơn</p>
      </div>

      <div className={styles.grid3}>
        <Card className={styles.card}>
          <Statistic
            title="Tổng đã thanh toán (2025)"
            value={`${totalPaid.toLocaleString("vi-VN")} VNĐ`}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic title="Thanh toán kế tiếp" value="5,000,000 VNĐ" />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Phương thức"
            value="Chuyển khoản • Gia hạn tự động"
          />
        </Card>
      </div>

      <Card
        className={styles.card}
        title="Lịch sử thanh toán"
        extra={<Button icon={<Download size={16} />}>Xuất báo cáo</Button>}
      >
        <Table rowKey="id" columns={columns as any} dataSource={payments} />
      </Card>

      <Card className={styles.card} title="Thông tin xuất hóa đơn">
        <div className={styles.grid2}>
          <div>
            <div className={styles.muted}>Tên tổ chức</div>
            <div>Trường THPT Nguyễn Huệ</div>
          </div>
          <div>
            <div className={styles.muted}>Mã số thuế</div>
            <div>0123456789</div>
          </div>
          <div>
            <div className={styles.muted}>Địa chỉ</div>
            <div>123 Đường ABC, Quận 1, TP. HCM</div>
          </div>
          <div>
            <div className={styles.muted}>Email nhận hóa đơn</div>
            <div>org@test.com</div>
          </div>
        </div>
        <Button className={styles.mt8}>Cập nhật thông tin</Button>
      </Card>
    </div>
  );
}
