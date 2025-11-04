"use client";

import React from "react";
import { Card, Statistic, Tag } from "antd";
import { TrendingUp } from "lucide-react";
import styles from "@/assets/styles/RevenuePanel.module.scss";

const total = 12500000; // mock
const month = 2150000;
const byPlan = [
  { plan: "FREE", value: 0, pct: 0 },
  { plan: "BASIC", value: 1200000, pct: 9.6 },
  { plan: "PREMIUM", value: 11300000, pct: 90.4 },
];

export default function RevenuePanelPage() {
  return (
    <div className={styles.wrap}>
      <h1>Doanh thu Chi tiết</h1>

      <div className={styles.grid3}>
        <Card className={styles.card}>
          <Statistic
            title="Tổng doanh thu (YTD)"
            value={`${(total / 1e6).toFixed(1)}M VNĐ`}
            prefix={<TrendingUp size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Tháng này"
            value={`${(month / 1e6).toFixed(1)}M VNĐ`}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="TB/tháng"
            value={`${(total / 12 / 1e6).toFixed(1)}M VNĐ`}
          />
        </Card>
      </div>

      <Card className={styles.card} title="Doanh thu theo gói">
        <div className={styles.planList}>
          {byPlan.map((p) => (
            <div key={p.plan} className={styles.planRow}>
              <div className={styles.planLeft}>
                <Tag>{p.plan}</Tag>
                <span>{(p.value / 1e6).toFixed(1)}M</span>
              </div>
              <div className={styles.bar}>
                <div style={{ width: `${p.pct}%` }} />
              </div>
              <div className={styles.pct}>{p.pct.toFixed(1)}%</div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
