"use client";
import { useEffect, useState } from "react";
import { ConfigProvider, theme as antdTheme } from "antd";
import { useTheme } from "next-themes";

export default function AntdThemeRegistry({
  children,
}: {
  children: React.ReactNode;
}) {
  const { resolvedTheme } = useTheme();
  const isDark = resolvedTheme === "dark";
  const [mounted, setMounted] = useState(false);
  useEffect(() => setMounted(true), []);
  const algorithm = isDark
    ? antdTheme.darkAlgorithm
    : antdTheme.defaultAlgorithm;
  return (
    <ConfigProvider theme={{ algorithm }}>
      {mounted ? (
        children
      ) : (
        <div style={{ visibility: "hidden" }}>{children}</div>
      )}
    </ConfigProvider>
  );
}
