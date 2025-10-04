"use client";
import { Avatar, Col, Rate, Row, Tag, Typography } from "antd";
// import Image from "next/image";
const { Title, Paragraph, Text } = Typography;
import "@/assets/styles/heroHomePage.scss";
const HeroHomePage = () => {
  return (
    <>
      <section className="home-hero">
        <div className="container">
          <Row gutter={[20, 20]}>
            {/* Left */}
            <Col xl={12} lg={12} md={12} sm={24} xs={24}>
              <div className="home-hero__left">
                {/* Badge */}
                <div className="home-hero__badge">
                  <Tag className="home-hero__tag" color="blue">
                    #1 Nền tảng thi trắc nghiệm online tốt nhất
                  </Tag>
                </div>

                {/* Title */}
                <Title level={1} className="home-hero__title">
                  Có một cách đơn giản hơn để{" "}
                  <span className="title-accent">tạo</span>
                  <br />
                  <span>trắc nghiệm online</span>
                </Title>

                {/* Description */}
                <Paragraph className="home-hero__desc">
                  Tạo câu hỏi và đề thi nhanh với những giải pháp thông minh.
                  <br />
                  <Text strong>
                    Academix tận dụng sức mạnh công nghệ để nâng cao trình độ
                    học tập của bạn.
                  </Text>
                </Paragraph>

                {/* Social proof */}
                <div className="home-hero__social">
                  <Avatar.Group
                  // maxCount={3}
                  // maxStyle={{ color: "#1677ff", background: "#e6f4ff" }}
                  >
                    {/* <Avatar src="/avatars/a1.png" />
                <Avatar src="/avatars/a2.png" />
                <Avatar src="/avatars/a3.png" />
                <Avatar src="/avatars/a4.png" /> */}
                  </Avatar.Group>

                  <div className="home-hero__proof">
                    <Text>
                      <Text strong className="text-primary">
                        Hơn 200.000+
                      </Text>
                      khách hàng đã yêu thích sử dụng
                    </Text>
                    <Rate
                      disabled
                      defaultValue={5}
                      className="home-hero__rate"
                    />
                  </div>
                </div>
              </div>
            </Col>

            {/* Right (để trống/ảnh minh hoạ) */}
            <Col xl={12} lg={12} md={12} sm={24} xs={24}>
              <div className="home-hero__right" />
            </Col>
          </Row>
        </div>
      </section>
    </>
  );
};
export default HeroHomePage;
