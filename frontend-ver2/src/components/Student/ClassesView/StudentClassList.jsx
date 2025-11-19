import { useEffect, useMemo, useState } from "react";
import {
  Card,
  Typography,
  Tag,
  Progress,
  Pagination,
  Spin,
  Empty,
  message,
} from "antd";
import {
  Users,
  BookOpen,
  GraduationCap,
  Calendar,
  FileText,
  CheckCircle2,
} from "lucide-react";
import styles from "../../../assets/styles/StudentClassList.module.scss";
import { callListMyClassesAPI } from "../../../services/api.service";

const { Title, Text } = Typography;

export function StudentClassList() {
  const [classes, setClasses] = useState([]);
  const [loading, setLoading] = useState(false);

  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(6);

  const total = classes.length;

  const fetchMyClasses = async () => {
    try {
      setLoading(true);
      const res = await callListMyClassesAPI();

      if (res && res.data) {
        const data = res.data;
        const arr = Array.isArray(data)
          ? data
          : Array.isArray(data.data)
          ? data.data
          : [];

        const mapped = arr.map((c) => ({
          id: c.classId || c.id,
          name: c.className || c.name,
          code: c.classCode || c.code,
          description: c.description || "",
          teacherName: c.teacherName || c.ownerName || "Chưa có GV",
          teacherCount: c.teacherCount ?? 0,
          studentCount: c.studentCount ?? 0,
          joinedAt: c.joinedAt || "",
          isActive: c.isActive ?? true,
          progress: c.progress ?? 0,
          materialsCount: c.materialsCount ?? 0,
          testsCount: c.testsCount ?? 0,
        }));

        setClasses(mapped);
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchMyClasses error:", err);
      message.error("Có lỗi khi tải danh sách lớp học");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMyClasses();
  }, []);

  const pagedClasses = useMemo(() => {
    const start = (current - 1) * pageSize;
    return classes.slice(start, start + pageSize);
  }, [classes, current, pageSize]);

  const handleChangePage = (page, pageSizeNew) => {
    if (pageSizeNew && pageSizeNew !== pageSize) {
      setPageSize(pageSizeNew);
      setCurrent(1);
    } else {
      setCurrent(page);
    }
  };

  return (
    <div>
      <Card className={styles.containerCard} bordered={false}>
        {/* Header */}
        <div className={styles.header}>
          <div>
            <Title level={4} className={styles.title}>
              Lớp học của tôi
            </Title>
            <Text type="secondary">Các lớp học bạn đang tham gia</Text>
          </div>
        </div>

        <Spin spinning={loading}>
          {total === 0 && !loading ? (
            <div className={styles.emptyWrap}>
              <Empty description="Bạn chưa tham gia lớp học nào" />
            </div>
          ) : (
            <>
              {/* Grid */}
              <div className={styles.grid}>
                {pagedClasses.map((c) => {
                  const formatDate = (dateString) => {
                    if (!dateString) return "";
                    const date = new Date(dateString);
                    return date.toLocaleDateString("vi-VN", {
                      day: "2-digit",
                      month: "2-digit",
                      year: "numeric",
                    });
                  };

                  return (
                    <Card key={c.id} className={styles.card} bordered>
                      <div className={styles.cardHeader}>
                        <div className={styles.meta}>
                          <div className={styles.titleRow}>
                            <div className={styles.cardTitle}>{c.name}</div>
                            {c.isActive && (
                              <Tag
                                color="success"
                                className={styles.activeTag}
                                icon={<CheckCircle2 size={12} />}
                              >
                                Đang hoạt động
                              </Tag>
                            )}
                          </div>
                          {c.code && (
                            <Tag className={styles.tagCode}>{c.code}</Tag>
                          )}
                          {c.description && (
                            <Text
                              className={styles.description}
                              type="secondary"
                            >
                              {c.description}
                            </Text>
                          )}
                        </div>
                      </div>

                      <div className={styles.statsGrid}>
                        <div className={styles.statItem}>
                          <GraduationCap
                            size={18}
                            className={styles.statIcon}
                          />
                          <div className={styles.statContent}>
                            <Text className={styles.statLabel} type="secondary">
                              Giáo viên
                            </Text>
                            <Text className={styles.statValue} strong>
                              {c.teacherCount}
                            </Text>
                          </div>
                        </div>
                        <div className={styles.statItem}>
                          <Users size={18} className={styles.statIcon} />
                          <div className={styles.statContent}>
                            <Text className={styles.statLabel} type="secondary">
                              Học sinh
                            </Text>
                            <Text className={styles.statValue} strong>
                              {c.studentCount}
                            </Text>
                          </div>
                        </div>
                      </div>

                      {c.joinedAt && (
                        <div className={styles.inline}>
                          <Calendar size={16} />
                          <span>Tham gia: {formatDate(c.joinedAt)}</span>
                        </div>
                      )}
                    </Card>
                  );
                })}
              </div>

              {/* Pagination – chỉ hiện khi cần */}
              {total > pageSize && (
                <div className={styles.pagination}>
                  <Pagination
                    current={current}
                    pageSize={pageSize}
                    total={total}
                    showSizeChanger
                    pageSizeOptions={[4, 6, 8, 12]}
                    onChange={handleChangePage}
                    onShowSizeChange={handleChangePage}
                  />
                </div>
              )}
            </>
          )}
        </Spin>
      </Card>
    </div>
  );
}
