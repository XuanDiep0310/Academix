import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router";
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
import styles from "../../../assets/styles/MaterialManagement.module.scss";
import {
  callListMyClassesAPI,
  callListMaterialsByClassAPI,
  callDownloadMaterialAPI,
  callUploadMaterialAPI,
} from "../../../services/api.service";

const { Title, Text } = Typography;

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

const IconOf = (t) => MATERIAL_ICONS[t] || FileText;
const API_BASE_URL = import.meta.env.VITE_BACKEND_URL || "";
const buildFileUrl = (path) => {
  if (!path) return "";
  if (path.startsWith("http://") || path.startsWith("https://")) return path;
  return `${API_BASE_URL}${path}`;
};
export default function MaterialManagement() {
  const [classes, setClasses] = useState([]);
  const [loadingClasses, setLoadingClasses] = useState(false);

  const [materials, setMaterials] = useState([]);
  const [loadingMaterials, setLoadingMaterials] = useState(false);

  const [selectedClassId, setSelectedClassId] = useState(null);

  const [filterType, setFilterType] = useState("all");
  const [search, setSearch] = useState("");
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [openEditor, setOpenEditor] = useState(false);
  const [viewing, setViewing] = useState(null);

  const [form] = Form.useForm();
  const [uploadedFile, setUploadedFile] = useState(null);
  const [filePreviewUrl, setFilePreviewUrl] = useState("");
  const navigate = useNavigate();

  /* ====================== GỌI API LỚP CỦA TÔI ====================== */
  const fetchMyClasses = async () => {
    try {
      setLoadingClasses(true);
      const res = await callListMyClassesAPI();

      if (res && res.success && Array.isArray(res.data)) {
        const mapped = res.data.map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
        }));
        setClasses(mapped);

        if (!selectedClassId && mapped.length > 0) {
          setSelectedClassId(mapped[0].id);
        }
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchMyClasses error:", err);
      message.error("Có lỗi xảy ra khi tải danh sách lớp học");
    } finally {
      setLoadingClasses(false);
    }
  };
  useEffect(() => {
    fetchMyClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
  const mapApiMaterial = (m) => ({
    id: m.materialId,
    title: m.title,
    type: (m.materialType || "").toLowerCase(),
    url: buildFileUrl(m.fileUrl), // 👈 GHÉP URL Ở ĐÂY
    classId: m.classId,
    className: m.className,
    description: m.description,
    uploadedAt: m.createdAt,

    fileName: m.fileName,
    fileSize: m.fileSize,
    fileSizeFormatted: m.fileSizeFormatted,
    uploadedBy: m.uploadedBy,
    uploadedByName: m.uploadedByName,
  });
  const fetchMaterials = async () => {
    try {
      setLoadingMaterials(true);

      const qs = new URLSearchParams();
      qs.set("page", String(current));
      qs.set("pageSize", String(pageSize));
      qs.set("sortBy", "CreatedAt");
      qs.set("sortOrder", "desc");
      if (filterType !== "all") qs.set("type", filterType);
      if (search.trim()) qs.set("search", search.trim());

      const res = await callListMaterialsByClassAPI(
        selectedClassId,
        qs.toString()
      );

      if (res && res.success === true) {
        const api = res.data;

        const mapped = api.materials?.map(mapApiMaterial) || [];
        setMaterials(mapped);
        setTotal(api.totalCount ?? mapped.length);
      } else {
        message.error("Không thể tải danh sách tài liệu");
      }
    } catch (err) {
      console.error("fetchMaterials error:", err);
      message.error("Có lỗi xảy ra khi tải tài liệu");
    } finally {
      setLoadingMaterials(false);
    }
  };
  useEffect(() => {
    if (!selectedClassId) return;

    fetchMaterials();
  }, [selectedClassId, current, pageSize, filterType, search]);

  const handleOnChangePagi = (pagination) => {
    if (
      pagination &&
      pagination.pageSize &&
      +pagination.pageSize !== +pageSize
    ) {
      setPageSize(+pagination.pageSize);
      setCurrent(1);
    }

    if (pagination && pagination.current && +pagination.current !== +current) {
      setCurrent(+pagination.current);
    }
  };

  /* ====================== UPLOAD & FORM LOGIC (LOCAL) ====================== */
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
    const url = URL.createObjectURL(file);
    setUploadedFile(file);
    setFilePreviewUrl(url);
    message.success(`Đã chọn file: ${file.name}`);
    return Upload.LIST_IGNORE;
  };

  const onTypeChange = (value) => {
    setUploadedFile(null);
    setFilePreviewUrl("");
    form.setFieldValue("type", value);
  };

  const openCreate = () => {
    if (!selectedClassId) {
      message.warning("Vui lòng chọn lớp trước khi thêm tài liệu");
      return;
    }

    setUploadedFile(null);
    setFilePreviewUrl("");
    form.setFieldsValue({
      classId: selectedClassId,
      title: "",
      type: "pdf",
      url: "",
      description: "",
    });
    setOpenEditor(true);
  };

  const handleCreate = async () => {
    try {
      const values = await form.validateFields();

      const type = values.type;

      // ---- TRƯỜNG HỢP LINK: hiện tại chưa có API riêng, bạn có thể xử lý sau ----
      if (type === "link") {
        const classInfo = classes.find((c) => c.id === values.classId);

        const newItem = {
          id: Date.now().toString(),
          title: values.title,
          type: "link",
          url: values.url, // dùng URL người dùng nhập
          classId: values.classId,
          className: classInfo?.name || "",
          description: values.description,
          uploadedAt: new Date().toISOString(),
        };

        setMaterials((prev) => [newItem, ...prev]);
        setOpenEditor(false);
        form.resetFields();
        setCurrent(1);
        message.success(
          "Đã thêm tài liệu liên kết (local – hãy nối API link sau)"
        );
        return;
      }

      // ---- TRƯỜNG HỢP FILE (pdf / image / video) → upload multipart như WinForms ----
      if (!uploadedFile) {
        message.error("Vui lòng chọn file để upload");
        return;
      }

      if (!values.classId) {
        message.error("Chưa chọn lớp");
        return;
      }

      const formData = new FormData();
      formData.append("Title", values.title);
      formData.append("Description", values.description || "");
      formData.append("File", uploadedFile, uploadedFile.name);

      const hide = message.loading("Đang upload tài liệu...", 0);

      const res = await callUploadMaterialAPI(values.classId, formData);
      console.log(res);

      hide();

      if (res && res.success === true) {
        const apiMaterial = res.data;
        const newItem = mapApiMaterial(apiMaterial);
        setMaterials((prev) => [newItem, ...prev]);

        message.success("Upload tài liệu thành công");

        setOpenEditor(false);
        setUploadedFile(null);
        setFilePreviewUrl("");
        form.resetFields();
        setCurrent(1);
      } else {
        console.error("Upload error:", res);
        message.error("Upload tài liệu thất bại");
      }
    } catch (error) {
      console.error("handleCreate error:", error);
      message.destroy();
      // Nếu là lỗi validate của Form thì không cần báo thêm
      if (error?.errorFields) return;
      message.error("Có lỗi xảy ra khi upload tài liệu");
    }
  };

  const handleDelete = (id) => {
    // Tạm thời xóa local
    setMaterials((prev) => prev.filter((m) => m.id !== id));
    message.success("Đã xóa tài liệu (local)");
  };
  const handleDownload = async (id, classId, fileName) => {
    try {
      const res = await callDownloadMaterialAPI(classId, id);

      // Nếu backend trả lỗi (404, 500, ...) thì axios sẽ throw, nên chỉ cần check thêm cho chắc
      if (!res || res.status !== 200) {
        message.error("Không thể tải file (trạng thái không thành công)");
        return;
      }

      // bytes → Blob
      const blob = new Blob([res.data]);

      // Lấy tên file ưu tiên từ header nếu có
      let downloadName = fileName || `material-${id}`;
      const disposition = res.headers["content-disposition"];
      if (disposition) {
        const match = /filename\*?=(?:UTF-8''|")?([^";]+)/i.exec(disposition);
        if (match && match[1]) {
          downloadName = decodeURIComponent(match[1]);
        }
      }

      // Tạo link ảo để browser bật hộp thoại lưu file
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = downloadName;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error("Download material error:", err);
      message.error("Có lỗi xảy ra khi tải file");
    }
  };

  const columns = useMemo(
    () => [
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
      {
        title: "Lớp học",
        dataIndex: "className",
        key: "className",
        width: 200,
      },
      {
        title: "Mô tả",
        dataIndex: "description",
        key: "description",
        // giới hạn chiều rộng + tự chấm ...
        width: 400,
        ellipsis: true,
        render: (text) => (
          <span className={styles.descriptionCell}>{text}</span>
        ),
      },
      {
        title: "Ngày tải",
        dataIndex: "uploadedAt",
        key: "uploadedAt",
        width: 170,
        render: (val) => (
          <Text type="secondary">
            {val ? new Date(val).toLocaleString() : "--"}
          </Text>
        ),
      },
      {
        title: "Thao tác",
        key: "actions",
        align: "right",
        width: 220,
        render: (_, row) => (
          <Space>
            <Button
              size="small"
              // onClick={() => setViewing(row)}
              onClick={() =>
                navigate(`/teacher/materials/${row.id}`, {
                  state: { material: row },
                })
              }
              icon={<Eye size={16} />}
            >
              Xem
            </Button>
            <Button
              size="small"
              onClick={() => handleDownload(row.id, row.classId, row.fileName)}
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
    ],
    []
  );

  /* ====================== RENDER ====================== */
  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <div>
          <Title level={4} className={styles.title}>
            Tài liệu học tập
          </Title>
          <Text type="secondary">Quản lý tài liệu cho các lớp bạn dạy</Text>
        </div>

        <Button
          type="primary"
          icon={<Plus size={16} />}
          onClick={openCreate}
          disabled={!selectedClassId}
        >
          Thêm tài liệu
        </Button>
      </div>

      {/* Filters */}
      <div className={styles.filters}>
        <Space wrap>
          <Select
            loading={loadingClasses}
            value={selectedClassId ?? undefined}
            onChange={(v) => {
              setSelectedClassId(v);
              setCurrent(1);
            }}
            placeholder="Chọn lớp"
            style={{ width: 260 }}
            suffixIcon={<Filter size={16} />}
            options={classes.map((c) => ({
              value: c.id,
              label: `${c.name} (${c.code})`,
            }))}
          />

          <Select
            value={filterType}
            onChange={(v) => {
              setFilterType(v);
              setCurrent(1);
            }}
            style={{ width: 200 }}
            suffixIcon={<Filter size={16} />}
            options={[
              { value: "all", label: "Tất cả loại" },
              { value: "pdf", label: "PDF" },
              { value: "link", label: "Liên kết" },
              { value: "image", label: "Hình ảnh" },
              { value: "video", label: "Video" },
            ]}
          />

          <Input.Search
            allowClear
            placeholder="Tìm theo tiêu đề/mô tả..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onSearch={() => setCurrent(1)}
            style={{ width: 260 }}
          />
        </Space>
      </div>

      {/* Table */}
      <div className={styles.tableCard}>
        <Table
          rowKey="id"
          dataSource={materials}
          columns={columns}
          loading={{
            spinning: loadingMaterials,
            tip: "Đang tải danh sách tài liệu...",
          }}
          locale={{ emptyText: <Empty description="Chưa có tài liệu" /> }}
          pagination={{
            current,
            pageSize,
            total,
            showSizeChanger: true,
            pageSizeOptions: [5, 10, 20, 50],
            showTotal: (t, range) =>
              `${range[0]}-${range[1]} trên ${t} tài liệu`,
          }}
          onChange={handleOnChangePagi}
          size="middle"
          scroll={{ x: 900 }}
          sticky
        />
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
            classId: selectedClassId ?? undefined,
            title: "",
            type: "pdf",
            url: "",
            description: "",
          }}
        >
          <Form.Item
            label="Lớp học"
            name="classId"
            rules={[{ required: true, message: "Vui lòng chọn lớp" }]}
          >
            <Select
              options={classes.map((c) => ({
                value: c.id,
                label: `${c.name} (${c.code})`,
              }))}
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

                  // Nếu là link → bắt buộc URL
                  if (t === "link") {
                    if (!value) return Promise.reject("Vui lòng nhập URL");
                    if (value && !/^https?:\/\//i.test(value))
                      return Promise.reject("URL không hợp lệ");
                    return Promise.resolve();
                  }

                  // Nếu là pdf/image/video → URL là tùy chọn
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

            {viewing.type === "image" && (
              <div className={styles.image}>
                <img src={viewing.url} alt={viewing.title} />
              </div>
            )}

            {viewing.type === "video" && (
              <div className={styles.video}>
                <video src={viewing.url} controls />
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
          </div>
        )}
      </Modal>
    </div>
  );
}
