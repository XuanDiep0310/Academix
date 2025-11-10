import { useMemo, useState } from "react";
import {
  Button,
  Modal,
  Form,
  Input,
  Select,
  Table,
  Tag,
  Typography,
  Space,
  Pagination,
  Upload,
  message,
  Divider,
  Empty,
} from "antd";
import {
  Plus,
  FileText,
  Link as LinkIcon,
  Image as ImageIcon,
  Video as VideoIcon,
  Trash2,
  Eye,
  Download,
  Filter,
  Upload as UploadIcon,
} from "lucide-react";
import styles from "../../assets/styles/MaterialManagement.module.scss";

const { Title, Text } = Typography;

/* ====================== DATASET MẪU NGAY TRONG FILE ====================== */
const CLASSES = ["Toán cao cấp 1", "Đại số tuyến tính", "Giải tích"];

const MATERIAL_LABELS = {
  pdf: "PDF",
  link: "Liên kết",
  image: "Hình ảnh",
  video: "Video",
};

const MATERIAL_ICONS = {
  pdf: FileText,
  link: LinkIcon,
  image: ImageIcon,
  video: VideoIcon,
};

function generateMaterials() {
  const types = ["pdf", "link", "image", "video"];
  const titles = [
    "Bài giảng chương",
    "Video hướng dẫn",
    "Tài liệu tham khảo",
    "Slide trình bày",
    "Hình ảnh minh họa",
    "Link tham khảo",
  ];
  const materials = [];
  for (let i = 1; i <= 50; i++) {
    const type = types[i % types.length];
    const className = CLASSES[i % CLASSES.length];
    materials.push({
      id: String(i),
      title: `${titles[i % titles.length]} ${Math.floor(i / 6) + 1}`,
      type, // "pdf" | "link" | "image" | "video"
      url:
        type === "video"
          ? "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
          : type === "image"
          ? "https://picsum.photos/1200/800"
          : "https://example.com/document.pdf",
      classId: String((i % 3) + 1),
      className,
      description: `Tài liệu học tập cho ${className.toLowerCase()}`,
      uploadedAt: `2024-0${Math.min((i % 3) + 1, 9)}-${String(
        (i % 28) + 1
      ).padStart(2, "0")}`,
    });
  }
  return materials;
}
/* ======================================================================= */

