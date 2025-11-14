import { Badge, Descriptions, Drawer } from "antd";
import moment from "moment";

const UserDetail = (props) => {
  const { userDetail, setUserDetail, isDetailOpen, setIsDetailOpen } = props;

  const handleClose = () => {
    setUserDetail(null);
    setIsDetailOpen(false);
  };

  // Lấy giá trị an toàn cho cả raw API và row đã map
  const id = userDetail?.userId ?? userDetail?.id;
  const fullName = userDetail?.fullName ?? userDetail?.name;
  const email = userDetail?.email ?? "-";

  // role có thể là "Student"/"Teacher" hoặc "student"/"teacher"
  const rawRole = userDetail?.role;
  const normRole =
    typeof rawRole === "string" ? rawRole.toLowerCase() : undefined;

  let roleLabel = rawRole || "-";
  let roleBadgeStatus = "default";

  if (normRole === "student") {
    roleLabel = "Học sinh";
    roleBadgeStatus = "processing";
  } else if (normRole === "teacher") {
    roleLabel = "Giáo viên";
    roleBadgeStatus = "success";
  } else if (normRole === "admin") {
    roleLabel = "Quản trị";
    roleBadgeStatus = "error";
  }

  // isActive hoặc status
  const isActive =
    typeof userDetail?.isActive === "boolean"
      ? userDetail.isActive
      : userDetail?.status === "active";

  const createdAt =
    userDetail?.createdAt || userDetail?.createAt || userDetail?.CreatedAt;
  const updatedAt =
    userDetail?.updatedAt || userDetail?.updateAt || userDetail?.UpdatedAt;

  return (
    <Drawer
      title="Chức năng xem chi tiết"
      onClose={handleClose}
      open={isDetailOpen}
      width="40vw"
    >
      <Descriptions title="Thông tin người dùng" column={2} bordered>
        <Descriptions.Item label="Id">{id}</Descriptions.Item>

        <Descriptions.Item label="Tên hiển thị">{fullName}</Descriptions.Item>

        <Descriptions.Item label="Email">{email}</Descriptions.Item>

        <Descriptions.Item label="Role" span={2}>
          <Badge status={roleBadgeStatus} text={roleLabel} />
        </Descriptions.Item>

        <Descriptions.Item label="Trạng thái" span={2}>
          {isActive ? (
            <Badge status="success" text="Hoạt động" />
          ) : (
            <Badge status="error" text="Đã khóa" />
          )}
        </Descriptions.Item>

        <Descriptions.Item label="Ngày tạo">
          {createdAt ? moment(createdAt).format("DD-MM-YYYY HH:mm:ss") : "-"}
        </Descriptions.Item>

        <Descriptions.Item label="Ngày cập nhật">
          {updatedAt ? moment(updatedAt).format("DD-MM-YYYY HH:mm:ss") : "-"}
        </Descriptions.Item>
      </Descriptions>
    </Drawer>
  );
};

export default UserDetail;
