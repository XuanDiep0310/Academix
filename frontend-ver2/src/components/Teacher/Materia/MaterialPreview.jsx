import { useLocation, useNavigate } from "react-router";
import { Button, Typography } from "antd";

const { Title, Text } = Typography;

export default function MaterialPreview() {
  //   const { id } = useParams();
  const navigate = useNavigate();
  const location = useLocation();

  // Material được truyền từ trang list qua state
  const material = location.state?.material;

  // Nếu F5 trang mà không có state -> quay lại list
  if (!material) {
    navigate("/teacher/materials");
    return null;
  }

  return (
    <div style={{ padding: 24 }}>
      <Button onClick={() => navigate(-1)} style={{ marginBottom: 16 }}>
        ← Quay lại
      </Button>

      <Title level={4}>{material.title}</Title>
      <Text type="secondary">
        {material.className} — {material.type.toUpperCase()}
      </Text>

      <div style={{ marginTop: 16 }}>
        {material.type === "pdf" && (
          <iframe
            src={material.url}
            title={material.title}
            style={{ width: "100%", height: "80vh", border: "none" }}
          />
        )}

        {material.type === "image" && (
          <div style={{ textAlign: "center" }}>
            <img
              src={material.url}
              alt={material.title}
              style={{ maxWidth: "100%", maxHeight: "80vh" }}
            />
          </div>
        )}

        {material.type === "video" && (
          <div style={{ textAlign: "center" }}>
            <video
              src={material.url}
              style={{ maxWidth: "100%", maxHeight: "80vh" }}
              controls
            />
          </div>
        )}

        {material.type === "link" && (
          <div style={{ marginTop: 16 }}>
            <Text type="secondary">Tài liệu liên kết bên ngoài</Text>
            <br />
            <a href={material.url} target="_blank" rel="noreferrer">
              Mở liên kết
            </a>
          </div>
        )}
      </div>

      {material.description && (
        <div style={{ marginTop: 16 }}>
          <Text>{material.description}</Text>
        </div>
      )}
    </div>
  );
}