export default function MaterialManagement() {
  // Data local
  const [materials, setMaterials] = useState(() => generateMaterials());

  // UI state
  const [openEditor, setOpenEditor] = useState(false);
  const [viewing, setViewing] = useState(null);

  const [filterClass, setFilterClass] = useState("all");
  const [filterType, setFilterType] = useState("all");

  const [page, setPage] = useState(1);
  const pageSize = 10;

  // Form
  const [form] = Form.useForm();
  const [uploadedFile, setUploadedFile] = useState(null);
  const [filePreviewUrl, setFilePreviewUrl] = useState("");

  /* -------------------------- FILTER & PAGINATION -------------------------- */
  const filtered = useMemo(() => {
    return materials.filter((m) => {
      const byClass = filterClass === "all" || m.className === filterClass;
      const byType = filterType === "all" || m.type === filterType;
      return byClass && byType;
    });
  }, [materials, filterClass, filterType]);

  const total = filtered.length;
  const dataSource = useMemo(() => {
    const start = (page - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, page]);

  const IconOf = (t) => MATERIAL_ICONS[t] || FileText;

  /* ------------------------------ FILE UPLOAD ------------------------------ */
  const beforeUpload = (file) => {
    const type = form.getFieldValue("type");
    if (type === "pdf" && file.type !== "application/pdf") {
      message.error("Vui lòng chọn file PDF");
      return Upload.LIST_IGNORE;
    }
    if (type === "image" && !file.type.startsWith("image/")) {
      message.error("Vui lòng chọn file hình ảnh");
      return Upload.LIST_IGNORE;
    }
    if (type === "video" && !file.type.startsWith("video/")) {
      message.error("Vui lòng chọn file video");
      return Upload.LIST_IGNORE;
    }
    if (file.size > 50 * 1024 * 1024) {
      message.error("Kích thước file không vượt quá 50MB");
      return Upload.LIST_IGNORE;
    }
    // Không upload thật — chỉ tạo preview local
    const url = URL.createObjectURL(file);
    setUploadedFile(file);
    setFilePreviewUrl(url);
    message.success(`Đã chọn file: ${file.name}`);
    return Upload.LIST_IGNORE; // chặn antd tự upload
  };

  const onTypeChange = (value) => {
    // reset file + preview khi đổi loại
    setUploadedFile(null);
    setFilePreviewUrl("");
    form.setFieldValue("type", value);
  };

  /* --------------------------------- CRUD --------------------------------- */
  const openCreate = () => {
    setUploadedFile(null);
    setFilePreviewUrl("");
    form.setFieldsValue({
      classId: "1",
      title: "",
      type: "pdf",
      url: "",
      description: "",
    });
    setOpenEditor(true);
  };

  const handleCreate = async () => {
    const values = await form.validateFields();
    // validate: phải có file hoặc URL (trừ khi type=link thì bắt buộc URL)
    if (values.type !== "link" && !uploadedFile && !values.url) {
      message.error("Vui lòng upload file hoặc nhập URL");
      return;
    }

    const newItem = {
      id: String(Date.now()),
      ...values,
      url: uploadedFile ? filePreviewUrl : values.url,
      className: CLASSES[Number(values.classId) - 1] || CLASSES[0],
      uploadedAt: new Date().toISOString().split("T")[0],
    };
    setMaterials((prev) => [newItem, ...prev]);
    setOpenEditor(false);
    setUploadedFile(null);
    setFilePreviewUrl("");
    form.resetFields();
    setPage(1);
    message.success("Đã thêm tài liệu");
  };

  const handleDelete = (id) => {
    setMaterials((prev) => prev.filter((m) => m.id !== id));
    message.success("Đã xóa tài liệu");
  };

  /* -------------------------------- COLUMNS ------------------------------- */
  const columns = [
    {
      title: "Tiêu đề",
      dataIndex: "title",
      key: "title",
      render: (_t, row) => {
        const Icon = IconOf(row.type);
        return (
          <Space>
            <Icon size={16} />
            <span>{row.title}</span>
          </Space>
        );
      },
    },
    {
      title: "Loại",
      dataIndex: "type",
      key: "type",
      width: 120,
      render: (t) => <Tag>{MATERIAL_LABELS[t] || t}</Tag>,
    },
    { title: "Lớp học", dataIndex: "className", key: "className", width: 180 },
    {
      title: "Mô tả",
      dataIndex: "description",
      key: "description",
      render: (text) => <span className={styles.truncate}>{text}</span>,
    },
    {
      title: "Ngày tải",
      dataIndex: "uploadedAt",
      key: "uploadedAt",
      width: 130,
    },
    {
      title: "Thao tác",
      key: "actions",
      align: "right",
      width: 200,
      render: (_, row) => (
        <Space>
          <Button
            size="small"
            onClick={() => setViewing(row)}
            icon={<Eye size={16} />}
          >
            Xem
          </Button>
          <Button
            size="small"
            onClick={() => window.open(row.url, "_blank")}
            icon={<Download size={16} />}
          >
            Tải
          </Button>
          <Button
            size="small"
            danger
            onClick={() => handleDelete(row.id)}
            icon={<Trash2 size={16} />}
          >
            Xóa
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Tài liệu học tập
          </Title>
          <Text type="secondary">Quản lý tài liệu cho các lớp học</Text>
        </div>

        <Button type="primary" icon={<Plus size={16} />} onClick={openCreate}>
          Thêm tài liệu
        </Button>
      </div>

      {/* Filters */}
      <div className={styles.filters}>
        <Space>
          <Select
            value={filterClass}
            onChange={(v) => {
              setFilterClass(v);
              setPage(1);
            }}
            style={{ width: 220 }}
            suffixIcon={<Filter size={16} />}
            options={[
              { value: "all", label: "Tất cả lớp học" },
              ...CLASSES.map((c) => ({ value: c, label: c })),
            ]}
          />
          <Select
            value={filterType}
            onChange={(v) => {
              setFilterType(v);
              setPage(1);
            }}
            style={{ width: 220 }}
            suffixIcon={<Filter size={16} />}
            options={[
              { value: "all", label: "Tất cả loại" },
              { value: "pdf", label: "PDF" },
              { value: "link", label: "Liên kết" },
              { value: "image", label: "Hình ảnh" },
              { value: "video", label: "Video" },
            ]}
          />
        </Space>
      </div>

      {/* Table */}
      <div className={styles.tableCard}>
        <Table
          rowKey="id"
          dataSource={dataSource}
          columns={columns}
          pagination={false}
          locale={{ emptyText: <Empty description="Chưa có tài liệu" /> }}
        />

        {total > pageSize && (
          <div className={styles.pagination}>
            <Pagination
              current={page}
              pageSize={pageSize}
              total={total}
              showSizeChanger={false}
              onChange={(p) => setPage(p)}
            />
          </div>
        )}
      </div>

      {/* Modal tạo mới */}
      <Modal
        title="Thêm tài liệu mới"
        open={openEditor}
        onCancel={() => setOpenEditor(false)}
        onOk={handleCreate}
        okText="Thêm tài liệu"
        destroyOnClose
      >
        <Form
          layout="vertical"
          form={form}
          initialValues={{
            classId: "1",
            title: "",
            type: "pdf",
            url: "",
            description: "",
          }}
        >
          <Form.Item
            label="Lớp học"
            name="classId"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: "1", label: "Toán cao cấp 1" },
                { value: "2", label: "Đại số tuyến tính" },
                { value: "3", label: "Giải tích" },
              ]}
            />
          </Form.Item>

          <Form.Item
            label="Tiêu đề"
            name="title"
            rules={[{ required: true, message: "Vui lòng nhập tiêu đề" }]}
          >
            <Input placeholder="VD: Bài giảng chương 1" />
          </Form.Item>

          <Form.Item
            label="Loại tài liệu"
            name="type"
            rules={[{ required: true }]}
          >
            <Select
              options={[
                { value: "pdf", label: "PDF" },
                { value: "link", label: "Liên kết" },
                { value: "image", label: "Hình ảnh" },
                { value: "video", label: "Video" },
              ]}
              onChange={onTypeChange}
            />
          </Form.Item>

          {/* Upload file cho pdf/image/video */}
          {["pdf", "image", "video"].includes(form.getFieldValue("type")) && (
            <>
              <Form.Item label="Upload file">
                <Upload
                  beforeUpload={beforeUpload}
                  showUploadList={!!uploadedFile}
                  maxCount={1}
                  itemRender={() =>
                    uploadedFile ? (
                      <Tag icon={<UploadIcon size={14} />}>
                        {uploadedFile.name.length > 24
                          ? uploadedFile.name.slice(0, 24) + "..."
                          : uploadedFile.name}
                      </Tag>
                    ) : null
                  }
                >
                  <Button icon={<UploadIcon size={16} />}>Chọn file</Button>
                </Upload>
                {uploadedFile &&
                  form.getFieldValue("type") === "image" &&
                  filePreviewUrl && (
                    <div className={styles.previewImg}>
                      <img src={filePreviewUrl} alt="Preview" />
                    </div>
                  )}
                <Text
                  type="secondary"
                  style={{ display: "block", marginTop: 8 }}
                >
                  {form.getFieldValue("type") === "pdf" &&
                    "Chấp nhận file PDF, tối đa 50MB"}
                  {form.getFieldValue("type") === "image" &&
                    "Chấp nhận JPG, PNG, GIF, tối đa 50MB"}
                  {form.getFieldValue("type") === "video" &&
                    "Chấp nhận MP4, AVI, MOV, tối đa 50MB"}
                </Text>

                <Divider plain>Hoặc</Divider>
              </Form.Item>
            </>
          )}

          <Form.Item
            label={
              form.getFieldValue("type") === "link"
                ? "URL"
                : "URL tài liệu (tùy chọn)"
            }
            name="url"
            rules={[
              ({ getFieldValue }) => ({
                validator(_, value) {
                  const t = getFieldValue("type");
                  if (t === "link" && !value)
                    return Promise.reject("Vui lòng nhập URL");
                  if (t !== "link" && !uploadedFile && !value)
                    return Promise.reject("Nhập URL hoặc chọn file");
                  if (value && !/^https?:\/\//i.test(value))
                    return Promise.reject("URL không hợp lệ");
                  return Promise.resolve();
                },
              }),
            ]}
          >
            <Input placeholder="https://..." />
          </Form.Item>

          <Form.Item label="Mô tả" name="description">
            <Input.TextArea rows={3} placeholder="Mô tả ngắn..." />
          </Form.Item>
        </Form>
      </Modal>

      {/* Modal xem tài liệu */}
      <Modal
        title={
          viewing ? (
            <Space direction="vertical" size={0}>
              <Text strong>{viewing.title}</Text>
              <Text type="secondary">
                {viewing.className} — {MATERIAL_LABELS[viewing.type]}
              </Text>
            </Space>
          ) : (
            ""
          )
        }
        open={!!viewing}
        onCancel={() => setViewing(null)}
        footer={null}
        width={960}
        destroyOnClose
      >
        {viewing && (
          <div className={styles.viewer}>
            {viewing.type === "pdf" && (
              <iframe
                src={viewing.url}
                title={viewing.title}
                className={styles.pdf}
              />
            )}

            {viewing.type === "video" && (
              <div className={styles.video}>
                <iframe
                  src={viewing.url.replace("watch?v=", "embed/")}
                  title={viewing.title}
                  allowFullScreen
                />
              </div>
            )}

            {viewing.type === "image" && (
              <div className={styles.image}>
                <img src={viewing.url} alt={viewing.title} />
              </div>
            )}

            {viewing.type === "link" && (
              <div className={styles.linkWrap}>
                <Text type="secondary">Tài liệu liên kết bên ngoài</Text>
                <Button type="primary">
                  <a href={viewing.url} target="_blank" rel="noreferrer">
                    Mở liên kết
                  </a>
                </Button>
              </div>
            )}

            <div className={styles.desc}>
              <Text type="secondary">{viewing.description}</Text>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
