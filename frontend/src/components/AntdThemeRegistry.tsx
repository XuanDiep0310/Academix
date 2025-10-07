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
            colorPrimary: "#a78bfa", // tím nhạt (dark)
            colorBgBase: "#0b0f19",
            colorTextBase: "#e2e8f0",
            borderRadius: 12,
            controlHeight: 40,
            fontSize: 14,
          }
        : {
            colorPrimary: "#7c3aed", // tím (light)
            colorBgBase: "#ffffff",
            colorTextBase: "#0f172a",
            borderRadius: 12,
            controlHeight: 40,
            fontSize: 14,
          },
    [isDark]
  );

  // Override component (đặc biệt là Button)
  const components = useMemo(
    () => ({
      Button: {
        borderRadius: 12,
        paddingInline: 16,
        fontWeight: 600,
        // primary
        colorPrimary: token.colorPrimary,
        colorPrimaryHover: isDark ? "#8b5cf6" : "#6d28d9",
        colorPrimaryActive: isDark ? "#7c3aed" : "#5b21b6",
        // default
        defaultBg: isDark ? "#111827" : "#f8fafc",
        defaultColor: isDark ? "#e5e7eb" : "#0f172a",
        defaultHoverBg: isDark ? "#1f2937" : "#f1f5f9",
        defaultActiveBg: isDark ? "#111827" : "#e2e8f0",
      },
      Typography: {
        colorLink: token.colorPrimary,
        colorLinkHover: isDark ? "#8b5cf6" : "#6d28d9",
        colorLinkActive: isDark ? "#7c3aed" : "#5b21b6",
      },
      Input: {
        borderRadius: 12,
        activeShadow: `0 0 0 2px ${
          isDark ? "rgba(167,139,250,0.25)" : "rgba(124,58,237,0.25)"
        }`,
      },
    }),
    [isDark, token.colorPrimary, token.colorTextBase]
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
        components,
        cssVar: true, // Bật CSS variables để bạn override thêm bằng :root/.dark nếu muốn
        hashed: true, // Để nguyên; set false nếu muốn dễ debug className
      }}
    >
      {children}
    </ConfigProvider>
  );
}
