"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Layout, Menu, Badge } from "antd";
import type { MenuProps } from "antd";
import {
  Home,
  DollarSign,
  Shield,
  LifeBuoy,
  Settings,
  FileText,
} from "lucide-react";
import styles from "@/assets/styles/AdminLayout.module.scss";

const { Sider, Content } = Layout;

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname() || "";

  const items: MenuProps["items"] = [
    {
      key: "/admin",
      icon: <Home size={18} />,
      label: <Link href="/admin">Trang chủ</Link>,
    },
    {
      key: "/admin/revenuepanel",
      icon: <DollarSign size={18} />,
      label: <Link href="/admin/revenuepanel">Doanh thu</Link>,
    },
    {
      key: "/admin/subscriptionmanagement",
      icon: <Shield size={18} />,
      label: <Link href="/admin/subscriptionmanagement">Subscriptions</Link>,
    },
    {
      key: "/admin/supporttickets",
      icon: <LifeBuoy size={18} />,
      label: <Link href="/admin/supporttickets">Hỗ trợ</Link>,
    },
    {
      key: "/admin/systemconfig",
      icon: <Settings size={18} />,
      label: <Link href="/admin/systemconfig">Cấu hình</Link>,
    },
    {
      key: "/admin/auditlogs",
      icon: <FileText size={18} />,
      label: <Link href="/admin/auditlogs">Audit Logs</Link>,
    },
  ];

  const allKeys = items.map((i) => String(i?.key));
  const selectedKey =
    allKeys.reduce(
      (best, k) =>
        pathname.startsWith(k) && k.length > best.length ? k : best,
      ""
    ) || "/admin";

  return (
    <Layout className={styles.shell}>
      <Sider
        width={260}
        className={styles.sider}
        breakpoint="lg"
        collapsedWidth={0}
      >
        <div className={styles.brand}>
          <span className={styles.dot} /> Admin
        </div>
        <Menu
          mode="inline"
          selectedKeys={[selectedKey]}
          items={items}
          className={styles.menu}
        />
        <div className={styles.summary}>
          <div>
            <span>Thông báo</span>
            <Badge count={3} />
          </div>
          <div>
            <span>Tickets mở</span>
            <Badge count={5} color="red" />
          </div>
        </div>
      </Sider>
      <Layout>
        <Content className={styles.content}>{children}</Content>
      </Layout>
    </Layout>
  );
}
