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
  Upload,
  message,
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
  deleteMaterialAPI,
  callCreateLinkMaterialAPI,
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

  // watch type để render form động
  const typeWatch = Form.useWatch("type", form);

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
    url: buildFileUrl(m.fileUrl),
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
      if (!selectedClassId) return;

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
    // eslint-disable-next-line react-hooks/exhaustive-deps
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
    return Upload.LIST_IGNORE; // không upload auto, chỉ chọn local
  };

  const onTypeChange = (value) => {
    setUploadedFile(null);
    setFilePreviewUrl("");
    form.setFieldValue("type", value);
    if (value !== "link") {
      form.setFieldValue("url", undefined);
    }
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
      const type = values.type; // pdf | image | video | link

      if (!values.classId) {
        message.error("Chưa chọn lớp");
        return;
      }

      // ========== TRƯỜNG HỢP LIÊN KẾT ==========
      if (type === "link") {
        // Form đã validate url bắt buộc rồi
        const payload = {
          title: values.title,
          description: values.description || "",
          materialType: "link", // quan trọng
          fileUrl: values.url, // URL người dùng nhập
          fileName: "", // link nên có thể để rỗng
          fileSize: 0, // 0 hoặc null tùy BE, ở đây cho 0
        };

        const hide = message.loading("Đang tạo tài liệu liên kết...", 0);
        const res = await callCreateLinkMaterialAPI(values.classId, payload);
        hide();

        if (res && res.success === true) {
          const apiMaterial = res.data;
          const newItem = mapApiMaterial(apiMaterial);
          setMaterials((prev) => [newItem, ...prev]);

          message.success("Đã thêm tài liệu liên kết");
          setOpenEditor(false);
          setUploadedFile(null);
          setFilePreviewUrl("");
          form.resetFields();
          setCurrent(1);
        } else {
          console.error("Create link material error:", res);
          message.error("Thêm tài liệu liên kết thất bại");
        }
        return;
      }

      // ========== TRƯỜNG HỢP FILE: PDF / IMAGE / VIDEO ==========
      if (!uploadedFile) {
        message.error("Vui lòng chọn file để upload");
        return;
      }

      const formData = new FormData();
      formData.append("Title", values.title);
      formData.append("Description", values.description || "");
      formData.append("MaterialType", type); // cho BE biết loại
      formData.append("File", uploadedFile, uploadedFile.name);

      const hide = message.loading("Đang upload tài liệu...", 0);
      const res = await callUploadMaterialAPI(values.classId, formData);
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
      if (error?.errorFields) return; // lỗi validate form
      message.error("Có lỗi xảy ra khi lưu tài liệu");
    }
  };

  const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

  const handleDelete = (id, classId) => {
    Modal.confirm({
      title: "Xác nhận xóa",
      content: "Bạn có chắc muốn xóa tài liệu này?",
      okText: "Xóa",
      okType: "danger",
      cancelText: "Hủy",
      onOk: async () => {
        try {
          const hide = message.loading("Đang xóa...", 0);
          const res = await deleteMaterialAPI(id, classId);
          await delay(500);
          hide();

          if (res && res.success) {
            setMaterials((prev) => prev.filter((m) => m.id !== id));
            message.success("Đã xóa tài liệu");
          } else {
            message.error("Xóa tài liệu thất bại");
          }
        } catch (err) {
          message.error("Có lỗi xảy ra khi xóa tài liệu");
        }
      },
    });
  };

  const handleDownload = async (row) => {
    try {
      const res = await callDownloadMaterialAPI(row.classId, row.id);

      let blob;

      if (res.data instanceof Blob) {
        blob = res.data;
      } else if (res.request?.response instanceof Blob) {
        blob = res.request.response;
      } else {
        const contentType =
          res.headers && res.headers["content-type"]
            ? res.headers["content-type"]
            : "application/octet-stream";
        blob = new Blob([res.data], { type: contentType });
      }

      let downloadName = row.fileName || `material-${row.id}`;
      const disposition = res.headers && res.headers["content-disposition"];

      if (disposition) {
        let match =
          /filename\*=(?:UTF-8''|)([^;]+)/i.exec(disposition) ||
          /filename="?([^"]+)"?/i.exec(disposition);

        if (match && match[1]) {
          downloadName = decodeURIComponent(match[1].trim());
        }
      }

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
              onClick={() => handleDownload(row)}
              icon={<Download size={16} />}
            >
              Tải
            </Button>
            <Button
              size="small"
              danger
              onClick={() => handleDelete(row.id, row.classId)}
              icon={<Trash2 size={16} />}
            >
              Xóa
            </Button>
          </Space>
        ),
      },
    ],
    [navigate]
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
          {["pdf", "image", "video"].includes(typeWatch) && (
            <Form.Item label="Upload file">
              <Upload
                beforeUpload={beforeUpload}
                showUploadList={false}
                maxCount={1}
              >
                <Button icon={<UploadIcon size={16} />}>Chọn file</Button>
              </Upload>

              {/* Preview PDF */}
              {uploadedFile && typeWatch === "pdf" && (
                <div className={styles.filePreview}>
                  <div className={styles.filePreviewContent}>
                    <FileText className={styles.fileIcon} size={32} />
                    <div className={styles.fileInfo}>
                      <span className={styles.fileName}>
                        {uploadedFile.name}
                      </span>
                      <span className={styles.fileSize}>
                        {(uploadedFile.size / 1024 / 1024).toFixed(2)} MB
                      </span>
                    </div>
                  </div>
                </div>
              )}

              {/* Preview IMAGE */}
              {uploadedFile && typeWatch === "image" && filePreviewUrl && (
                <div className={styles.imagePreview}>
                  <img src={filePreviewUrl} alt={uploadedFile.name} />
                  <div className={styles.imageOverlay}>
                    <span className={styles.imageName}>
                      {uploadedFile.name}
                    </span>
                    <Tag icon={<ImageIcon size={14} />}>
                      {(uploadedFile.size / 1024 / 1024).toFixed(2)} MB
                    </Tag>
                  </div>
                </div>
              )}

              {/* Preview VIDEO */}
              {uploadedFile && typeWatch === "video" && filePreviewUrl && (
                <div className={styles.videoPreview}>
                  <video src={filePreviewUrl} controls />
                  <div className={styles.videoInfo}>
                    <span>{uploadedFile.name}</span>
                    <Tag icon={<VideoIcon size={14} />}>
                      {(uploadedFile.size / 1024 / 1024).toFixed(2)} MB
                    </Tag>
                  </div>
                </div>
              )}

              <Text type="secondary" style={{ display: "block", marginTop: 8 }}>
                {typeWatch === "pdf" && "Chấp nhận file PDF, tối đa 50MB"}
                {typeWatch === "image" &&
                  "Chấp nhận JPG, PNG, GIF, tối đa 50MB"}
                {typeWatch === "video" &&
                  "Chấp nhận MP4, AVI, MOV, tối đa 50MB"}
              </Text>
            </Form.Item>
          )}

          {/* Chỉ loại LIÊN KẾT mới có URL */}
          {typeWatch === "link" && (
            <Form.Item
              label="URL"
              name="url"
              rules={[
                { required: true, message: "Vui lòng nhập URL" },
                () => ({
                  validator(_, value) {
                    if (value && !/^https?:\/\//i.test(value)) {
                      return Promise.reject("URL không hợp lệ");
                    }
                    return Promise.resolve();
                  },
                }),
              ]}
            >
              <Input placeholder="https://..." />
            </Form.Item>
          )}

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
