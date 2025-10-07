"use client";
import { Button, Col, Rate, Row, Typography } from "antd";
// import Image from "next/image";
const { Title, Paragraph, Text } = Typography;
import "@/assets/styles/heroHomePage.scss";
import { TypeAnimation } from "react-type-animation";
import { useLocale, useTranslations } from "next-intl";
import { useMemo } from "react";
import diepAvatar from "@/assets/img/diep.jpg";
import voAvatar from "@/assets/img/vo.png";
import yenAvatar from "@/assets/img/yen.jpg";
import nganAvatar from "@/assets/img/ngan.jpg";
import diepandvoAvatar from "@/assets/img/diepandvo.jpg";
import voanddiepAvatar from "@/assets/img/voanddiep.jpg";
import Image from "next/image";
const HeroHomePage = () => {
  const t = useTranslations("UserPage");
  const locale = useLocale();

  const animSeq = useMemo(
    () => [
      t("hero.title-animation1"),
      1200,
      t("hero.title-animation2"),
      1200,
      t("hero.title-animation3"),
      1200,
      t("hero.title-animation4"),
      1200,
    ],
    [locale, t]
  );

  return (
    <>
      <section className="home-hero">
        <div className="container">
          <Row gutter={[20, 20]}>
            {/* Left */}
            <Col xl={10} lg={10} md={10} sm={24} xs={24}>
              <div className="home-hero__left">
                <div className="home-hero__badge">
                  <div className="home-hero__tag">
                    <h3>{t("hero.tag")}</h3>
                  </div>
                </div>

                {/* Title */}
                <Title level={1} className="home-hero__title">
                  <span>{t("hero.titleBegin")}</span>
                  <div className="home-hero__title--ani">
                    <TypeAnimation
                      key={locale}
                      sequence={animSeq}
                      wrapper="span"
                      speed={40}
                      style={{ display: "inline-block" }}
                      repeat={Infinity}
                    />
                  </div>
                  <div className="home-hero__line"></div>
                  <span>{t("hero.titleEnd")}</span>
                </Title>
                <div className="home-hero__lines"></div>
                <Paragraph className="home-hero__desc">
                  {t("hero.desc")}
                </Paragraph>

                <div className="home-hero__bottom">
                  <div className="home-hero__social">
                    <div className="home-hero__avatars">
                      <div className="home-hero__avatar">
                        <Image
                          src={voanddiepAvatar}
                          alt="VovaDiep"
                          className="home-hero__avatar--img"
                        />
                      </div>
                      <div className="home-hero__avatar">
                        {/* <img src={diepAvatar} alt="diep" />
                         */}
                        <Image
                          src={diepAvatar}
                          alt="diep"
                          className="home-hero__avatar--img"
                        />
                      </div>
                      <div className="home-hero__avatar">
                        <Image
                          src={voAvatar}
                          alt="vo"
                          className="home-hero__avatar--img"
                        />
                      </div>
                      <div className="home-hero__avatar">
                        <Image
                          src={yenAvatar}
                          alt="yen"
                          className="home-hero__avatar--img"
                        />
                      </div>
                      <div className="home-hero__avatar">
                        <Image
                          src={nganAvatar}
                          alt="ngan"
                          className="home-hero__avatar--img"
                        />
                      </div>
                    </div>

                    <span className="home-hero__proof">{t("hero.rate")}</span>
                  </div>
                  <Rate disabled defaultValue={5} className="home-hero__rate" />

                  <button className="home-hero__btn">{t("hero.btn")}</button>
                </div>
              </div>
            </Col>

            {/* Right (để trống/ảnh minh hoạ) */}
            <Col xl={14} lg={14} md={14} sm={24} xs={24}>
              <div className="home-hero__right">
                <Image
                  src={diepandvoAvatar}
                  alt="DiepVaVo"
                  className="home-hero__right--img"
                />
              </div>
            </Col>
          </Row>
        </div>
      </section>
    </>
  );
};
export default HeroHomePage;
