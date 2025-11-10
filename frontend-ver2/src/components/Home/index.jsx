import React from "react";
import { Card, Row, Col, Typography, Button } from "antd";
import { ReadOutlined, TeamOutlined } from "@ant-design/icons";

const { Title, Paragraph } = Typography;

export default function Home({ onRoleSelect }) {
  return (
    <div style={{ minHeight: "calc(100vh - 64px)" }} className="flex-center">
      <div style={{ width: "100%", maxWidth: 960 }}>
        <div style={{ textAlign: "center", marginBottom: 32 }}>
          <div
            className="flex-center"
            style={{
              width: 80,
              height: 80,
              background: "#1677ff",
              borderRadius: "50%",
              margin: "0 auto 16px",
            }}
          >
            <ReadOutlined style={{ color: "#fff", fontSize: 40 }} />
          </div>
          <Title level={2} style={{ color: "#0f172a", marginBottom: 8 }}>
            Hệ thống kiểm tra trực tuyến
          </Title>
          <Paragraph type="secondary">
            Chọn vai trò của bạn để tiếp tục
          </Paragraph>
        </div>

        <Row gutter={[16, 16]}>
          <Col xs={24} md={12}>
            <Card className="section-card" hoverable>
              <Card.Meta
                avatar={
                  <div
                    className="flex-center"
                    style={{
                      width: 64,
                      height: 64,
                      background: "#e6f4ff",
                      borderRadius: 12,
                    }}
                  >
                    <TeamOutlined style={{ color: "#1677ff", fontSize: 28 }} />
                  </div>
                }
                title={<div style={{ color: "#0f172a" }}>Giáo viên</div>}
                description="Exam Creator"
              />
              <ul style={{ marginTop: 16, color: "#475569" }}>
                <li>• Quản lý câu hỏi</li>
                <li>• Tạo bài kiểm tra</li>
                <li>• Chấm bài và đánh giá</li>
              </ul>
              <Button
                type="primary"
                block
                onClick={() => onRoleSelect("teacher")}
              >
                Vào với vai trò Giáo viên
              </Button>
            </Card>
          </Col>

          <Col xs={24} md={12}>
            <Card className="section-card" hoverable>
              <Card.Meta
                avatar={
                  <div
                    className="flex-center"
                    style={{
                      width: 64,
                      height: 64,
                      background: "#e6f4ff",
                      borderRadius: 12,
                    }}
                  >
                    <ReadOutlined style={{ color: "#1677ff", fontSize: 28 }} />
                  </div>
                }
                title={<div style={{ color: "#0f172a" }}>Học sinh</div>}
                description="Exam Taker"
              />
              <ul style={{ marginTop: 16, color: "#475569" }}>
                <li>• Xem danh sách bài kiểm tra</li>
                <li>• Làm bài trực tuyến</li>
                <li>• Xem kết quả và điểm số</li>
              </ul>
              <Button
                type="primary"
                block
                onClick={() => onRoleSelect("student")}
              >
                Vào với vai trò Học sinh
              </Button>
            </Card>
          </Col>
        </Row>
      </div>
    </div>
  );
}
