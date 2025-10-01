"use client";
import { useTranslations } from "next-intl";

const HomePageHeader = () => {
  const t = useTranslations("HomePage");
  return <h1>{t("title")}</h1>;
};
export default HomePageHeader;
