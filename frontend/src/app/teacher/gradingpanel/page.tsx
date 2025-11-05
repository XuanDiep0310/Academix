"use client";

import React, { useMemo, useState } from "react";
import styles from "@/assets/styles/GradingPanel.module.scss";
import {
  Card,
  Select,
  Alert,
  List,
  Tag,
  InputNumber,
  Input,
  Button,
  Space,
  Typography,
} from "antd";
import { ShieldAlert, Eye, Copy, CheckCircle, Sparkles } from "lucide-react";
import { mockExams, mockStudents, mockExamResults } from "@/data/seed";

const { Paragraph } = Typography;

export default function GradingPanelPage() {
  const exams = mockExams.filter(
    (e) => e.type === "essay" || e.type === "mixed"
  );
  const [examId, setExamId] = useState<string>(exams[0]?.id ?? "");
  const [scores, setScores] = useState<Record<string, number>>({});
  const [notes, setNotes] = useState<Record<string, string>>({});
  const [sortByViolation, setSortByViolation] = useState(false);

  const subs = useMemo(() => {
    if (!examId) return [];
    return mockExamResults
      .filter((r) => r.examId === examId)
      .map((r) => {
        const s = mockStudents.find((x) => x.id === r.studentId);
        const e = mockExams.find((x) => x.id === examId);
        return {
          studentId: r.studentId,
          studentName: s?.name ?? "Unknown",
          examTitle: e?.title ?? "",
          focusLossCount: r.focusLossCount,
          copyPasteCount: r.copyPasteCount,
          answer: "Định lý Lagrange phát biểu rằng ... (mock answer)",
          submittedAt: new Date(r.completedAt).toLocaleString("vi-VN"),
          currentScore: r.score,
          totalPoints: e?.totalPoints ?? 100,
        };
      });
  }, [examId]);

  const sorted = sortByViolation
    ? [...subs].sort(
        (a, b) =>
          b.focusLossCount +
          b.copyPasteCount -
          (a.focusLossCount + a.copyPasteCount)
      )
    : subs;

  const highCount = subs.filter(
    (s) => s.focusLossCount > 10 || s.copyPasteCount > 5
  ).length;

  const aiSuggest = (sid: string) => {
    const suggest = Math.floor(Math.random() * 3) + 8; // 8-10
    setScores((prev) => ({ ...prev, [sid]: suggest }));
    setNotes((prev) => ({
      ...prev,
      [sid]:
        "AI gợi ý: Bài làm đầy đủ, trình bày rõ ràng. Nên bổ sung ví dụ minh họa.",
    }));
  };

  return (
    <div className={styles.wrap}>
      <div>
        <h1>Chấm điểm</h1>
        <p className={styles.muted}>
          Chấm bài tự luận và ghi nhận điểm cho học sinh
        </p>
      </div>

      <Card className={styles.card} title="Chọn bài kiểm tra">
        <Select
          value={examId}
          onChange={setExamId}
          options={[
            { value: "", label: "-- Chọn bài kiểm tra --" },
            ...exams.map((e) => ({
              value: e.id,
              label: `${e.title} - ${e.className}`,
            })),
          ]}
          style={{ width: "100%" }}
        />
      </Card>

      {!!examId && (
        <>
          <Card className={styles.card}>
            <div className={styles.grid5}>
              <div>
                <span className={styles.muted}>Bài kiểm tra</span>
                <div>{mockExams.find((e) => e.id === examId)?.title}</div>
              </div>
              <div>
                <span className={styles.muted}>Lớp</span>
                <div>{mockExams.find((e) => e.id === examId)?.className}</div>
              </div>
              <div>
                <span className={styles.muted}>Số bài nộp</span>
                <div>{subs.length}</div>
              </div>
              <div>
                <span className={styles.muted}>Đã chấm</span>
                <div>
                  {Object.keys(scores).length}/{subs.length}
                </div>
              </div>
              <div>
                <span className={styles.muted}>⚠️ Vi phạm cao</span>
                <div className={styles.danger}>{highCount}</div>
              </div>
            </div>
          </Card>

          {highCount > 0 && (
            <Alert
              className={styles.alert}
              type="error"
              showIcon
              message={
                <span>
                  <ShieldAlert size={16} /> Phát hiện {highCount} học sinh có
                  dấu hiệu vi phạm nghiêm trọng (mất focus &gt; 10 hoặc
                  copy/paste &gt; 5 lần).
                </span>
              }
            />
          )}

          <div className={styles.listHead}>
            <span className={styles.muted}>
              Hiển thị {sorted.length} bài nộp
            </span>
            <Button onClick={() => setSortByViolation((v) => !v)}>
              {sortByViolation ? "✓ " : ""}Sắp xếp theo vi phạm
            </Button>
          </div>

          <List
            dataSource={sorted}
            renderItem={(s) => {
              const high = s.focusLossCount > 10 || s.copyPasteCount > 5;
              return (
                <Card
                  className={`${styles.subCard} ${
                    high ? styles.subCardDanger : ""
                  }`}
                >
                  <div className={styles.subHeader}>
                    <div className={styles.subTitle}>
                      <b>{s.studentName}</b>
                      {high && <Tag color="red">VI PHẠM CAO</Tag>}
                    </div>
                    <div className={styles.badges}>
                      {!!s.focusLossCount && (
                        <Tag
                          icon={<Eye size={14} />}
                          color={s.focusLossCount > 10 ? "red" : "default"}
                        >
                          {s.focusLossCount} lần mất focus
                        </Tag>
                      )}
                      {!!s.copyPasteCount && (
                        <Tag
                          icon={<Copy size={14} />}
                          color={s.copyPasteCount > 5 ? "red" : "default"}
                        >
                          {s.copyPasteCount} lần copy
                        </Tag>
                      )}
                      {scores[s.studentId] !== undefined && (
                        <Tag
                          icon={<CheckCircle size={14} />}
                          color="processing"
                        >
                          Đã chấm
                        </Tag>
                      )}
                    </div>
                  </div>

                  <div className={styles.answer}>
                    <span className={styles.muted}>Bài làm:</span>
                    <Paragraph>{s.answer}</Paragraph>
                  </div>

                  <div className={styles.grid2}>
                    <div className={styles.scoreBox}>
                      <span className={styles.muted}>
                        Điểm (/{s.totalPoints})
                      </span>
                      <Space.Compact style={{ width: "100%" }}>
                        <InputNumber
                          min={0}
                          max={s.totalPoints}
                          value={scores[s.studentId]}
                          onChange={(v) =>
                            setScores((prev) => ({
                              ...prev,
                              [s.studentId]: Number(v),
                            }))
                          }
                          style={{ width: "100%" }}
                        />
                        <Button
                          icon={<Sparkles size={16} />}
                          onClick={() => aiSuggest(s.studentId)}
                        >
                          AI gợi ý
                        </Button>
                      </Space.Compact>
                    </div>
                    <div>
                      <span className={styles.muted}>Nhận xét</span>
                      <Input.TextArea
                        rows={3}
                        value={notes[s.studentId]}
                        onChange={(e) =>
                          setNotes((prev) => ({
                            ...prev,
                            [s.studentId]: e.target.value,
                          }))
                        }
                      />
                    </div>
                  </div>
                </Card>
              );
            }}
          />

          <div className={styles.actionsRight}>
            <Button> Lưu nháp </Button>
            <Button type="primary"> Lưu và công bố điểm </Button>
          </div>
        </>
      )}
    </div>
  );
}
