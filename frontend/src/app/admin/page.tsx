"use client";

import React from "react";
import { Card, Statistic, Button, List, Tag } from "antd";
import { Users, Building2, TrendingUp, DollarSign } from "lucide-react";
import styles from "@/assets/styles/AdminDashboard.module.scss";

// mock nhanh (thay bằng data thật nếu có)
const orgs = [
  { id: 1, name: "THPT A" },
  { id: 2, name: "THPT B" },
];
const users = { teachers: 45, students: 820 };
const subsActive = 7;
const revenueM = 12.5;

const recentSubs = [
  { org: "THPT A", plan: "Premium", end: "2025-12-31" },
  { org: "THPT B", plan: "Basic", end: "2025-11-30" },
];

export default function AdminDashboardPage() {
  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <h1>Quản trị Hệ thống</h1>
        <p>Tổng quan và quản lý toàn nền tảng</p>
      </div>

      <div className={styles.grid4}>
        <Card className={styles.card}>
          <Statistic
            title="Tổng doanh thu (ước tính)"
            value={`${revenueM.toFixed(1)}M VNĐ`}
            prefix={<DollarSign size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Tổ chức"
            value={orgs.length}
            prefix={<Building2 size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Người dùng"
            value={users.teachers + users.students}
            prefix={<Users size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Subscriptions hoạt động"
            value={subsActive}
            prefix={<TrendingUp size={16} />}
          />
        </Card>
      </div>

      <Card
        className={styles.card}
        title="Subscriptions gần đây"
        extra={<Button href="/admin/subscriptionmanagement">Quản lý</Button>}
      >
        <List
          dataSource={recentSubs}
          renderItem={(s) => (
            <List.Item className={styles.item}>
              <div>{s.org}</div>
              <div className={styles.right}>
                <Tag color="blue">{s.plan.toUpperCase()}</Tag>
                <span className={styles.muted}>
                  Hết hạn: {new Date(s.end).toLocaleDateString("vi-VN")}
                </span>
              </div>
            </List.Item>
          )}
        />
      </Card>

      <Card className={styles.card} title="Gợi ý thao tác nhanh">
        <div className={styles.quick}>
          <Button href="/admin/revenuepanel">Xem doanh thu</Button>
          <Button href="/admin/supporttickets" type="primary">
            Xử lý tickets
          </Button>
          <Button href="/admin/systemconfig" danger>
            Kiểm tra cấu hình
          </Button>
        </div>
      </Card>
    </div>
  );
}
