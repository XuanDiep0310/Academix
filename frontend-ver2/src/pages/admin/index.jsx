// src/pages/admin/DashboardAdmin.jsx
import { Card, Col, Row, Statistic, Typography, Spin, Divider } from "antd";
import { useEffect, useRef, useState } from "react";
import CountUp from "react-countup";
// Import Icons từ lucide-react
import {
  Users,
  UserCheck,
  UserX,
  GraduationCap,
  ClipboardList,
  Upload,
  BarChart,
  Target,
  FileText,
  Bookmark,
  BookOpen,
  TrendingUp, // Sử dụng cho các chỉ số tăng trưởng
  Database, // Sử dụng cho dung lượng
} from "lucide-react";
import {
  callDashboardUsersAPI,
  callDashboardClassesAPI,
  callMaterialsStatisticsGlobalAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;

// Mảng màu sắc cơ bản của Ant Design và style card mới
const COLORS = {
  primary: "#1677ff", // Blue
  success: "#52c41a", // Green
  warning: "#faad14", // Gold
  error: "#ff4d4f", // Red
  secondary: "rgba(0, 0, 0, 0.45)", // Tiêu đề phụ
};

const STATISTIC_CARD_STYLE = {
  // Thêm box-shadow nhẹ để card nổi lên trên nền
  boxShadow: "0 4px 12px rgba(0, 0, 0, 0.05)",
  borderRadius: 8,
  transition: "all 0.3s",
};

const AdminPage = () => {
  // ... (Giữ nguyên các State và Logic useEffect)
  const [userStats, setUserStats] = useState({
    totalUsers: 0,
    totalAdmins: 0,
    totalTeachers: 0,
    totalStudents: 0,
    activeUsers: 0,
    inactiveUsers: 0,
    userGrowth: [],
  });

  const [classStats, setClassStats] = useState({
    totalClasses: 0,
    activeClasses: 0,
    inactiveClasses: 0,
    totalTeachers: 0,
    totalStudents: 0,
    averageStudentsPerClass: 0,
    classGrowth: [],
  });

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
        🚀 Tổng quan hệ thống
      </Title>
      <Text type="secondary" style={{ display: "block", marginBottom: 24 }}>
        Thống kê người dùng, lớp học và tài liệu toàn hệ thống
      </Text>

      {/* bọc toàn bộ block thống kê trong ref để observer theo dõi */}
      <div ref={statsRef}>
        <Spin spinning={loading}>
          {/* PHẦN 1: THỐNG KÊ NGƯỜI DÙNG */}
          <Title level={4} style={{ marginTop: 0, marginBottom: 16 }}>
            👥 Thống kê người dùng
          </Title>
          <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
            {/* Cột 1: Tổng người dùng (Primary) */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng người dùng"
                  value={userStats.totalUsers}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.primary, fontWeight: 700 }} // Đậm hơn
                  prefix={<Users size={18} color={COLORS.primary} />}
                />
              </Card>
            </Col>

            {/* Cột 2: Tổng giáo viên */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng giáo viên"
                  value={userStats.totalTeachers}
                  formatter={formatter}
                  prefix={<BookOpen size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 3: Tổng học sinh */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng học sinh"
                  value={userStats.totalStudents}
                  formatter={formatter}
                  prefix={<Bookmark size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 4: Số admin */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Số admin"
                  value={userStats.totalAdmins}
                  formatter={formatter}
                  prefix={<Target size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 5: Đang hoạt động (Success) */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Đang hoạt động"
                  value={userStats.activeUsers}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.success, fontWeight: 700 }}
                  prefix={<UserCheck size={18} color={COLORS.success} />}
                />
              </Card>
            </Col>

            {/* Cột 6: Không hoạt động (Warning) */}
            <Col xs={24} sm={12} lg={6} xl={4}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Không hoạt động"
                  value={userStats.inactiveUsers}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.warning, fontWeight: 700 }}
                  prefix={<UserX size={18} color={COLORS.warning} />}
                />
              </Card>
            </Col>

            {/* Cột 7: Người dùng mới (Tháng gần nhất) (Primary) - Kích thước lớn hơn */}
            <Col xs={24} md={12} lg={8}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title={
                    latestUserGrowth.month
                      ? `Người dùng mới (${latestUserGrowth.month})`
                      : "Người dùng mới (tháng gần nhất)"
                  }
                  value={latestUserGrowth.count}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.primary, fontWeight: 700 }}
                  prefix={<TrendingUp size={18} color={COLORS.primary} />}
                />
              </Card>
            </Col>
          </Row>

          <Divider />

          {/* PHẦN 2: THỐNG KÊ LỚP HỌC */}
          <Title level={4} style={{ marginBottom: 16 }}>
            📚 Thống kê lớp học
          </Title>
          <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
            {/* Cột 1: Tổng số lớp (Primary) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng số lớp"
                  value={classStats.totalClasses}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.primary, fontWeight: 700 }}
                  prefix={<GraduationCap size={18} color={COLORS.primary} />}
                />
              </Card>
            </Col>

            {/* Cột 2: Lớp đang hoạt động (Success) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Lớp đang hoạt động"
                  value={classStats.activeClasses}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.success, fontWeight: 700 }}
                  prefix={<UserCheck size={18} color={COLORS.success} />}
                />
              </Card>
            </Col>

            {/* Cột 3: Lớp ngừng hoạt động (Error) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Lớp ngừng hoạt động"
                  value={classStats.inactiveClasses}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.error, fontWeight: 700 }}
                  prefix={<UserX size={18} color={COLORS.error} />}
                />
              </Card>
            </Col>

            {/* Cột 4: Tổng học sinh (trong các lớp) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng học sinh (trong các lớp)"
                  value={classStats.totalStudents}
                  formatter={formatter}
                  prefix={<Users size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 5: Sĩ số trung bình / lớp */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Sĩ số trung bình / lớp"
                  value={classStats.averageStudentsPerClass}
                  precision={1} // Sử dụng precision của Statistic
                  prefix={<Target size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 6: Lớp mới (Tháng gần nhất) (Primary) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title={
                    latestClassGrowth.month
                      ? `Lớp mới (${latestClassGrowth.month})`
                      : "Lớp mới (tháng gần nhất)"
                  }
                  value={latestClassGrowth.count}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.primary, fontWeight: 700 }}
                  prefix={<TrendingUp size={18} color={COLORS.primary} />}
                />
              </Card>
            </Col>
          </Row>

          <Divider />

          {/* PHẦN 3: THỐNG KÊ TÀI LIỆU TOÀN HỆ THỐNG */}
          <Title level={4} style={{ marginBottom: 16 }}>
            📁 Thống kê tài liệu
          </Title>
          <Row gutter={[24, 24]} style={{ marginBottom: 16 }}>
            {/* Cột 1: Tổng số tài liệu (Primary) */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tổng số tài liệu"
                  value={materialStats.totalMaterials}
                  formatter={formatter}
                  valueStyle={{ color: COLORS.primary, fontWeight: 700 }}
                  prefix={<FileText size={18} color={COLORS.primary} />}
                />
              </Card>
            </Col>

            {/* Cột 2: Dung lượng đã dùng (Success) - Giữ nguyên không CountUp */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Dung lượng đã dùng"
                  value={materialStats.totalStorageUsed}
                  formatter={() => (
                    <Text
                      style={{
                        color: COLORS.success,
                        fontWeight: 700,
                        fontSize: "24px",
                      }}
                    >
                      {materialStats.totalStorageUsedFormatted}
                    </Text>
                  )}
                  prefix={<Database size={18} color={COLORS.success} />}
                />
              </Card>
            </Col>

            {/* Cột 3: Tài liệu tải lên hôm nay */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tài liệu tải lên hôm nay"
                  value={materialStats.materialsUploadedToday}
                  formatter={formatter}
                  prefix={<Upload size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 4: Tài liệu tuần này */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tài liệu tuần này"
                  value={materialStats.materialsUploadedThisWeek}
                  formatter={formatter}
                  prefix={<ClipboardList size={18} color={COLORS.secondary} />}
                />
              </Card>
            </Col>

            {/* Cột 5: Tài liệu tháng này */}
            <Col xs={24} sm={12} lg={8} xl={6}>
              <Card bordered={false} hoverable style={STATISTIC_CARD_STYLE}>
                <Statistic
                  title="Tài liệu tháng này"
                  value={materialStats.materialsUploadedThisMonth}
                  formatter={formatter}
                  prefix={<ClipboardList size={18} color={COLORS.secondary} />}
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
