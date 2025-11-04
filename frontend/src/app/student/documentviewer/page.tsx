"use client";
import React, { useState, useMemo } from "react";
import {
  FileText,
  Video,
  BookOpen,
  Download,
  Eye,
  CheckCircle,
  Search,
  Filter,
} from "lucide-react";
import { Tabs, Input, Select, Card, Button } from "antd";
import styles from "@/assets/styles/DocumentViewer.module.scss";
import { SEED_DOCUMENTS, Document } from "@/data/seed";

const { Option } = Select;
const { TabPane } = Tabs;

const DocumentViewer = () => {
  // Sử dụng dữ liệu seed, sau này có thể fetch từ API
  const documents = SEED_DOCUMENTS;

  const [activeTab, setActiveTab] = useState<
    "all" | "pdf" | "video" | "article"
  >("all");
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedClass, setSelectedClass] = useState<string>("all");

  const uniqueClasses = useMemo(
    () => ["all", ...new Set(documents.map((d) => d.className))],
    [documents]
  );

  const getIcon = (type: string) => {
    switch (type) {
      case "pdf":
        return <FileText size={24} />;
      case "video":
        return <Video size={24} />;
      default:
        return <BookOpen size={24} />;
    }
  };

  const getTypeLabel = (type: string) => {
    switch (type) {
      case "pdf":
        return "PDF";
      case "video":
        return "Video";
      default:
        return "Bài viết";
    }
  };

  const getFilteredDocuments = useMemo(() => {
    let docs = documents;

    if (activeTab !== "all") {
      docs = docs.filter((d) => d.type === activeTab);
    }

    if (selectedClass !== "all") {
      docs = docs.filter((d) => d.className === selectedClass);
    }

    if (searchTerm.trim()) {
      docs = docs.filter(
        (d) =>
          d.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
          d.uploadedBy.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    return docs;
  }, [documents, activeTab, selectedClass, searchTerm]);

  const documentsByType = useMemo(
    () => ({
      all: documents.length,
      pdf: documents.filter((d) => d.type === "pdf").length,
      video: documents.filter((d) => d.type === "video").length,
      article: documents.filter((d) => d.type === "article").length,
    }),
    [documents]
  );
  const viewedCount = documents.filter((d) => d.viewed).length;
  const totalCount = documents.length;

  const DocumentCard = ({ doc }: { doc: Document }) => (
    <Card className={styles.documentCard}>
      <div className={styles.cardHeader}>
        <div className={styles.cardHeaderContent}>
          <div className={`${styles.iconWrapper} ${styles[`icon${doc.type}`]}`}>
            {getIcon(doc.type)}
          </div>

          <div className={styles.cardInfo}>
            <div className={styles.titleRow}>
              <h3>{doc.title}</h3>
              {doc.viewed && (
                <span className={styles.viewedBadge}>
                  <CheckCircle size={16} />
                </span>
              )}
            </div>

            <div className={styles.metadata}>
              <span className={styles.metaItem}>{doc.className}</span>
              <span className={styles.separator}>•</span>
              <span className={styles.metaItem}>{doc.uploadedBy}</span>
            </div>
          </div>
        </div>

        <span className={styles.typeBadge}>{getTypeLabel(doc.type)}</span>
      </div>

      <div className={styles.cardContent}>
        <div className={styles.cardDetails}>
          <div className={styles.detailItem}>
            <span className={styles.label}>Ngày tải lên:</span>

            <span>{new Date(doc.uploadedAt).toLocaleDateString("vi-VN")}</span>
          </div>

          {doc.size && (
            <div className={styles.detailItem}>
              <span className={styles.label}>Kích thước:</span>
              <span>{doc.size}</span>
            </div>
          )}

          {doc.duration && (
            <div className={styles.detailItem}>
              <span className={styles.label}>Thời lượng:</span>
              <span>{doc.duration}</span>
            </div>
          )}

          {doc.version > 1 && (
            <span className={styles.versionBadge}>Phiên bản {doc.version}</span>
          )}
        </div>

        <div className={styles.cardActions}>
          <Button
            className={styles.btnView}
            type="primary"
            icon={<Eye size={16} />}
          >
            Xem
          </Button>

          <Button className={styles.btnDownload} icon={<Download size={16} />}>
            Tải về
          </Button>
        </div>
      </div>
    </Card>
  );

  const tabItems = [
    { key: "all", label: `Tất cả (${documentsByType.all})` },
    { key: "pdf", label: `PDF (${documentsByType.pdf})` },
    { key: "video", label: `Video (${documentsByType.video})` },
    { key: "article", label: `Bài viết (${documentsByType.article})` },
  ];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>Tài liệu học tập</h1>
          <p>Xem và tải về tài liệu cho các lớp học của bạn</p>
        </div>
      </div>
      {/* Search and Filter Bar */}
      <div className={styles.filterBar}>
        <div className={styles.searchBox}>
          <Input
            placeholder="Tìm kiếm tài liệu..."
            prefix={<Search className={styles.searchIcon} />}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className={styles.antdInputOverride}
          />
        </div>

        <div className={styles.classFilter}>
          <Select
            value={selectedClass}
            onChange={setSelectedClass}
            style={{ width: "100%" }}
            suffixIcon={<Filter size={18} />}
          >
            <Option value="all">Tất cả lớp học</Option>
            {uniqueClasses
              .filter((c) => c !== "all")
              .map((className) => (
                <Option key={className} value={className}>
                  {className}
                </Option>
              ))}
          </Select>
        </div>
      </div>
      <div className={styles.tabs}>
        <Tabs
          defaultActiveKey="all"
          onChange={(key) =>
            setActiveTab(key as "all" | "pdf" | "video" | "article")
          }
          tabBarExtraContent={
            <div className={styles.stats}>
              <div className={styles.statItem}>
                <span className={styles.statValue}>
                  {viewedCount}/{totalCount}
                </span>
                <span className={styles.statLabel}>Đã xem</span>
              </div>
            </div>
          }
        >
          {tabItems.map((item) => (
            <TabPane tab={item.label} key={item.key}>
              <div className={styles.tabContent}>
                {getFilteredDocuments.length > 0 ? (
                  <div className={styles.documentsGrid}>
                    {getFilteredDocuments.map((doc) => (
                      <DocumentCard key={doc.id} doc={doc} />
                    ))}
                  </div>
                ) : (
                  <div className={styles.emptyState}>
                    <BookOpen className={styles.emptyIcon} />
                    <h3>Không tìm thấy tài liệu</h3>
                    <p>Thử thay đổi bộ lọc hoặc tìm kiếm với từ khóa khác</p>
                  </div>
                )}
              </div>
            </TabPane>
          ))}
        </Tabs>
      </div>
    </div>
  );
};
export default DocumentViewer;
