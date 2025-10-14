"use client";
import "@/assets/styles/titleHomePage.scss";
import { useTranslations } from "next-intl";
const TitleHomePage = () => {
  const t = useTranslations();
  return (
    <section className="title-home">
      <div className="container">
        <div className="title-home__content">
          <span>{t("UserPage.title")}</span>
          <p className=""></p>
        </div>
      </div>
    </section>
  );
};
export default TitleHomePage;
