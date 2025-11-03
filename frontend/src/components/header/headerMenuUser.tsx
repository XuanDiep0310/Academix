"use client";
import LocaleSwitcher from "@/components/LocaleSwitcher";
import type { MenuProps } from "antd";
import { Menu } from "antd";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { useSession, signOut } from "next-auth/react";
// signIn,

type MenuItem = Required<MenuProps>["items"][number];

const HeaderMenuUser = () => {
  // const [current, setCurrent] = useState("mail");
  const t = useTranslations("UserPage");
  const { data } = useSession();
  const session = data;
  console.log(">> check session", session);

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
    ...(session
      ? [
          {
            key: "user",
            label: <span>hihi </span>,
            children: [
              { key: "profile", label: <Link href="/profile">Profile</Link> },
              {
                key: "signout",
                label: (
                  <a
                    onClick={(e) => {
                      e.preventDefault();
                      signOut();
                    }}
                  >
                    Sign out
                  </a>
                ),
              },
            ],
          },
        ]
      : [
          { label: <Link href="/signup">{t("signUp")}</Link>, key: "signup" },
          {
            label: <Link href="/auth/signin">{t("signIn")}</Link>,
            key: "signin",
          },
        ]),
    {
      label: <LocaleSwitcher />,
      key: "LocaleSwitcher",
      disabled: true, // Disable item này
      style: {
        cursor: "default",
        opacity: 1, // Giữ opacity bình thường khi disabled
      },
    },
  ];

  const onClick: MenuProps["onClick"] = (e) => {
    console.log("click ", e);
    // setCurrent(e.key);
  };

  return <Menu onClick={onClick} mode="horizontal" items={items} />;
};

export default HeaderMenuUser;
