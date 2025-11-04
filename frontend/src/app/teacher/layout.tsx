"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Layout, Menu, Badge } from "antd";
import type { MenuProps } from "antd";
import {
  Home,
  GraduationCap,
  ClipboardList,
  Database,
  FileText,
  FolderOpen,
  Shield,
} from "lucide-react";
import styles from "@/assets/styles/layoutTeacher.module.scss";
import { mockClasses, mockExams, mockExamResults } from "@/data/seed";

const { Sider, Content } = Layout;

export default function TeacherLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname() || "";

  // stats cho badge
  const pendingGrading = mockExams.filter(
    (e) => e.type === "essay" || e.type === "mixed"
  ).length;
  const highRiskCount = mockExamResults.filter(
    (r) => r.focusLossCount > 5 || r.copyPasteCount > 0
  ).length;

  // Helper: label có badge ở bên phải
  const withBadgeRight = (
    href: string,
    text: string,
    count?: number,
    color?: string
  ) => (
    <Link href={href} className={styles.menuLabel}>
      <span>{text}</span>
      {typeof count === "number" && count > 0 && (
        <Badge count={count} color={color} />
      )}
    </Link>
  );

  const items: MenuProps["items"] = [
    {
      key: "/teacher",
      icon: <Home size={18} />,
      label: <Link href="/teacher">Trang chủ</Link>,
    },
    {
      key: "/teacher/studentmanagement",
      icon: <GraduationCap size={18} />,
      label: <Link href="/teacher/studentmanagement">Quản lý học sinh</Link>,
    },
    {
      key: "/teacher/examcreation",
      icon: <ClipboardList size={18} />,
      label: <Link href="/teacher/examcreation">Tạo bài kiểm tra</Link>,
    },
    {
      key: "/teacher/questionbank",
      icon: <Database size={18} />,
      label: <Link href="/teacher/questionbank">Ngân hàng câu hỏi</Link>,
    },
    {
      key: "/teacher/gradingpanel",
      icon: <FileText size={18} />,
      // Badge để ở label (bên phải)
      label: withBadgeRight(
        "/teacher/gradingpanel",
        "Chấm điểm",
        pendingGrading
      ),
    },
    {
      key: "/teacher/documentmanagement",
      icon: <FolderOpen size={18} />,
      label: <Link href="/teacher/documentmanagement">Tài liệu</Link>,
    },
    {
      key: "/teacher/cheatingmonitor",
      icon: <Shield size={18} />,
      // Badge để ở label (bên phải)
      label: withBadgeRight(
        "/teacher/cheatingmonitor",
        "Theo dõi gian lận",
        highRiskCount,
        "red"
      ),
    },
  ];

  // Chọn key khớp tiền tố dài nhất với pathname (ổn cho mọi cấp /teacher/xxx[/...])
  const allKeys = (items ?? []).map((i) => String(i?.key));
  const selectedKey =
    allKeys.reduce((best, key) => {
      if (pathname.startsWith(key) && key.length > best.length) return key;
      return best;
    }, "") || "/teacher"; // fallback

  return (
    <Layout className={styles.shell}>
      <Sider
        width={260}
        className={styles.sider}
        breakpoint="lg"
        collapsedWidth={0}
      >
        <div className={styles.brand}>
          <span className={styles.brandDot} /> Teacher
        </div>

        <Menu
          mode="inline"
          selectedKeys={[selectedKey]}
          items={items}
          className={styles.menu}
        />

        <div className={styles.myClasses}>
          <h4>Lớp học của bạn</h4>
          <ul>
            {mockClasses
              .filter((c) => c.teacherId === "t1")
              .map((c) => (
                <li key={c.id}>
                  <span>{c.name}</span>
                  <Badge count={c.studentCount} color="#1677ff" />
                </li>
              ))}
          </ul>
        </div>
      </Sider>

      <Layout>
        <Content className={styles.content}>{children}</Content>
      </Layout>
    </Layout>
  );
}
