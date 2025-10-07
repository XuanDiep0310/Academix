"use client";

import { Card, Col, Row, Typography } from "antd";
import {
  ReadFilled,
  SolutionOutlined,
  HomeFilled,
  RightOutlined,
} from "@ant-design/icons";
import Link from "next/link";
import "@/assets/styles/whoForHomePage.scss";
import { useTranslations } from "next-intl";
const { Title, Paragraph } = Typography;

const WhoFor = () => {
  const t = useTranslations();
  const items = [
    {
      key: "student",
      icon: <ReadFilled />,
      title: t("UserPage.whoFor.student.title"),
      desc: t("UserPage.whoFor.student.desc"),
      href: t("UserPage.whoFor.student.href"),
    },
    {
      key: "teacher",
      icon: <SolutionOutlined />,
      title: t("UserPage.whoFor.teacher.title"),
      desc: t("UserPage.whoFor.teacher.desc"),
      href: t("UserPage.whoFor.teacher.href"),
    },
    {
      key: "center",
      icon: <HomeFilled />,
      title: t("UserPage.whoFor.center.title"),
      desc: t("UserPage.whoFor.center.desc"),
      href: t("UserPage.whoFor.center.href"),
    },
  ];

  return (
    <section className="who-for">
      <div className="container" style={{ overflow: "hidden" }}>
        <Row gutter={[16, 16]}>
          {items.map((it) => (
            <Col key={it.key} xs={24} md={12} lg={8}>
              <Card className="who-for__card">
                <div className="who-for__header">
                  <span className="who-for__icon">{it.icon}</span>
                  <Title level={4} className="who-for__title">
                    {it.title}
                  </Title>
                </div>

                <Paragraph className="who-for__desc">{it.desc}</Paragraph>

                <Link href={it.href} className="who-for__link">
                  Bắt đầu <RightOutlined />
                </Link>
              </Card>
            </Col>
          ))}
        </Row>
      </div>
    </section>
  );
};
export default WhoFor;
