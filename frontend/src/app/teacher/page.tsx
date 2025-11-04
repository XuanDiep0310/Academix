"use client";

import React from "react";
import { Card, Statistic, Badge, Button, List, Tag } from "antd";
import {
  BookOpen,
  Users,
  FileText,
  TrendingUp,
  AlertTriangle,
} from "lucide-react";
import styles from "@/assets/styles/TeacherDashboard.module.scss";
import {
  mockClasses,
  mockExams,
  mockExamResults,
  mockDocuments,
  mockStudents,
} from "@/data/seed";

export default function TeacherDashboardPage() {
  const myClasses = mockClasses.filter((c) => c.teacherId === "t1");
  const totalStudents = myClasses.reduce((s, c) => s + c.studentCount, 0);
  const activeExams = mockExams.filter((e) => e.status === "ongoing").length;
  const pendingGrading = mockExams.filter(
    (e) => e.type === "essay" || e.type === "mixed"
  ).length;
  const highRisk = mockExamResults.filter(
    (r) => r.focusLossCount > 5 || r.copyPasteCount > 0
  );

  const recentExams = mockExams.slice(0, 3);
  const classStats = myClasses.map((cls) => ({
    ...cls,
    examCount: mockExams.filter((e) => e.classId === cls.id).length,
    docCount: mockDocuments.filter((d) => d.classId === cls.id).length,
  }));

  return (
    <div className={styles.wrap}>
      <div className={styles.header}>
        <h1>Xin chào, Giáo viên!</h1>
        <p>Quản lý lớp học và học sinh của bạn</p>
      </div>

      {/* Stats */}
      <div className={styles.grid4}>
        <Card className={styles.cardStat}>
          <Statistic
            title="Tổng số lớp"
            value={myClasses.length}
            prefix={<BookOpen size={16} />}
          />
        </Card>
        <Card className={styles.cardStat}>
          <Statistic
            title="Tổng học sinh"
            value={totalStudents}
            prefix={<Users size={16} />}
          />
        </Card>
        <Card className={styles.cardStat}>
          <Statistic
            title="Bài kiểm tra đang diễn ra"
            value={activeExams}
            prefix={<FileText size={16} />}
          />
        </Card>
        <Card className={styles.cardStat}>
          <Statistic
            title="Chờ chấm điểm"
            value={pendingGrading}
            prefix={<TrendingUp size={16} />}
          />
        </Card>
      </div>

      {/* Anti-cheat warning */}
      {highRisk.length > 0 && (
        <Card className={styles.alertCard}>
          <div className={styles.alertHeader}>
            <div className={styles.alertTitle}>
              <AlertTriangle size={18} />
              <span>Cảnh báo gian lận</span>
              <Badge
                count={highRisk.length}
                style={{ backgroundColor: "#fa541c" }}
              />
            </div>
            <Button href="/(user)/teacher/cheatingmonitor" ghost>
              Xem chi tiết
            </Button>
          </div>

          <List
            dataSource={highRisk.slice(0, 3)}
            renderItem={(r) => {
              const s = mockStudents.find((x) => x.id === r.studentId);
              const e = mockExams.find((x) => x.id === r.examId);
              return (
                <List.Item className={styles.alertItem}>
                  <div>
                    <div className={styles.bold}>{s?.name}</div>
                    <div className={styles.muted}>{e?.title}</div>
                  </div>
                  <div className={styles.badges}>
                    <Tag color="red">{r.focusLossCount} lần mất focus</Tag>
                    {r.copyPasteCount > 0 && (
                      <Tag color="red">{r.copyPasteCount} lần copy</Tag>
                    )}
                  </div>
                </List.Item>
              );
            }}
          />
        </Card>
      )}

      {/* Recent exams */}
      <Card
        className={styles.cardBlock}
        title="Bài kiểm tra gần đây"
        extra={<Button href="/(user)/teacher/examcreation">Tạo mới</Button>}
      >
        <List
          dataSource={recentExams}
          renderItem={(exam) => (
            <List.Item className={styles.rowItem}>
              <div>
                <div className={styles.bold}>{exam.title}</div>
                <div className={styles.muted}>{exam.className}</div>
              </div>
              <Tag
                color={
                  exam.status === "ongoing"
                    ? "blue"
                    : exam.status === "completed"
                    ? "default"
                    : "green"
                }
              >
                {exam.status === "ongoing"
                  ? "Đang diễn ra"
                  : exam.status === "completed"
                  ? "Đã kết thúc"
                  : "Sắp diễn ra"}
              </Tag>
            </List.Item>
          )}
        />
      </Card>

      {/* Class statistics */}
      <Card className={styles.cardBlock} title="Thống kê lớp học">
        <div className={styles.classGrid}>
          {classStats.map((cls) => (
            <div key={cls.id} className={styles.classBox}>
              <div className={styles.bold}>{cls.name}</div>
              <div className={styles.kv}>
                <span>Học sinh</span>
                <strong>{cls.studentCount}</strong>
              </div>
              <div className={styles.kv}>
                <span>Bài kiểm tra</span>
                <strong>{cls.examCount}</strong>
              </div>
              <div className={styles.kv}>
                <span>Tài liệu</span>
                <strong>{cls.docCount}</strong>
              </div>
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
