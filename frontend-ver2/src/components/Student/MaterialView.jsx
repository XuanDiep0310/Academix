import { useMemo, useState } from "react";
import { Card, Typography, Tag, Button, Select, Modal, Empty } from "antd";
import {
  FileText,
  Link as LinkIcon,
  Image,
  Video,
  Eye,
  Download,
} from "lucide-react";
import styles from "../../assets/styles/MaterialView.module.scss";

const { Title, Text } = Typography;

/* ====================== BASE DATA (no API) ====================== */
const MATERIALS = [
  {
    id: "1",
    title: "Bài giảng chương 1",
    type: "pdf",
    url: "https://example.com/document.pdf",
    classId: "1",
    className: "Toán cao cấp 1",
    description: "Giới thiệu về đạo hàm và các quy tắc tính đạo hàm cơ bản",
    uploadedAt: "2024-03-01",
  },
  {
    id: "2",
    title: "Video hướng dẫn giải bài tập",
    type: "video",
    url: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    classId: "1",
    className: "Toán cao cấp 1",
    description: "Hướng dẫn chi tiết cách giải các dạng bài tập về đạo hàm",
    uploadedAt: "2024-03-05",
  },
  {
    id: "3",
    title: "Tài liệu tham khảo",
    type: "link",
    url: "https://www.example.com",
    classId: "2",
    className: "Lập trình C++",
    description: "Link đến tài liệu C++ trực tuyến",
    uploadedAt: "2024-03-03",
  },
  {
    id: "4",
    title: "Bài tập thực hành",
    type: "pdf",
    url: "https://example.com/exercises.pdf",
    classId: "2",
    className: "Lập trình C++",
    description: "Tập hợp bài tập lập trình C++",
    uploadedAt: "2024-03-07",
  },
];

const MATERIAL_ICONS = {
  pdf: FileText,
  link: LinkIcon,
  image: Image,
  video: Video,
};
const MATERIAL_LABELS = {
  pdf: "PDF",
  link: "Liên kết",
  image: "Hình ảnh",
  video: "Video",
};
/* =============================================================== */

export default function MaterialView() {
  const [selectedClassId, setSelectedClassId] = useState("all");
  const [viewing, setViewing] = useState(null);

  const classes = useMemo(() => {
    const map = new Map();
    MATERIALS.forEach((m) => map.set(m.classId, m.className));
    return Array.from(map, ([value, label]) => ({ value, label }));
  }, []);

  const list = useMemo(() => {
    return selectedClassId === "all"
      ? MATERIALS
      : MATERIALS.filter((m) => m.classId === selectedClassId);
  }, [selectedClassId]);

  const handleView = (m) => setViewing(m);
  const handleDownload = (m) => window.open(m.url, "_blank");

  // Build video embed url if YouTube
  const toEmbed = (url) =>
    url.includes("watch?v=") ? url.replace("watch?v=", "embed/") : url;

  return (
    <div className={styles.wrap}>
      {/* Header & Filters */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Tài liệu học tập
          </Title>
          <Text type="secondary">Tài liệu và học liệu từ các lớp học</Text>
        </div>
        <div className={styles.filters}>
          <Select
            value={selectedClassId}
            onChange={setSelectedClassId}
            style={{ width: 220 }}
            options={[{ value: "all", label: "Tất cả lớp" }, ...classes]}
          />
        </div>
      </div>

      {/* List */}
      {list.length === 0 ? (
        <Card className={styles.card}>
          <Empty description="Không có tài liệu nào" />
        </Card>
      ) : (
        <div className={styles.grid}>
          {list.map((m) => {
            const Icon = MATERIAL_ICONS[m.type];
            return (
              <Card key={m.id} className={styles.card} bordered>
                <div className={styles.cardHeader}>
                  <div className={styles.iconBox}>
                    <Icon size={18} />
                  </div>
                  <div className={styles.meta}>
                    <div className={styles.cardTitle}>{m.title}</div>
                    <div className={styles.tags}>
                      <Tag>{m.className}</Tag>
                      <Tag color="default">{MATERIAL_LABELS[m.type]}</Tag>
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
                  <Text type="secondary">Đăng ngày: {m.uploadedAt}</Text>
                </div>
              </Card>
            );
          })}
        </div>
      )}

      {/* Viewer */}
      <Modal
        title={viewing?.title}
        open={!!viewing}
        onCancel={() => setViewing(null)}
        footer={null}
        width={980}
        bodyStyle={{ maxHeight: "78vh", overflow: "auto" }}
        destroyOnClose
      >
        {viewing && (
          <div className={styles.viewer}>
            <Text
              type="secondary"
              style={{ display: "block", marginBottom: 8 }}
            >
              {viewing.className} • {MATERIAL_LABELS[viewing.type]}
            </Text>

            {viewing.type === "pdf" && (
              <iframe
                src={viewing.url}
                className={styles.iframe}
                title={viewing.title}
              />
            )}

            {viewing.type === "video" && (
              <div className={styles.aspect}>
                <iframe
                  src={toEmbed(viewing.url)}
                  className={styles.aspectInner}
                  title={viewing.title}
                  allowFullScreen
                />
              </div>
            )}

            {viewing.type === "image" && (
              <img
                src={viewing.url}
                alt={viewing.title}
                className={styles.image}
              />
            )}

            {viewing.type === "link" && (
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
            </Card>
          </div>
        )}
      </Modal>
    </div>
  );
}
