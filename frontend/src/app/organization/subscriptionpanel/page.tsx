"use client";

import React from "react";
import { Card, Button, Tag } from "antd";
import { Check, Star } from "lucide-react";
import styles from "@/assets/styles/SubscriptionPanel.module.scss";

// mock
const current = {
  plan: "premium",
  price: 5_000_000,
  startDate: "2025-01-01",
  endDate: "2025-12-31",
  currentUsers: 180,
  maxUsers: 200,
};

const plans = [
  {
    name: "Free",
    price: 0,
    maxUsers: 50,
    features: [
      "Tối đa 50 người dùng",
      "Lưu trữ 5GB",
      "Tính năng cơ bản",
      "Hỗ trợ email",
    ],
  },
  {
    name: "Basic",
    price: 1_200_000,
    maxUsers: 100,
    features: [
      "Tối đa 100 người dùng",
      "Lưu trữ 20GB",
      "Tính năng nâng cao",
      "Chống gian lận cơ bản",
      "Hỗ trợ ưu tiên",
    ],
  },
  {
    name: "Premium",
    price: 5_000_000,
    maxUsers: 500,
    features: [
      "Tối đa 500 người dùng",
      "Lưu trữ 100GB",
      "Tất cả tính năng",
      "Chống gian lận nâng cao",
      "AI chấm điểm",
      "Báo cáo chi tiết",
      "Hỗ trợ 24/7",
    ],
    recommended: true,
  },
  {
    name: "Enterprise",
    price: 0,
    maxUsers: 9999,
    features: [
      "Không giới hạn người dùng",
      "Lưu trữ không giới hạn",
      "Tùy chỉnh theo yêu cầu",
      "Dedicated support",
      "SLA đảm bảo",
      "Training & onboarding",
    ],
    custom: true,
  },
];

export default function SubscriptionPanelPage() {
  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <h1>Gói dịch vụ</h1>
        <p>Chọn gói phù hợp với tổ chức</p>
      </div>

      <Card className={styles.card} title="Gói hiện tại">
        <div className={styles.kv}>
          <span>Gói:</span>
          <Tag color="blue">{current.plan.toUpperCase()}</Tag>
        </div>
        <div className={styles.kv}>
          <span>Người dùng:</span>
          <span>
            {current.currentUsers}/{current.maxUsers}
          </span>
        </div>
        <div className={styles.kv}>
          <span>Giá:</span>
          <b>{current.price.toLocaleString("vi-VN")} VNĐ / năm</b>
        </div>
        <div className={styles.kv}>
          <span>Bắt đầu:</span>
          <span>{new Date(current.startDate).toLocaleDateString("vi-VN")}</span>
        </div>
        <div className={styles.kv}>
          <span>Hết hạn:</span>
          <span>{new Date(current.endDate).toLocaleDateString("vi-VN")}</span>
        </div>
        <Button type="primary" className={styles.mt8}>
          Gia hạn gói dịch vụ
        </Button>
      </Card>

      <h2 className={styles.h2}>Các gói dịch vụ có sẵn</h2>
      <div className={styles.planGrid}>
        {plans.map((p) => {
          const isCurrent = p.name.toLowerCase() === current.plan;
          return (
            <Card
              key={p.name}
              className={`${styles.planCard} ${
                p.recommended ? styles.reco : ""
              } ${isCurrent ? styles.current : ""}`}
            >
              {p.recommended && (
                <Tag className={styles.tagTop}>
                  <Star size={12} /> Phổ biến nhất
                </Tag>
              )}
              {isCurrent && <Tag className={styles.tagTopRight}>Đang dùng</Tag>}
              <div className={styles.planHead}>
                <div className={styles.planName}>{p.name}</div>
                <div className={styles.planPrice}>
                  {p.custom
                    ? "Liên hệ"
                    : `${p.price.toLocaleString("vi-VN")} VNĐ/năm`}
                </div>
              </div>
              <div className={styles.featureList}>
                {p.features.map((f, i) => (
                  <div key={i} className={styles.feature}>
                    <Check size={14} /> {f}
                  </div>
                ))}
              </div>
              <Button disabled={isCurrent} className={styles.mt8}>
                {p.custom
                  ? "Liên hệ Sales"
                  : isCurrent
                  ? "Đang sử dụng"
                  : "Nâng cấp"}
              </Button>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
