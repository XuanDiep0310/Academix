// src/pages/admin/MaterialView.jsx (Đã chỉnh sửa)
import { useEffect, useMemo, useState } from "react";
import {
  Card,
  Typography,
  Tag,
  Button,
  Select,
  Modal,
  Empty,
  Spin,
  Pagination,
  message,
} from "antd";
import {
  FileText,
  Link as LinkIcon,
  Image,
  Video,
  Eye,
  Download,
} from "lucide-react";
import styles from "../../assets/styles/MaterialView.module.scss";
import {
  callListMyClassesAPI,
  callListMaterialsByClassAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;

const API_BASE_URL = import.meta.env.VITE_BACKEND_URL || "";

const buildFileUrl = (path) => {
  if (!path) return "";
  if (path.startsWith("http://") || path.startsWith("https://")) return path;
  return `${API_BASE_URL}${path}`;
};

const MATERIAL_ICONS = {
  pdf: FileText,
  link: LinkIcon,
  image: Image,
  video: Video,
  file: FileText,
};

const MATERIAL_LABELS = {
  pdf: "PDF",
  link: "Liên kết",
  image: "Hình ảnh",
  video: "Video",
  file: "Tập tin",
};

// MÀU SẮC TAG MỚI
const MATERIAL_TAG_COLORS = {
  pdf: "red", // Red
  link: "geekblue", // Blue
  image: "green", // Green
  video: "volcano", // Orange
  file: "default", // Default (Grey)
};

// Chuẩn hóa kiểu materialType backend -> key ở trên
const mapMaterialType = (materialType) => {
  const t = (materialType || "").toLowerCase();
  if (t.includes("pdf")) return "pdf";
  if (t.includes("video")) return "video";
  if (t.includes("image") || t.includes("img")) return "image";
  if (t.includes("link") || t.includes("url")) return "link";
  return "file";
};

/* ========================================================= */

export default function MaterialView() {
  // ... (Giữ nguyên State và Logic fetching)
  /* ------ lớp của student ------ */
  const [classes, setClasses] = useState([]);
  const [selectedClassId, setSelectedClassId] = useState(null);
  const [loadingClasses, setLoadingClasses] = useState(false);

  /* ------ materials ------ */
  const [materials, setMaterials] = useState([]);
  const [loadingMaterials, setLoadingMaterials] = useState(false);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(6);
  const [total, setTotal] = useState(0);

  /* ------ modal xem chi tiết ------ */
  const [viewing, setViewing] = useState(null);

  /* ================== FETCH LỚP CỦA STUDENT ================== */

  const fetchMyClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();

      if (res && res.success && res.data) {
        const arr = Array.isArray(res.data) ? res.data : res.data.data || [];
        const mapped = arr.map((c) => ({
          value: c.classId ?? c.id,
          label: `${c.className || c.name} (${c.classCode || c.code})`,
        }));

        setClasses(mapped);
        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].value);
        }
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchMyClasses error:", err);
      message.error("Có lỗi khi tải danh sách lớp học");
    } finally {
      setLoadingClasses(false);
    }
  };

  useEffect(() => {
    fetchMyClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  /* ================== FETCH MATERIALS THEO LỚP ================== */

  const fetchMaterials = async () => {
    if (!selectedClassId) return;

    try {
      setLoadingMaterials(true);
      const qs = new URLSearchParams();
      qs.set("page", String(page));
      qs.set("pageSize", String(pageSize));
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");

      const res = await callListMaterialsByClassAPI(
        selectedClassId,
        qs.toString()
      );

      if (res && res.success && res.data) {
        const api = res.data;
        const arr = Array.isArray(api.materials) ? api.materials : [];

        const mapped = arr.map((m) => {
          const type = mapMaterialType(m.materialType);
          return {
            id: m.materialId,
            title: m.title,
            type,
            url: buildFileUrl(m.fileUrl), // 👈 GHÉP URL ĐẦY ĐỦ Ở ĐÂY
            classId: m.classId,
            className: m.className,
            description: m.description,
            uploadedAt: m.createdAt,
            uploadedByName: m.uploadedByName,
            fileSizeFormatted: m.fileSizeFormatted,
          };
        });

        setMaterials(mapped);
        setTotal(api.totalCount ?? mapped.length);
      } else {
        message.error("Không thể tải danh sách tài liệu");
      }
    } catch (err) {
      console.error("fetchMaterials error:", err);
      message.error("Có lỗi khi tải tài liệu");
    } finally {
      setLoadingMaterials(false);
    }
  };

  useEffect(() => {
    fetchMaterials();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedClassId, page, pageSize]);

  /* ================== HANDLER / HELPERS ================== */

  const handleChangeClass = (value) => {
    setSelectedClassId(value);
    setPage(1);
  };

  const handleChangePage = (p, ps) => {
    if (ps !== pageSize) {
      setPageSize(ps);
      setPage(1);
    } else {
      setPage(p);
    }
  };

  const handleView = (m) => setViewing(m);

  const handleDownload = (m) => {
    if (m.url) {
      // giờ m.url đã là URL đầy đủ tới backend, mở tab mới để tải/xem
      window.open(m.url, "_blank", "noopener,noreferrer");
    }
  };

  // Build video embed url if YouTube
  const toEmbed = (url) =>
    url.includes("watch?v=") ? url.replace("watch?v=", "embed/") : url;

  const selectedClassLabel = useMemo(() => {
    const found = classes.find((c) => c.value === selectedClassId);
    return found?.label || "";
  }, [classes, selectedClassId]);

  /* ================== RENDER ================== */

  return (
    <div className={styles.wrap}>
      {/* Header & Filters */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            📂 Tài liệu học tập
          </Title>
          <Text type="secondary">
            Tài liệu và học liệu từ các lớp bạn đang tham gia
          </Text>
        </div>
        <div className={styles.filters}>
          <Select
            loading={loadingClasses}
            value={selectedClassId ?? undefined}
            onChange={handleChangeClass}
            style={{ width: 260 }}
            placeholder="Chọn lớp"
            options={classes}
          />
        </div>
      </div>

      <Spin spinning={loadingMaterials}>
        {/* List */}
        {materials.length === 0 ? (
          <Card className={styles.card}>
            <Empty description="Không có tài liệu nào trong lớp này" />
          </Card>
        ) : (
          <>
            <div className={styles.grid}>
              {materials.map((m) => {
                const Icon = MATERIAL_ICONS[m.type] || FileText;
                const tagColor = MATERIAL_TAG_COLORS[m.type] || "default"; // Lấy màu tag
                return (
                  <Card key={m.id} className={styles.card} bordered>
                    <div className={styles.cardHeader}>
                      <div className={styles.iconBox}>
                        <Icon size={20} /> {/* Tăng size Icon */}
                      </div>
                      <div className={styles.meta}>
                        <div className={styles.cardTitle}>{m.title}</div>
                        <div className={styles.tags}>
                          <Tag color="blue">{m.className}</Tag>{" "}
                          {/* Class Tag màu xanh */}
                          <Tag color={tagColor}>
                            {MATERIAL_LABELS[m.type] || "Tài liệu"}
                          </Tag>
                        </div>
                      </div>
                      <div className={styles.actions}>
                        <Button
                          size="small"
                          icon={<Download size={16} />}
                          onClick={() => handleDownload(m)}
                        >
                          Tải về
                        </Button>
                        <Button
                          size="small"
                          type="primary"
                          icon={<Eye size={16} />}
                          onClick={() => handleView(m)}
                        >
                          Xem
                        </Button>
                      </div>
                    </div>
                    <div className={styles.desc}>{m.description}</div>
                    <div className={styles.footer}>
                      <Text type="secondary">
                        {m.uploadedByName ? (
                          <>
                            <Text strong>GV: {m.uploadedByName}</Text>
                            {" • "}
                          </>
                        ) : (
                          ""
                        )}
                        Đăng ngày:{" "}
                        {m.uploadedAt
                          ? new Date(m.uploadedAt).toLocaleString("vi-VN")
                          : "-"}
                      </Text>
                    </div>
                  </Card>
                );
              })}
            </div>

            {/* Pagination */}
            {total > pageSize && (
              <div className={styles.pagination}>
                <Pagination
                  current={page}
                  pageSize={pageSize}
                  total={total}
                  showSizeChanger
                  pageSizeOptions={[6, 12, 18, 24]} // Tăng options để phù hợp với Grid 3 cột
                  onChange={handleChangePage}
                  onShowSizeChange={handleChangePage}
                />
              </div>
            )}
          </>
        )}
      </Spin>

      {/* Viewer */}
      <Modal
        title={viewing?.title}
        open={!!viewing}
        onCancel={() => setViewing(null)}
        footer={null}
        width={viewing?.type === "image" ? 720 : 1000} // Tối ưu chiều rộng modal
        bodyStyle={{ maxHeight: "80vh", overflow: "auto", padding: 16 }} // Tăng maxHeight và giảm padding
        destroyOnClose
      >
        {viewing && (
          <div className={styles.viewer}>
            <Text
              type="secondary"
              style={{ display: "block", marginBottom: 8 }}
            >
              {selectedClassLabel} •{" "}
              <Tag color={MATERIAL_TAG_COLORS[viewing.type]}>
                {MATERIAL_LABELS[viewing.type] || "Tài liệu"}
              </Tag>
            </Text>

            {/* Tăng kích thước iframe/embed cho trải nghiệm xem tốt hơn */}
            {viewing.type === "pdf" && viewing.url && (
              <iframe
                src={viewing.url}
                className={styles.iframe}
                style={{ height: 700 }} // Tăng chiều cao
                title={viewing.title}
              />
            )}

            {viewing.type === "video" && viewing.url && (
              <div className={styles.aspect}>
                <iframe
                  src={toEmbed(viewing.url)}
                  className={styles.aspectInner}
                  title={viewing.title}
                  allowFullScreen
                />
              </div>
            )}

            {viewing.type === "image" && viewing.url && (
              <img
                src={viewing.url}
                alt={viewing.title}
                className={styles.image}
              />
            )}

            {viewing.type === "link" && viewing.url && (
              <div className={styles.linkBox}>
                <p className={styles.linkHint}>Tài liệu liên kết bên ngoài</p>
                <Button type="primary">
                  <a
                    href={viewing.url}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    Mở liên kết
                  </a>
                </Button>
              </div>
            )}

            <Card size="small" className={styles.note}>
              <Text>{viewing.description}</Text>
              <div style={{ marginTop: 8 }}>
                <Text type="secondary" style={{ fontSize: 12 }}>
                  Tải lên bởi: {viewing.uploadedByName} | Kích thước:{" "}
                  {viewing.fileSizeFormatted || "N/A"}
                </Text>
              </div>
            </Card>
          </div>
        )}
      </Modal>
    </div>
  );
}
