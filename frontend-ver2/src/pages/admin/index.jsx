// src/pages/admin/DashboardAdmin.jsx
import { Card, Col, Row, Statistic, Typography, Spin, Divider } from "antd";
import { useEffect, useState } from "react";
import CountUp from "react-countup";
import {
  callDashboardUsersAPI,
  callDashboardClassesAPI,
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

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        setLoading(true);

        // Gọi 2 API song song
        const [userRes, classRes] = await Promise.all([
          callDashboardUsersAPI(),
          callDashboardClassesAPI(),
        ]);

        if (userRes && userRes.success && userRes.data) {
          setUserStats(userRes.data);
        }

        if (classRes && classRes.success && classRes.data) {
          setClassStats(classRes.data);
        }
      } catch (e) {
        console.error("fetch dashboard error:", e);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, []);

  const formatter = (value) => <CountUp end={value || 0} separator="," />;

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
      {/* PHẦN 1: THỐNG KÊ NGƯỜI DÙNG */}
      <Title level={3} style={{ marginBottom: 16 }}>
        Tổng quan hệ thống
      </Title>
      <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
        Thống kê người dùng trên hệ thống Academix
      </Text>

      <Spin spinning={loading}>
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

        {/* PHẦN 2: THỐNG KÊ LỚP HỌC (lấy những cái quan trọng) */}
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
                precision={1}
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
      </Spin>
    </>
  );
};

export default AdminPage;
