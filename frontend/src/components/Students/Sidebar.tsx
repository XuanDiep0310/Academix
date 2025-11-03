"use client";
import React from "react";
import { Home, BookMarked, MessageSquare, MessageCircle } from "lucide-react";
import { Menu } from "antd";
import styles from "@/assets/styles/StudentDashboard.module.scss";
import { ClassInfo } from "@/data/seed";

type ViewType = "dashboard" | "exam" | "documents" | "chatbot" | "qa";

interface SidebarProps {
  currentView: ViewType;
  setCurrentView: (view: ViewType) => void;
  classes: ClassInfo[];
}

const navItems = [
  { key: "dashboard", icon: <Home />, label: "Trang chủ" },
  { key: "documents", icon: <BookMarked />, label: "Tài liệu" },
  { key: "chatbot", icon: <MessageSquare />, label: "Chatbot hỗ trợ" },
  { key: "qa", icon: <MessageCircle />, label: "Hỏi đáp" },
];

export const Sidebar = (props: SidebarProps) => {
  const { currentView, setCurrentView, classes } = props;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const handleMenuClick = (e: any) => {
    setCurrentView(e.key as ViewType);
  };

  return (
    <aside className={styles.sidebar}>
      <Menu
        mode="inline"
        selectedKeys={[currentView]}
        onClick={handleMenuClick}
        className={styles.navMenu}
        items={navItems.map((item) => ({
          key: item.key,
          icon: item.icon,
          label: item.label,
        }))}
      />
      <div className={styles.classesSection}>
        <h4>Lớp học của bạn</h4>
        <div className={styles.classesList}>
          {classes.slice(0, 3).map((cls) => (
            <div key={cls.id} className={styles.classItem}>
              {cls.name}
            </div>
          ))}
        </div>
      </div>
    </aside>
  );
};
