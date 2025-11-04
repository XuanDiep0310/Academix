"use client";

import React, { useState } from "react";
import { Card, Badge, Button, Input } from "antd";
import styles from "@/assets/styles/SupportTickets.module.scss";

const tickets = [
  {
    id: "t1",
    subject: "Không đăng nhập được",
    user: "admin@a.edu",
    created: "2025-10-30T09:00",
    status: "open",
    priority: "high",
  },
  {
    id: "t2",
    subject: "Lỗi xuất Excel",
    user: "org@b.edu",
    created: "2025-10-31T14:20",
    status: "in-progress",
    priority: "medium",
  },
];

export default function SupportTicketsPage() {
  const [selected, setSelected] = useState<string | null>(null);
  const sel = tickets.find((t) => t.id === selected);

  return (
    <div className={styles.wrap}>
      <h1>Hỗ trợ & Khiếu nại</h1>

      <div className={styles.grid2}>
        <Card className={styles.card} title="Tất cả Tickets">
          <div className={styles.list}>
            {tickets.map((t) => (
              <div
                key={t.id}
                className={`${styles.item} ${
                  selected === t.id ? styles.active : ""
                }`}
                onClick={() => setSelected(t.id)}
              >
                <div className={styles.top}>
                  <div className={styles.badges}>
                    <Badge
                      status={
                        t.status === "open"
                          ? "error"
                          : t.status === "in-progress"
                          ? "processing"
                          : "success"
                      }
                      text={
                        t.status === "open"
                          ? "Mở"
                          : t.status === "in-progress"
                          ? "Đang xử lý"
                          : "Đã giải quyết"
                      }
                    />
                    <Badge
                      status={
                        t.priority === "high"
                          ? "error"
                          : t.priority === "medium"
                          ? "warning"
                          : "default"
                      }
                      text={`Ưu tiên ${t.priority}`}
                    />
                  </div>
                </div>
                <div className={styles.title}>{t.subject}</div>
                <div className={styles.sub}>
                  {t.user} • {new Date(t.created).toLocaleString("vi-VN")}
                </div>
              </div>
            ))}
          </div>
        </Card>

        <Card className={styles.card} title="Chi tiết Ticket">
          {sel ? (
            <div className={styles.detail}>
              <div className={styles.kv}>
                <span>Tiêu đề:</span>
                <b>{sel.subject}</b>
              </div>
              <div className={styles.kv}>
                <span>Người gửi:</span>
                <span>{sel.user}</span>
              </div>
              <div className={styles.kv}>
                <span>Thời gian:</span>
                <span>{new Date(sel.created).toLocaleString("vi-VN")}</span>
              </div>
              <Input.TextArea rows={5} placeholder="Nhập phản hồi..." />
              <div className={styles.actions}>
                <Button type="primary">Gửi & Đánh dấu đã giải quyết</Button>
                <Button>Gửi phản hồi</Button>
              </div>
            </div>
          ) : (
            <div className={styles.empty}>Chọn một ticket để xem chi tiết</div>
          )}
        </Card>
      </div>
    </div>
  );
}
