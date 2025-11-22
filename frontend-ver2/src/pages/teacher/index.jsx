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
  Select,
} from "antd";
import CountUp from "react-countup";
import { FolderOpen, Upload, Users, BarChart } from "lucide-react"; // Import thêm icons
import {
  callMaterialsStatisticsAPI,
  callListMyClassesAPI,
} from "../../services/api.service";

// IMPORT SCSS
import styles from "../../assets/styles/TeacherPage.module.scss";

const { Title, Text } = Typography;

// MAPPING ICON VÀ MÀU CHO CÁC STATISTIC
const STAT_MAPPINGS = {
  totalMaterials: {
    title: "Tổng số tài liệu",
    icon: FolderOpen,
    color: "#1890ff",
  },
  totalStorageUsed: {
    title: "Dung lượng đã dùng",
    icon: BarChart,
    color: "#52c41a",
  },
  materialsUploadedToday: {
    title: "Tải lên hôm nay",
    icon: Upload,
    color: "#faad14",
  },
  materialsUploadedThisWeek: {
    title: "Tải lên tuần này",
    icon: Upload,
    color: "#eb2f96",
  },
  materialsUploadedThisMonth: {
    title: "Tải lên tháng này",
    icon: Upload,
    color: "#722ed1",
  },
};

// MAPPING ICON CHO PHÂN LOẠI TÀI LIỆU (Tùy chỉnh nếu cần)
const getTypeColor = (type) => {
  switch (type.toLowerCase()) {
    case "pdf":
      return "volcano";
    case "video":
      return "cyan";
    case "image":
      return "green";
    default:
      return "geekblue";
  }
};

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

  const [classes, setClasses] = useState([]);
  const [selectedClassId, setSelectedClassId] = useState(null);

  const [loadingStats, setLoadingStats] = useState(false);
  const [loadingClasses, setLoadingClasses] = useState(false);

  // ---- formatter cho Statistic ----
  const formatter = (value) => <CountUp end={value || 0} separator="," />;

  const materialTypeEntries = Object.entries(stats.materialsByType || {});

  // ================== LOAD DANH SÁCH LỚP CỦA GV ==================
  const fetchClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();
      if (res?.success && Array.isArray(res.data)) {
        const mapped = res.data.map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
        }));
        setClasses(mapped);

        // auto chọn lớp đầu tiên nếu chưa chọn
        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].id);
        }
      }
    } catch (e) {
      console.error("fetch classes error: ", e);
    } finally {
      setLoadingClasses(false);
    }
  };

  useEffect(() => {
    fetchClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // ================== LOAD STATISTICS THEO CLASS ==================
  const fetchStats = async (classId) => {
    if (!classId) return;
    try {
      setLoadingStats(true);
      const res = await callMaterialsStatisticsAPI(classId);

      if (res && res.success && res.data) {
        setStats({
          totalMaterials: res.data.totalMaterials ?? 0,
          materialsByType: res.data.materialsByType || {},
          totalStorageUsed: res.data.totalStorageUsed ?? 0,
          totalStorageUsedFormatted:
            res.data.totalStorageUsedFormatted || "0 MB",
          materialsUploadedToday: res.data.materialsUploadedToday ?? 0,
          materialsUploadedThisWeek: res.data.materialsUploadedThisWeek ?? 0,
          materialsUploadedThisMonth: res.data.materialsUploadedThisMonth ?? 0,
          topUploaders: Array.isArray(res.data.topUploaders)
            ? res.data.topUploaders
            : [],
        });
      } else {
        setStats((prev) => ({
          ...prev,
          materialsByType: {},
          topUploaders: [],
        }));
      }
    } catch (e) {
      console.error("fetch materials statistics error:", e);
    } finally {
      setLoadingStats(false);
    }
  };

  useEffect(() => {
    if (selectedClassId) {
      fetchStats(selectedClassId);
    }
  }, [selectedClassId]);

  const currentClass = classes.find((c) => c.id === selectedClassId);

  return (
    <div className={styles.wrap}>
      {/* HEADER + CHỌN LỚP */}
      <Title level={3} className={styles.mainTitle}>
        <BarChart size={28} style={{ marginRight: 12, color: "#1890ff" }} />
        Tổng quan tài nguyên giảng dạy
      </Title>
      <Text type="secondary" className={styles.subHeader}>
        Thống kê tài liệu, bài giảng, file mà bạn (và các GV khác) đã tải lên
        cho từng lớp.
      </Text>

      <div className={styles.classSelector}>
        <Text strong>Chọn lớp:</Text>
        <Select
          style={{ minWidth: 260 }}
          loading={loadingClasses}
          value={selectedClassId ?? undefined}
          placeholder="Chọn lớp"
          onChange={(v) => setSelectedClassId(v)}
          options={classes.map((c) => ({
            value: c.id,
            label: `${c.name} (${c.code})`,
          }))}
        />
        {currentClass && (
          <Text type="secondary" className={styles.currentClassText}>
            Đang xem thống kê cho lớp{" "}
            <Text strong className={styles.classNameHighlight}>
              {currentClass.name}
            </Text>
          </Text>
        )}
      </div>

      <Divider className={styles.customDivider} />

      <Spin spinning={loadingStats}>
        {/* PHẦN 1: TỔNG QUAN */}
        <Title level={4} className={styles.sectionTitle}>
          Thống kê chung
        </Title>
        <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
          {Object.keys(STAT_MAPPINGS).map((key) => {
            const map = STAT_MAPPINGS[key];
            const Icon = map.icon;
            const value =
              stats[key] === "totalStorageUsed"
                ? stats.totalStorageUsedFormatted
                : stats[key];

            return (
              <Col xs={24} sm={12} lg={8} xl={6} key={key}>
                <Card bordered={false} className={styles.statCard}>
                  <Statistic
                    title={map.title}
                    value={value}
                    formatter={
                      key.includes("totalStorageUsed") ? undefined : formatter
                    }
                    prefix={<Icon size={24} style={{ color: map.color }} />}
                  />
                </Card>
              </Col>
            );
          })}
        </Row>

        <Divider className={styles.customDivider} />

        {/* PHẦN 2: PHÂN LOẠI */}
        <Title level={4} className={styles.sectionTitle}>
          Phân loại tài liệu
        </Title>
        <Text type="secondary" className={styles.subHeader}>
          Thống kê số lượng tài liệu theo từng loại (PDF, video, hình ảnh,…)
        </Text>

        {materialTypeEntries.length === 0 ? (
          <Text type="secondary">Chưa có dữ liệu phân loại tài liệu</Text>
        ) : (
          <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
            {materialTypeEntries.map(([type, count]) => (
              <Col xs={24} sm={12} lg={8} xl={6} key={type}>
                <Card bordered={false} className={styles.statCard}>
                  <Statistic
                    title={type}
                    value={count}
                    formatter={formatter}
                    prefix={
                      <Tag color={getTypeColor(type)}>{type.toUpperCase()}</Tag>
                    }
                  />
                </Card>
              </Col>
            ))}
          </Row>
        )}

        <Divider className={styles.customDivider} />

        {/* PHẦN 3: TOP GIÁO VIÊN */}
        <Title level={4} className={styles.sectionTitle}>
          Giáo viên tích cực tải tài liệu
        </Title>
        <Text type="secondary" className={styles.subHeader}>
          Danh sách giáo viên có số lượng tài liệu upload nhiều nhất trong lớp
          này.
        </Text>

        {!stats.topUploaders || stats.topUploaders.length === 0 ? (
          <Text type="secondary">Chưa có dữ liệu top giáo viên.</Text>
        ) : (
          <Card bordered={false} className={styles.listCard}>
            <List
              itemLayout="horizontal"
              dataSource={stats.topUploaders}
              renderItem={(item, index) => (
                <List.Item className={styles.listItem}>
                  <List.Item.Meta
                    avatar={
                      <Users
                        size={20}
                        color={index < 3 ? "#faad14" : "#1890ff"}
                      />
                    }
                    title={
                      <>
                        <Tag
                          color={
                            index === 0
                              ? "gold"
                              : index === 1
                              ? "silver"
                              : index === 2
                              ? "bronze"
                              : "blue"
                          }
                          className={styles.rankTag}
                        >
                          #{index + 1}
                        </Tag>
                        <Text strong>
                          {item.fullName || "Giáo viên không tên"}
                        </Text>
                      </>
                    }
                    description={<Text type="secondary">{item.email}</Text>}
                  />
                  <div className={styles.materialCount}>
                    <Text
                      strong
                      style={{ color: "#1890ff", fontSize: "1.1em" }}
                    >
                      {item.materialCount}
                    </Text>{" "}
                    <Text type="secondary">tài liệu</Text>
                  </div>
                </List.Item>
              )}
            />
          </Card>
        )}
      </Spin>
    </div>
  );
};

export default TeacherPage;
