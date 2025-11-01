"use client";
import { useEffect, useMemo, useState } from "react";
import { ConfigProvider, theme as antdTheme } from "antd";
import viVN from "antd/locale/vi_VN";
import { useTheme } from "next-themes";

export default function AntdThemeRegistry({
  children,
}: {
  children: React.ReactNode;
}) {
  const { resolvedTheme } = useTheme();
  const isDark = resolvedTheme === "dark";

  // Tránh flash mismatch giữa SSR và client khi đổi theme
  const [mounted, setMounted] = useState(false);
  useEffect(() => setMounted(true), []);

  // Chọn algorithm theo theme
  const algorithm = useMemo(
    () => (isDark ? [antdTheme.darkAlgorithm] : [antdTheme.defaultAlgorithm]),
    [isDark]
  );

  // Token nền tảng (có thể chỉnh màu brand ở đây)
  const token = useMemo(
    () =>
      isDark
        ? {
            // colorPrimary: "#a78bfa", // tím nhạt (dark)
            colorBgBase: "#0b0f19",
            colorTextBase: "#e2e8f0",
            borderRadius: 12,
            controlHeight: 40,
            fontSize: 14,
          }
        : {
            // colorPrimary: "#7c3aed", // tím (light)
            colorBgBase: "#ffffff",
            colorTextBase: "#0f172a",
            borderRadius: 12,
            controlHeight: 40,
            fontSize: 14,
          },
    [isDark]
  );

  if (!mounted) {
    // Ẩn tạm UI đến khi client biết theme để tránh nháy màu
    return <div style={{ visibility: "hidden" }}>{children}</div>;
  }
  return (
    <ConfigProvider
      locale={viVN}
      theme={{
        algorithm,
        token,
        cssVar: true, // Bật CSS variables để bạn override thêm bằng :root/.dark nếu muốn
        hashed: true, // Để nguyên; set false nếu muốn dễ debug className
      }}
    >
      {children}
    </ConfigProvider>
  );
}
