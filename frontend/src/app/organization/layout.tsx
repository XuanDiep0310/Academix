"use client";

import React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Layout, Menu } from "antd";
import type { MenuProps } from "antd";
import { Home, UserCog, Package, DollarSign } from "lucide-react";
import styles from "@/assets/styles/OrganizationLayout.module.scss";

const { Sider, Content } = Layout;

export default function OrganizationLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname() || "";

  const items: MenuProps["items"] = [
    {
      key: "/organization",
      icon: <Home size={18} />,
      label: <Link href="/organization">Trang chủ</Link>,
    },
    {
      key: "/organization/teachermanagement",
      icon: <UserCog size={18} />,
      label: (
        <Link href="/organization/teachermanagement">Quản lý giáo viên</Link>
      ),
    },
    {
      key: "/organization/subscriptionpanel",
      icon: <Package size={18} />,
      label: <Link href="/organization/subscriptionpanel">Gói dịch vụ</Link>,
    },
    {
      key: "/organization/financepanel",
      icon: <DollarSign size={18} />,
      label: <Link href="/organization/financepanel">Tài chính</Link>,
    },
  ];

  const allKeys = items.map((i) => String(i?.key));
  const selectedKey =
    allKeys.reduce(
      (best, k) =>
        pathname.startsWith(k) && k.length > best.length ? k : best,
      ""
    ) || "/organization";

  return (
    <Layout className={styles.shell}>
      <Sider
        width={260}
        className={styles.sider}
        breakpoint="lg"
        collapsedWidth={0}
      >
        <div className={styles.brand}>
          <span className={styles.dot} /> Organization
        </div>
        <Menu
          mode="inline"
          selectedKeys={[selectedKey]}
          items={items}
          className={styles.menu}
        />
      </Sider>
      <Layout>
        <Content className={styles.content}>{children}</Content>
      </Layout>
    </Layout>
  );
}
