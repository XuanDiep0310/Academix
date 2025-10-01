"use client";

import React from "react";
import { Segmented } from "antd";
import { useTheme } from "next-themes";
import { useTranslations } from "next-intl";

export default function ThemeToggle() {
  const { setTheme, theme } = useTheme();
  const t = useTranslations("Theme");

  const [mounted, setMounted] = React.useState(false);
  React.useEffect(() => setMounted(true), []);

  const options = [
    { label: t("light"), value: "light" },
    { label: t("dark"), value: "dark" },
    { label: t("system"), value: "system" },
  ];

  if (!mounted) {
    // SSR & hydration khớp hoàn toàn (không controlled)
    return (
      <Segmented
        options={options}
        defaultValue="system"
        aria-label="theme-toggle"
      />
    );
  }

  return (
    <Segmented
      options={options}
      value={theme ?? "system"}
      onChange={(v) => setTheme(String(v))}
      aria-label="theme-toggle"
    />
  );
}
