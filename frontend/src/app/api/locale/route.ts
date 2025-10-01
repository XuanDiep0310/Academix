import { NextResponse } from "next/server";
export const dynamic = "force-dynamic";

const ALLOWED = new Set(["en", "vi", "zh", "ko"]);

export async function POST(req: Request) {
  const { locale } = await req.json();
  const value = ALLOWED.has(locale) ? locale : "vi";
  const res = NextResponse.json({ ok: true, locale: value });
  res.cookies.set("NEXT_LOCALE", value, {
    path: "/",
    maxAge: 60 * 60 * 24 * 365,
    sameSite: "lax",
  });
  return res;
}
