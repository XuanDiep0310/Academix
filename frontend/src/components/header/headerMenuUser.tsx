"use client";
import type { MenuProps } from "antd";
import { Menu } from "antd";
import { useTranslations } from "next-intl";
import Link from "next/link";

type MenuItem = Required<MenuProps>["items"][number];

const headerMenuUser = () => {
  // const [current, setCurrent] = useState("mail");
  const t = useTranslations("UserPage" as any);

  const items: MenuItem[] = [
    {
      label: <Link href="/">{t("home")}</Link>,
      key: `${t("home")}`,
    },

    {
      label: <Link href="/news">{t("news")}</Link>,
      key: `${t("news")}`,
    },
    {
      label: <Link href="/contact">{t("contact")}</Link>,
      key: `${t("contact")}`,
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
