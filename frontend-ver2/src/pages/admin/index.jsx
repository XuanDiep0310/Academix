// src/pages/admin/DashboardAdmin.jsx
import { Card, Col, Row, Statistic, Typography, Spin, Divider } from "antd";
import { useEffect, useRef, useState } from "react";
import CountUp from "react-countup";
import {
  callDashboardUsersAPI,
  callDashboardClassesAPI,
  callMaterialsStatisticsGlobalAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;

const AdminPage = () => {
  // Thống kê người dùng
  const [userStats, setUserStats] = useState({
    totalUsers: 0,
    totalAdmins: 0,
    totalTeachers: 0,
    totalStudents: 0,
    activeUsers: 0,
    inactiveUsers: 0,
    userGrowth: [],
  });

  // Thống kê lớp học
  const [classStats, setClassStats] = useState({
    totalClasses: 0,
    activeClasses: 0,
    inactiveClasses: 0,
    totalTeachers: 0,
    totalStudents: 0,
    averageStudentsPerClass: 0,
    classGrowth: [],
  });

  // Thống kê tài liệu (từ /api/materials/statistics)
  const [materialStats, setMaterialStats] = useState({
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

  // === hiệu ứng chạy số khi scroll ===
  const statsRef = useRef(null);
  const [animateNumbers, setAnimateNumbers] = useState(false);

  // bật cờ animateNumbers khi block thống kê vào viewport
  useEffect(() => {
    const el = statsRef.current;
    if (!el) return;

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            setAnimateNumbers(true);
            observer.disconnect(); // chỉ chạy 1 lần
          }
        });
      },
      {
        threshold: 0.2, // nhìn thấy ~20% là kích hoạt
      }
    );

    observer.observe(el);

    return () => observer.disconnect();
  }, []);

  // gọi dashboard APIs
  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        setLoading(true);

        const [userRes, classRes, materialRes] = await Promise.all([
          callDashboardUsersAPI(),
          callDashboardClassesAPI(),
          callMaterialsStatisticsGlobalAPI(),
        ]);

        if (userRes && userRes.success && userRes.data) {
          setUserStats(userRes.data);
        }

        if (classRes && classRes.success && classRes.data) {
          setClassStats(classRes.data);
        }

        if (materialRes && materialRes.success && materialRes.data) {
          const d = materialRes.data;
          setMaterialStats({
            totalMaterials: d.totalMaterials ?? 0,
            materialsByType: d.materialsByType || {},
            totalStorageUsed: d.totalStorageUsed ?? 0,
            totalStorageUsedFormatted: d.totalStorageUsedFormatted || "0 MB",
            materialsUploadedToday: d.materialsUploadedToday ?? 0,
            materialsUploadedThisWeek: d.materialsUploadedThisWeek ?? 0,
            materialsUploadedThisMonth: d.materialsUploadedThisMonth ?? 0,
            topUploaders: Array.isArray(d.topUploaders) ? d.topUploaders : [],
          });
        }
      } catch (e) {
        console.error("fetch dashboard error:", e);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, []);

  // formatter cho Statistic: chỉ animate khi animateNumbers = true
  const formatter = (value) =>
    animateNumbers ? <CountUp end={value || 0} separator="," /> : value || 0;

  const latestUserGrowth =
    userStats.userGrowth && userStats.userGrowth.length > 0
      ? userStats.userGrowth[userStats.userGrowth.length - 1]
      : { month: "", count: 0 };

  const latestClassGrowth =
    classStats.classGrowth && classStats.classGrowth.length > 0
      ? classStats.classGrowth[classStats.classGrowth.length - 1]
      : { month: "", count: 0 };

  return (
    <>
      {/* HEADER */}
      <Title level={3} style={{ marginBottom: 16 }}>
        Tổng quan hệ thống
      </Title>
      <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
        Thống kê người dùng trên hệ thống Academix
      </Text>

      {/* bọc toàn bộ block thống kê trong ref để observer theo dõi */}
      <div ref={statsRef}>
        <Spin spinning={loading}>
          {/* PHẦN 1: THỐNG KÊ NGƯỜI DÙNG */}
          <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng người dùng"
                  value={userStats.totalUsers}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng giáo viên"
                  value={userStats.totalTeachers}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng học sinh"
                  value={userStats.totalStudents}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Số admin"
                  value={userStats.totalAdmins}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Đang hoạt động"
                  value={userStats.activeUsers}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Không hoạt động"
                  value={userStats.inactiveUsers}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title={
                    latestUserGrowth.month
                      ? `Người dùng mới (${latestUserGrowth.month})`
                      : "Người dùng mới (tháng gần nhất)"
                  }
                  value={latestUserGrowth.count}
                  formatter={formatter}
                />
              </Card>
            </Col>
          </Row>

          <Divider />

          {/* PHẦN 2: THỐNG KÊ LỚP HỌC */}
          <Title level={4} style={{ marginBottom: 16 }}>
            Thống kê lớp học
          </Title>
          <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
            Tổng quan số lượng lớp, tình trạng hoạt động và sĩ số trung bình
          </Text>

          <Row gutter={[24, 24]}>
            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng số lớp"
                  value={classStats.totalClasses}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Lớp đang hoạt động"
                  value={classStats.activeClasses}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Lớp ngừng hoạt động"
                  value={classStats.inactiveClasses}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng học sinh (trong các lớp)"
                  value={classStats.totalStudents}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Sĩ số trung bình / lớp"
                  value={classStats.averageStudentsPerClass}
                  // nếu muốn hiển thị 1 chữ số thập phân có thể bỏ formatter để dùng precision
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title={
                    latestClassGrowth.month
                      ? `Lớp mới (${latestClassGrowth.month})`
                      : "Lớp mới (tháng gần nhất)"
                  }
                  value={latestClassGrowth.count}
                  formatter={formatter}
                />
              </Card>
            </Col>
          </Row>

          <Divider />

          {/* PHẦN 3: THỐNG KÊ TÀI LIỆU TOÀN HỆ THỐNG */}
          <Title level={4} style={{ marginBottom: 16 }}>
            Thống kê tài liệu
          </Title>
          <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
            Tổng quan số lượng tài liệu và dung lượng đã dùng trên hệ thống
          </Text>

          <Row gutter={[24, 24]} style={{ marginBottom: 16 }}>
            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tổng số tài liệu"
                  value={materialStats.totalMaterials}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                {/* Dung lượng dạng chuỗi "1.44 MB" → giữ formatter riêng, không CountUp */}
                <Statistic
                  title="Dung lượng đã dùng"
                  value={materialStats.totalStorageUsed}
                  formatter={() => materialStats.totalStorageUsedFormatted}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tài liệu tải lên hôm nay"
                  value={materialStats.materialsUploadedToday}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tài liệu tuần này"
                  value={materialStats.materialsUploadedThisWeek}
                  formatter={formatter}
                />
              </Card>
            </Col>

            <Col xs={24} md={12} lg={8}>
              <Card bordered={false}>
                <Statistic
                  title="Tài liệu tháng này"
                  value={materialStats.materialsUploadedThisMonth}
                  formatter={formatter}
                />
              </Card>
            </Col>
          </Row>
        </Spin>
      </div>
    </>
  );
};

export default AdminPage;
