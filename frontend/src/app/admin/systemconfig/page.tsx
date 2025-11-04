"use client";

import React from "react";
import { Card, Switch, Button } from "antd";
import styles from "@/assets/styles/SystemConfig.module.scss";

const features = [
  {
    id: "anti_cheat",
    name: "Chống gian lận nâng cao",
    desc: "Theo dõi focus, copy/paste, face detection",
    enabled: true,
  },
  {
    id: "ai_grading",
    name: "AI Chấm điểm",
    desc: "Gợi ý chấm tự động",
    enabled: true,
  },
  {
    id: "notification",
    name: "Thông báo realtime",
    desc: "WebSocket notifications",
    enabled: true,
  },
  {
    id: "2fa",
    name: "Xác thực 2 bước",
    desc: "Two-factor authentication",
    enabled: false,
  },
];

export default function SystemConfigPage() {
  return (
    <div className={styles.wrap}>
      <h1>Cấu hình Hệ thống</h1>

      <Card className={styles.card} title="Tính năng hệ thống">
        <div className={styles.list}>
          {features.map((f) => (
            <div key={f.id} className={styles.row}>
              <div className={styles.info}>
                <div className={styles.name}>{f.name}</div>
                <div className={styles.sub}>{f.desc}</div>
              </div>
              <Switch defaultChecked={f.enabled} />
            </div>
          ))}
        </div>
        <Button type="primary" className={styles.mt8}>
          Lưu cấu hình
        </Button>
      </Card>

      <div className={styles.grid2}>
        <Card className={styles.card} title="Email">
          <div className={styles.kv}>
            <span>SMTP</span>
            <b>smtp.example.com</b>
          </div>
          <div className={styles.kv}>
            <span>From</span>
            <b>noreply@eduplatform.com</b>
          </div>
          <Button className={styles.mt8}>Cấu hình</Button>
        </Card>
        <Card className={styles.card} title="Storage">
          <div className={styles.kv}>
            <span>Loại</span>
            <b>AWS S3</b>
          </div>
          <div className={styles.kv}>
            <span>Giới hạn/file</span>
            <b>500 MB</b>
          </div>
          <Button className={styles.mt8}>Cấu hình</Button>
        </Card>
      </div>
    </div>
  );
}
