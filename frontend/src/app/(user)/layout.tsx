import type { Metadata } from "next";
import { cookies } from "next/headers";
import { NextIntlClientProvider } from "next-intl";
import ThemeProviderClient from "@/components/ThemeProviderClient";
import AntdThemeRegistry from "@/components/AntdThemeRegistry";
import "antd/dist/reset.css";
import "@/app/globals.scss";
import en from "@/messages/en.json";
import HeaderLayoutUser from "@/components/header/headerLayoutUser";
type Messages = typeof en;
type Locale = "en" | "vi";
interface IProps {
  children: React.ReactNode;
}

export const metadata: Metadata = {
  title: "Next + AntD + i18n (cookie) + Dark Mode",
  description: "Cookie-based locale, no URL prefix",
};

export const dynamic = "force-dynamic";
export const revalidate = 0;

async function loadMessages(loc: Locale): Promise<Messages> {
  const primary = {
    en: () => import("@/messages/en.json"),
    vi: () => import("@/messages/vi.json"),
    zh: () => import("@/messages/zh.json"),
    ko: () => import("@/messages/ko.json"),
  } as const;

  try {
    return (await primary[loc]()).default;
  } catch {
    const fallback = {
      en: () => import("../../messages/en.json"),
      vi: () => import("../../messages/vi.json"),
      zh: () => import("../../messages/zh.json"),
      ko: () => import("../../messages/ko.json"),
    } as const;

    return (await fallback[loc]()).default;
  }
}
export default async function RootLayout({ children }: IProps) {
  const cookieStore = await cookies();
  const cookie = cookieStore.get("NEXT_LOCALE")?.value as Locale | undefined;
  const locale: Locale =
    cookie && ["en", "vi", "zh", "ko"].includes(cookie) ? cookie : "vi";
  const messages = await loadMessages(locale);

  return (
    <html lang={locale} suppressHydrationWarning>
      <body>
        <ThemeProviderClient>
          <AntdThemeRegistry>
            <NextIntlClientProvider
              messages={messages}
              locale={locale}
              timeZone="Asia/Ho_Chi_Minh"
            >
              <HeaderLayoutUser />
              {children}
            </NextIntlClientProvider>
          </AntdThemeRegistry>
        </ThemeProviderClient>
      </body>
    </html>
  );
}
