"use client";
import React, { useTransition } from "react";
import { Select } from "antd";
import { useLocale } from "next-intl";
import { useRouter } from "next/navigation";

type Locale = "vi" | "en" | "zh" | "ko";

interface Option {
  value: Locale;
  label: string;
}
const OPTIONS: Option[] = [
  { value: "vi", label: "Tiếng Việt" },
  { value: "en", label: "English" },
  { value: "zh", label: "中文" },
  { value: "ko", label: "한국어" },
] as const;

export default function LocaleSwitcher() {
  const cur = useLocale();
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  // tránh value không tồn tại trong options
  const value: Locale = (
    OPTIONS.some((o) => o.value === cur) ? cur : "vi"
  ) as Locale;

  async function onChange(next: Locale) {
    if (next === cur) return;
    await fetch("/api/locale", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ locale: next }),
      cache: "no-store",
    });
    startTransition(() => router.refresh());
  }

  return (
    <Select
      style={{ width: 160 }}
      value={value}
      options={OPTIONS}
      loading={isPending}
      disabled={isPending}
      onChange={onChange}
      aria-label="Change language"
    />
  );
}
