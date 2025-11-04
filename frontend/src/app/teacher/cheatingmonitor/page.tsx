"use client";

import React, { useState } from "react";
import styles from "@/assets/styles/CheatingMonitor.module.scss";
import {
  Card,
  Statistic,
  Segmented,
  Table,
  Tag,
  Progress,
  Row,
  Col,
} from "antd";
import { AlertTriangle, TrendingUp, Eye, Copy, Download } from "lucide-react";
import {
  mockCheatingAlerts,
  mockExamResults,
  mockStudents,
  mockExams,
} from "@/data/seed";

type Sev = "all" | "low" | "medium" | "high" | "critical";

export default function CheatingMonitorPage() {
  const [sev, setSev] = useState<Sev>("all");

  const highRisk = mockCheatingAlerts
    .map((a) => {
      const attempt = mockExamResults.find((r) => r.id === a.attemptId);
      const s = mockStudents.find((x) => x.id === a.studentId);
      const e = mockExams.find((x) => x.id === a.examId);
      if (!attempt || !s || !e) return null;
      return {
        studentName: s.name,
        examTitle: e.title,
        focusLossCount: attempt.focusLossCount,
        copyPasteCount: attempt.copyPasteCount,
        severity: a.severity,
      };
    })
    .filter(Boolean) as Array<{
    studentName: string;
    examTitle: string;
    focusLossCount: number;
    copyPasteCount: number;
    severity: Exclude<Sev, "all">;
  }>;

  const filtered =
    sev === "all" ? highRisk : highRisk.filter((x) => x.severity === sev);
  const order = { critical: 4, high: 3, medium: 2, low: 1 } as const;
  const sorted = [...filtered].sort(
    (a, b) =>
      order[b.severity] - order[a.severity] ||
      b.focusLossCount +
        b.copyPasteCount -
        (a.focusLossCount + a.copyPasteCount)
  );
  const top5 = sorted.slice(0, 5);

  const total = highRisk.length;
  const crt = highRisk.filter((x) => x.severity === "critical").length;
  const hi = highRisk.filter((x) => x.severity === "high").length;
  const avgFocus = total
    ? Math.round(highRisk.reduce((s, x) => s + x.focusLossCount, 0) / total)
    : 0;
  const avgCopy = total
    ? Math.round(highRisk.reduce((s, x) => s + x.copyPasteCount, 0) / total)
    : 0;

  const columns = [
    {
      title: "STT",
      render: (_: any, __: any, i: number) => (
        <span className={styles.muted}>#{i + 1}</span>
      ),
      width: 80,
    },
    { title: "Tên học sinh", dataIndex: "studentName" },
    { title: "Bài kiểm tra", dataIndex: "examTitle" },
    {
      title: "Mất focus",
      dataIndex: "focusLossCount",
      align: "center" as const,
      render: (v: number) => <Tag icon={<Eye size={14} />}>{v}</Tag>,
    },
    {
      title: "Copy/Paste",
      dataIndex: "copyPasteCount",
      align: "center" as const,
      render: (v: number) => <Tag icon={<Copy size={14} />}>{v}</Tag>,
    },
    {
      title: "Mức độ",
      dataIndex: "severity",
      align: "center" as const,
      render: (v: string) => {
        const color =
          v === "critical"
            ? "red"
            : v === "high"
            ? "orange"
            : v === "medium"
            ? "gold"
            : "blue";
        const label =
          v === "critical"
            ? "Cực kỳ nguy hiểm"
            : v === "high"
            ? "Cao"
            : v === "medium"
            ? "Trung bình"
            : "Thấp";
        return <Tag color={color}>{label}</Tag>;
      },
    },
  ];

  const pct = (n: number) => (total ? Math.round((100 * n) / total) : 0);

  return (
    <div className={styles.wrap}>
      <div>
        <h1>Theo dõi Gian lận</h1>
        <p className={styles.muted}>
          Giám sát và phân tích hành vi gian lận của học sinh trong các bài kiểm
          tra
        </p>
      </div>

      <div className={styles.grid4}>
        <Card className={styles.card}>
          <Statistic
            title="Tổng cảnh báo"
            value={total}
            prefix={<AlertTriangle size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="Nguy hiểm cao"
            value={crt + hi}
            prefix={<TrendingUp size={16} />}
            valueStyle={{ color: "#cf1322" }}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="TB mất focus"
            value={avgFocus}
            prefix={<Eye size={16} />}
          />
        </Card>
        <Card className={styles.card}>
          <Statistic
            title="TB Copy/Paste"
            value={avgCopy}
            prefix={<Copy size={16} />}
          />
        </Card>
      </div>

      <Card
        className={styles.card}
        title="Top 5 học sinh có dấu hiệu gian lận"
        extra={<Tag icon={<Download size={14} />}>Xuất báo cáo</Tag>}
      >
        <div className={styles.filterLine}>
          <span className={styles.muted}>Lọc theo mức độ:</span>
          <Segmented
            value={sev}
            onChange={(v) => setSev(v as Sev)}
            options={[
              { label: "Tất cả", value: "all" },
              { label: "Cực cao", value: "critical" },
              { label: "Cao", value: "high" },
              { label: "Trung bình", value: "medium" },
              { label: "Thấp", value: "low" },
            ]}
          />
        </div>

        <Table
          rowKey={(r) => r.studentName + r.examTitle}
          columns={columns as any}
          dataSource={top5}
          pagination={false}
        />
      </Card>

      <Row gutter={[12, 12]}>
        <Col xs={24} md={12}>
          <Card className={styles.card} title="Phân bố theo mức độ">
            <div className={styles.dist}>
              <div>
                <span className={styles.dotRed}></span> Cực kỳ nguy hiểm{" "}
                <Tag>{crt}</Tag>
              </div>
              <div>
                <span className={styles.dotOrange}></span> Cao <Tag>{hi}</Tag>
              </div>
              <div>
                <span className={styles.dotGold}></span> Trung bình{" "}
                <Tag>
                  {highRisk.filter((s) => s.severity === "medium").length}
                </Tag>
              </div>
              <div>
                <span className={styles.dotBlue}></span> Thấp{" "}
                <Tag>{highRisk.filter((s) => s.severity === "low").length}</Tag>
              </div>
            </div>
          </Card>
        </Col>
        <Col xs={24} md={12}>
          <Card className={styles.card} title="Hành vi phổ biến">
            <div className={styles.pbRow}>
              <div className={styles.pbLabel}>Chuyển tab/cửa sổ</div>
              <div className={styles.pbValue}>
                {highRisk.filter((s) => s.focusLossCount > 10).length} học sinh
              </div>
              <Progress
                percent={pct(
                  highRisk.filter((s) => s.focusLossCount > 10).length
                )}
                showInfo={false}
              />
            </div>
            <div className={styles.pbRow}>
              <div className={styles.pbLabel}>Copy/Paste nhiều lần</div>
              <div className={styles.pbValue}>
                {highRisk.filter((s) => s.copyPasteCount > 5).length} học sinh
              </div>
              <Progress
                percent={pct(
                  highRisk.filter((s) => s.copyPasteCount > 5).length
                )}
                showInfo={false}
              />
            </div>
            <div className={styles.pbRow}>
              <div className={styles.pbLabel}>Cả hai hành vi</div>
              <div className={styles.pbValue}>
                {
                  highRisk.filter(
                    (s) => s.focusLossCount > 10 && s.copyPasteCount > 5
                  ).length
                }{" "}
                học sinh
              </div>
              <Progress
                percent={pct(
                  highRisk.filter(
                    (s) => s.focusLossCount > 10 && s.copyPasteCount > 5
                  ).length
                )}
                showInfo={false}
                status="exception"
              />
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  );
}
