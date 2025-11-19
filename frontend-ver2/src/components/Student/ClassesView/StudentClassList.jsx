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
import { Users, BookOpen } from "lucide-react";
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
          teacherName: c.teacherName || c.ownerName || "Chưa có GV",
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
    <div className={styles.wrap}>
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
                {pagedClasses.map((c) => (
                  <Card key={c.id} className={styles.card} bordered>
                    <div className={styles.cardHeader}>
                      <div className={styles.meta}>
                        <div className={styles.cardTitle}>{c.name}</div>
                        {c.code && (
                          <Tag className={styles.tagCode}>{c.code}</Tag>
                        )}
                      </div>
                    </div>

                    <div className={styles.section}>
                      <div className={styles.rowBetween}>
                        <Text type="secondary">Tiến độ học tập</Text>
                        <Text strong>{c.progress}%</Text>
                      </div>
                      <Progress
                        percent={c.progress}
                        size="small"
                        showInfo={false}
                      />
                    </div>

                    <div className={styles.inline}>
                      <Users size={16} />
                      <span>Giáo viên: {c.teacherName}</span>
                    </div>

                    <div className={styles.footer}>
                      <div className={styles.inlineMuted}>
                        <BookOpen size={16} />
                        <span>{c.materialsCount} tài liệu</span>
                      </div>
                      <div className={styles.inlineMuted}>
                        <span>{c.testsCount} bài kiểm tra</span>
                      </div>
                    </div>
                  </Card>
                ))}
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
