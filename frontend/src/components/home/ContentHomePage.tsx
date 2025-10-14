"use client";
import { Col, Row } from "antd";
import Image from "next/image";
import "@/assets/styles/contentHomePage.scss";
import diepandvoAvatar from "@/assets/img/diepandvo.jpg";

import { useTranslations } from "next-intl";
const ContentHomePage = () => {
  const t = useTranslations();
  return (
    <>
      <section className="content-home">
        <div className="content-home__wrapper">
          <div className="content-home__auto--bg">
            <div className="container">
              <Row gutter={[20, 20]}>
                <div className="content-home__auto">
                  <Col
                    span={24}
                    md={24}
                    lg={0}
                    xl={0}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                  <Col
                    span={24}
                    md={24}
                    lg={10}
                    xl={10}
                    className="content-home__desc"
                  >
                    <button className="content-home__btnTop">
                      <span> {t("UserPage.content.autoQuiz.label")}</span>
                    </button>
                    <div className="content-home__title">
                      <span className="content-home__title--color">
                        {t("UserPage.content.autoQuiz.title-color")}
                      </span>
                      <span>{t("UserPage.content.autoQuiz.title")}</span>
                    </div>
                    <div className="content-home__list">
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              viewBox="0 0 24 24"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              width="1em"
                              height="1em"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M20 11v-2a2 2 0 0 0 -2 -2h-12a2 2 0 0 0 -2 2v5a2 2 0 0 0 2 2h4"></path>
                              <path d="M14 21v-4a2 2 0 1 1 4 0v4"></path>
                              <path d="M14 19h4"></path>
                              <path d="M21 15v6"></path>
                            </svg>
                          </span>
                          <div>{t("UserPage.content.autoQuiz.desc1")}</div>
                        </div>
                      </div>
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              viewBox="0 0 24 24"
                              width="1em"
                              height="1em"
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M2,22 L6,22 L6,18 L2,18 L2,22 Z M22,2 L12,12 M22,10 L22,2 L14,2 M22,13 L18,13 L18,22 L22,22 L22,13 Z M10,22 L14,22 L14,16 L10,16 L10,22 Z" />
                            </svg>
                          </span>
                          <div>{t("UserPage.content.autoQuiz.desc2")}</div>
                        </div>
                      </div>
                      <div className="content-home__btnBottom">
                        <button>{t("UserPage.content.autoQuiz.button")}</button>
                      </div>
                    </div>
                  </Col>
                  <Col
                    span={0}
                    md={0}
                    lg={14}
                    xl={14}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                </div>
              </Row>
            </div>
          </div>
          <div className="content-home__friendly--bg">
            <div className="container">
              <Row gutter={[20, 20]}>
                <div className="content-home__friendly">
                  <Col
                    span={24}
                    md={24}
                    lg={14}
                    xl={14}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                  <Col
                    span={24}
                    md={24}
                    lg={10}
                    xl={10}
                    className="content-home__desc"
                  >
                    <button className="content-home__btnTop">
                      <span> {t("UserPage.content.friendly.label")}</span>
                    </button>
                    <div className="content-home__title">
                      <span className="content-home__title--color">
                        {t("UserPage.content.friendly.title-color")}
                      </span>
                      <span>{t("UserPage.content.friendly.title")}</span>
                    </div>
                    <div className="content-home__list">
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              viewBox="0 0 24 24"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              width="1em"
                              height="1em"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M20 11v-2a2 2 0 0 0 -2 -2h-12a2 2 0 0 0 -2 2v5a2 2 0 0 0 2 2h4"></path>
                              <path d="M14 21v-4a2 2 0 1 1 4 0v4"></path>
                              <path d="M14 19h4"></path>
                              <path d="M21 15v6"></path>
                            </svg>
                          </span>
                          <div>{t("UserPage.content.friendly.desc1")}</div>
                        </div>
                      </div>
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              viewBox="0 0 24 24"
                              width="1em"
                              height="1em"
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M2,22 L6,22 L6,18 L2,18 L2,22 Z M22,2 L12,12 M22,10 L22,2 L14,2 M22,13 L18,13 L18,22 L22,22 L22,13 Z M10,22 L14,22 L14,16 L10,16 L10,22 Z" />
                            </svg>
                          </span>
                          <div>{t("UserPage.content.friendly.desc2")}</div>
                        </div>
                      </div>
                      <div className="content-home__btnBottom">
                        <button>{t("UserPage.content.friendly.button")}</button>
                      </div>
                    </div>
                  </Col>
                </div>
              </Row>
            </div>
          </div>
          <div className="content-home__virtualExam--bg">
            <div className="container">
              <Row gutter={[20, 20]}>
                <div className="content-home__virtualExam">
                  <Col
                    span={24}
                    md={24}
                    lg={0}
                    xl={0}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                  <Col
                    span={24}
                    md={24}
                    lg={10}
                    xl={10}
                    className="content-home__desc"
                  >
                    <button className="content-home__btnTop">
                      <span> {t("UserPage.content.virtualExam.label")}</span>
                    </button>
                    <div className="content-home__title">
                      <span className="content-home__title--color">
                        {t("UserPage.content.virtualExam.title-color")}
                      </span>
                      <span>{t("UserPage.content.virtualExam.title")}</span>
                    </div>
                    <div className="content-home__list">
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              viewBox="0 0 24 24"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              width="1em"
                              height="1em"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M20 11v-2a2 2 0 0 0 -2 -2h-12a2 2 0 0 0 -2 2v5a2 2 0 0 0 2 2h4"></path>
                              <path d="M14 21v-4a2 2 0 1 1 4 0v4"></path>
                              <path d="M14 19h4"></path>
                              <path d="M21 15v6"></path>
                            </svg>
                          </span>
                          <div>{t("UserPage.content.virtualExam.desc1")}</div>
                        </div>
                      </div>
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              viewBox="0 0 24 24"
                              width="1em"
                              height="1em"
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M2,22 L6,22 L6,18 L2,18 L2,22 Z M22,2 L12,12 M22,10 L22,2 L14,2 M22,13 L18,13 L18,22 L22,22 L22,13 Z M10,22 L14,22 L14,16 L10,16 L10,22 Z" />
                            </svg>
                          </span>
                          <div>{t("UserPage.content.virtualExam.desc2")}</div>
                        </div>
                      </div>
                      <div className="content-home__btnBottom">
                        <button>
                          {t("UserPage.content.virtualExam.button")}
                        </button>
                      </div>
                    </div>
                  </Col>
                  <Col
                    span={0}
                    md={0}
                    lg={14}
                    xl={14}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                </div>
              </Row>
            </div>
          </div>
          <div className="content-home__learning--bg">
            <div className="container">
              <Row gutter={[20, 20]}>
                <div className="content-home__learning">
                  <Col
                    span={24}
                    md={24}
                    lg={14}
                    xl={14}
                    className="content-home__image--wrapper"
                  >
                    <div className="content-home__image">
                      <Image src={diepandvoAvatar} alt="DiepVaVo" />
                    </div>
                  </Col>
                  <Col
                    span={24}
                    md={24}
                    lg={10}
                    xl={10}
                    className="content-home__desc"
                  >
                    <button className="content-home__btnTop">
                      <span> {t("UserPage.content.learning.label")}</span>
                    </button>
                    <div className="content-home__title">
                      <span>{t("UserPage.content.learning.title")}</span>
                      <span className="content-home__title--color">
                        {t("UserPage.content.learning.title-color")}
                      </span>
                    </div>
                    <div className="content-home__list">
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              viewBox="0 0 24 24"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              width="1em"
                              height="1em"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M20 11v-2a2 2 0 0 0 -2 -2h-12a2 2 0 0 0 -2 2v5a2 2 0 0 0 2 2h4"></path>
                              <path d="M14 21v-4a2 2 0 1 1 4 0v4"></path>
                              <path d="M14 19h4"></path>
                              <path d="M21 15v6"></path>
                            </svg>
                          </span>
                          <div>{t("UserPage.content.learning.desc1")}</div>
                        </div>
                      </div>
                      <div>
                        <div className="content-home__item">
                          <span>
                            <svg
                              viewBox="0 0 24 24"
                              width="1em"
                              height="1em"
                              stroke="currentColor"
                              fill="none"
                              strokeWidth="2"
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path d="M2,22 L6,22 L6,18 L2,18 L2,22 Z M22,2 L12,12 M22,10 L22,2 L14,2 M22,13 L18,13 L18,22 L22,22 L22,13 Z M10,22 L14,22 L14,16 L10,16 L10,22 Z" />
                            </svg>
                          </span>
                          <div>{t("UserPage.content.learning.desc2")}</div>
                        </div>
                      </div>
                      <div className="content-home__btnBottom">
                        <button>{t("UserPage.content.learning.button")}</button>
                      </div>
                    </div>
                  </Col>
                </div>
              </Row>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};
export default ContentHomePage;
