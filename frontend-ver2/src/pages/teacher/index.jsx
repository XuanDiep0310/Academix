// src/pages/teacher/TeacherPage.jsx
import { useEffect, useState } from "react";
import {
  Card,
  Col,
  Row,
  Statistic,
  Typography,
  Spin,
  Divider,
  List,
  Tag,
} from "antd";
import CountUp from "react-countup";
import { callMaterialsStatisticsAPI } from "../../services/api.service";

const { Title, Text } = Typography;

const TeacherPage = () => {
  const [stats, setStats] = useState({
    totalMaterials: 0,
    materialsByType: {},
    totalStorageUsed: 0,
    totalStorageUsedFormatted: "0 MB",
    materialsUploadedToday: 0,
    materialsUploadedThisWeek: 0,
    materialsUploadedThisMonth: 0,
    topUploaders: [],
  });

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setLoading(true);
        const res = await callMaterialsStatisticsAPI();

        if (res && res.success && res.data) {
          setStats({
            totalMaterials: res.data.totalMaterials ?? 0,
            materialsByType: res.data.materialsByType || {},
            totalStorageUsed: res.data.totalStorageUsed ?? 0,
            totalStorageUsedFormatted:
              res.data.totalStorageUsedFormatted || "0 MB",
            materialsUploadedToday: res.data.materialsUploadedToday ?? 0,
            materialsUploadedThisWeek: res.data.materialsUploadedThisWeek ?? 0,
            materialsUploadedThisMonth:
              res.data.materialsUploadedThisMonth ?? 0,
            topUploaders: Array.isArray(res.data.topUploaders)
              ? res.data.topUploaders
              : [],
          });
        }
      } catch (e) {
        console.error("fetch materials statistics error:", e);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const formatter = (value) => <CountUp end={value || 0} separator="," />;

  const materialTypeEntries = Object.entries(stats.materialsByType || {});

  return (
    <>
      {/* PHẦN 1: TỔNG QUAN TÀI NGUYÊN */}
      <Title level={3} style={{ marginBottom: 16 }}>
        Tổng quan tài nguyên giảng dạy
      </Title>
      <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
        Thống kê tài liệu, bài giảng, file mà bạn (và các GV khác) đã tải lên hệ
        thống
      </Text>

      <Spin spinning={loading}>
        <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
          <Col xs={24} md={12} lg={8}>
            <Card bordered={false}>
              <Statistic
                title="Tổng số tài liệu"
                value={stats.totalMaterials}
                formatter={formatter}
              />
            </Card>
          </Col>

          <Col xs={24} md={12} lg={8}>
            <Card bordered={false}>
              <Statistic
                title="Dung lượng đã dùng"
                value={stats.totalStorageUsed}
                formatter={() => stats.totalStorageUsedFormatted}
              />
            </Card>
          </Col>

          <Col xs={24} md={12} lg={8}>
            <Card bordered={false}>
              <Statistic
                title="Tài liệu tải lên hôm nay"
                value={stats.materialsUploadedToday}
                formatter={formatter}
              />
            </Card>
          </Col>

          <Col xs={24} md={12} lg={8}>
            <Card bordered={false}>
              <Statistic
                title="Tài liệu tuần này"
                value={stats.materialsUploadedThisWeek}
                formatter={formatter}
              />
            </Card>
          </Col>

          <Col xs={24} md={12} lg={8}>
            <Card bordered={false}>
              <Statistic
                title="Tài liệu tháng này"
                value={stats.materialsUploadedThisMonth}
                formatter={formatter}
              />
            </Card>
          </Col>
        </Row>

        <Divider />

        {/* PHẦN 2: PHÂN BỔ THEO LOẠI TÀI LIỆU */}
        <Title level={4} style={{ marginBottom: 16 }}>
          Phân loại tài liệu
        </Title>
        <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
          Thống kê số lượng tài liệu theo từng loại (PDF, video, bài giảng,…)
        </Text>

        {materialTypeEntries.length === 0 ? (
          <Text type="secondary">Chưa có dữ liệu phân loại tài liệu</Text>
        ) : (
          <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
            {materialTypeEntries.map(([type, count]) => (
              <Col xs={24} md={12} lg={8} key={type}>
                <Card bordered={false}>
                  <Statistic title={type} value={count} formatter={formatter} />
                </Card>
              </Col>
            ))}
          </Row>
        )}

        <Divider />

        {/* PHẦN 3: TOP GIÁO VIÊN TẢI LÊN NHIỀU NHẤT */}
        <Title level={4} style={{ marginBottom: 16 }}>
          Giáo viên tích cực tải tài liệu
        </Title>
        <Text type="secondary" style={{ display: "block", marginBottom: 16 }}>
          Danh sách những giáo viên có số lượng tài liệu upload nhiều nhất
        </Text>

        {!stats.topUploaders || stats.topUploaders.length === 0 ? (
          <Text type="secondary">Chưa có dữ liệu top giáo viên.</Text>
        ) : (
          <Card bordered={false}>
            <List
              itemLayout="horizontal"
              dataSource={stats.topUploaders}
              renderItem={(item, index) => (
                <List.Item>
                  <List.Item.Meta
                    title={
                      <>
                        <Tag color="blue" style={{ marginRight: 8 }}>
                          # {index + 1}
                        </Tag>
                        {item.fullName || "Giáo viên không tên"}
                      </>
                    }
                    description={item.email}
                  />
                  <div>
                    <Text strong>{item.materialCount}</Text>{" "}
                    <Text type="secondary">tài liệu</Text>
                  </div>
                </List.Item>
              )}
            />
          </Card>
        )}
      </Spin>
    </>
  );
};

export default TeacherPage;
