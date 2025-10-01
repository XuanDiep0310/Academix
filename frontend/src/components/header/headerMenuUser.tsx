"use client";
import { useState } from "react";
import type { MenuProps } from "antd";
import { Menu } from "antd";
import { useTranslations } from "next-intl";
import Link from "next/link";

type MenuItem = Required<MenuProps>["items"][number];

const headerMenuUser = () => {
  // const [current, setCurrent] = useState("mail");
  const t = useTranslations("UserPage");

  const items: MenuItem[] = [
    {
      label: <Link href="/">{t("home")}</Link>,
      key: `${t("home")}`,
    },
    {
      label: <Link href="/userExam">{t("userExam")}</Link>,
      key: `${t("userExam")}`,
    },
    {
      label: <Link href="/courses">{t("courses")}</Link>,
      key: `${t("courses")}`,
    },
    {
      label: <Link href="/blog">{t("blog")}</Link>,
      key: `${t("blog")}`,
    },
    {
      label: <Link href="/video">{t("video")}</Link>,
      key: `${t("video")}`,
    },
    {
      label: <Link href="/signIn">{t("signIn")}</Link>,
      key: `${t("signIn")}`,
    },
  ];

  const onClick: MenuProps["onClick"] = (e) => {
    console.log("click ", e);
    // setCurrent(e.key);
  };

  return <Menu onClick={onClick} mode="horizontal" items={items} />;
};

export default headerMenuUser;
