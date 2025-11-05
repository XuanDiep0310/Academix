"use client";

import React, { useMemo } from "react";
import { Card, Statistic, Progress, List, Tag, Button } from "antd";
import {
  Users,
  GraduationCap,
  Package,
  BarChart3,
  TrendingUp,
} from "lucide-react";
import styles from "@/assets/styles/OrganizationDashboard.module.scss";

// Mock tối thiểu (bạn có thể thay bằng data thật)
const teachers = [
  { id: "t1", org: "org1" },
  { id: "t2", org: "org1" },
  { id: "t3", org: "org1" },
];
const students = Array.from({ length: 320 }).map((_, i) => ({
  id: `s${i + 1}`,
}));
const classes = [{ id: "c1" }, { id: "c2" }, { id: "c3" }, { id: "c4" }];
const subscription = {
  plan: "Premium",
  price: 5_000_000,
  startDate: "2025-01-01",
  endDate: "2025-12-31",
  currentUsers: 180,
  maxUsers: 200,
  status: "active" as "active" | "expired",
};
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
];

export default function OrganizationDashboardPage() {
  const totalTeachers = teachers.filter((t) => t.org === "org1").length;
  const totalStudents = students.length;
  const activeClasses = classes.length;
  const usage = useMemo(
    () => Math.round((subscription.currentUsers * 100) / subscription.maxUsers),
    []
  );

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <h1>Tổng quan Tổ chức</h1>
        <p>Trường THPT Nguyễn Huệ</p>
      </div>

      <div className={styles.grid4}>
        <Card className={styles.card}>
          <Statistic
            title="Giáo viên"
            value={totalTeachers}
            prefix={<Users size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Học sinh"
            value={totalStudents}
            prefix={<GraduationCap size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Lớp học"
            value={activeClasses}
            prefix={<BarChart3 size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Gói dịch vụ"
            value={subscription.plan.toUpperCase()}
            prefix={<Package size={16} />}
          />
          <div className={styles.sub}>
            {subscription.currentUsers}/{subscription.maxUsers} users
          </div>
        </Card>
      </div>

      <Card className={styles.card} title="Sử dụng License">
        <div className={styles.usage}>
          <div className={styles.usageRow}>
            <span>Người dùng hiện tại</span>
            <span>
              {subscription.currentUsers}/{subscription.maxUsers}
            </span>
          </div>
          <Progress percent={usage} />
          <div className={styles.note}>
            {usage >= 90 ? (
              <span className={styles.warn}>
                ⚠️ Sắp đạt giới hạn! Hãy nâng cấp gói.
              </span>
            ) : (
              <>
                Còn <b>{subscription.maxUsers - subscription.currentUsers}</b>{" "}
                slot trống
              </>
            )}
          </div>
        </div>
        <div className={styles.kpis}>
          <div>
            <span>Giáo viên</span>
            <b>{totalTeachers}</b>
          </div>
          <div>
            <span>Học sinh</span>
            <b>{totalStudents}</b>
          </div>
          <div>
            <span>Quản trị</span>
            <b>2</b>
          </div>
        </div>
      </Card>

      <div className={styles.grid2}>
        <Card
          className={styles.card}
          title="Thông tin gói dịch vụ"
          extra={
            <Button href="/organization/subscriptionpanel">Chi tiết</Button>
          }
        >
          <div className={styles.kv}>
            <span>Gói hiện tại:</span>
            <Tag color="blue">{subscription.plan.toUpperCase()}</Tag>
          </div>
          <div className={styles.kv}>
            <span>Giá:</span>
            <b>{subscription.price.toLocaleString("vi-VN")} VNĐ/năm</b>
          </div>
          <div className={styles.kv}>
            <span>Ngày bắt đầu:</span>
            <span>
              {new Date(subscription.startDate).toLocaleDateString("vi-VN")}
            </span>
          </div>
          <div className={styles.kv}>
            <span>Ngày hết hạn:</span>
            <span>
              {new Date(subscription.endDate).toLocaleDateString("vi-VN")}
            </span>
          </div>
          <div className={styles.kv}>
            <span>Trạng thái:</span>
            <Tag color={subscription.status === "active" ? "green" : "red"}>
              {subscription.status === "active" ? "Đang hoạt động" : "Hết hạn"}
            </Tag>
          </div>
        </Card>

        <Card
          className={styles.card}
          title="Lịch sử thanh toán"
          extra={<Button href="/organization/financepanel">Xem tất cả</Button>}
        >
          <List
            dataSource={payments}
            renderItem={(p) => (
              <List.Item className={styles.payItem}>
                <div>
                  <div className={styles.bold}>
                    {p.invoice || "Thanh toán gói dịch vụ"}
                  </div>
                  <div className={styles.muted}>
                    {new Date(p.date).toLocaleDateString("vi-VN")}
                  </div>
                </div>
                <div className={styles.right}>
                  <div>{p.amount.toLocaleString("vi-VN")} VNĐ</div>
                  <Tag color={p.status === "completed" ? "green" : "orange"}>
                    {p.status === "completed" ? "Đã thanh toán" : "Đang xử lý"}
                  </Tag>
                </div>
              </List.Item>
            )}
          />
        </Card>

        <Card
          className={`${styles.card} ${styles.full}`}
          title="Báo cáo hoạt động"
        >
          <div className={styles.activity}>
            <div>
              <div className={styles.big}>{activeClasses}</div>
              <div className={styles.muted}>Lớp đang hoạt động</div>
            </div>
            <div>
              <div className={styles.big}>{totalTeachers}</div>
              <div className={styles.muted}>Giáo viên hoạt động</div>
            </div>
            <div>
              <div className={styles.big}>{totalStudents}</div>
              <div className={styles.muted}>Học sinh tham gia</div>
            </div>
            <div>
              <div
                className={styles.big}
                style={{ display: "flex", alignItems: "center", gap: 4 }}
              >
                85 <TrendingUp size={18} />
              </div>
              <div className={styles.muted}>Điểm TB hệ thống</div>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
