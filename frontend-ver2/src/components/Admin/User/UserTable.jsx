
import { Table, Tag, Badge, Space, Button, Popconfirm, Empty } from "antd";
import { Pencil, Lock, Unlock, Trash2 } from "lucide-react";
import moment from "moment";

const UserTable = (props) => {
  const { users, loading, pagination, onChange, onEdit, onDelete, onToggleStatus, onViewDetail } = props;
  const columns = [
    { title: "Họ tên", dataIndex: "name", key: "name" },
    { title: "Email", dataIndex: "email", key: "email" },
    {
      title: "Vai trò",
      dataIndex: "role",
      key: "role",
      render: (role) => {
        const roleMap = {
          teacher: { label: "Giáo viên", color: "geekblue" },
          student: { label: "Học sinh", color: "green" },
          admin: { label: "Quản trị", color: "volcano" },
        };

        const r = roleMap[role] || { label: role, color: "default" };

        return <Tag color={r.color}>{r.label}</Tag>;
      },
      width: 130,
    },
    {
      title: "Trạng thái",
      dataIndex: "status",
      key: "status",
      render: (st) =>
        st === "active" ? (
          <Badge status="success" text="Hoạt động" />
        ) : (
          <Badge status="error" text="Đã khóa" />
        ),
      width: 140,
    },
    {
      title: "Ngày tạo",
      dataIndex: "createdAt",
      key: "createdAt",
      render: (_, row) => moment(row.createdAt).format("DD-MM-YYYY"),
      width: 140,
    },
    {
      title: "Thao tác",
      key: "actions",
      align: "right",
      width: 300,
      render: (_, row) => (
        <Space>
          {/* Chi tiết */}
          <Button
            size="small"
            type="default"
            onClick={() => onViewDetail(row)}
          >
            Chi tiết
          </Button>

          {/* Sửa (GV + HS luôn) */}
          <Button
            size="small"
            type="primary"
            ghost
            icon={<Pencil size={16} />}
            onClick={() => onEdit(row)}
          >
            Sửa
          </Button>

          <Button
            size="small"
            onClick={() => onToggleStatus(row)}
            icon={
              row.status === "active" ? (
                <Lock size={16} />
              ) : (
                <Unlock size={16} />
              )
            }
          >
            {row.status === "active" ? "Khóa" : "Mở khóa"}
          </Button>

          <Popconfirm
            title={
              <>
                Xóa tài khoản <strong>{row.name}</strong>?
              </>
            }
            okText="Xóa"
            cancelText="Hủy"
            onConfirm={() => onDelete(row.id)}
          >
            <Button size="small" danger icon={<Trash2 size={16} />}>
              Xóa
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <Table
      rowKey="id"
      dataSource={users}
      columns={columns}
      loading={{
        spinning: loading,
        tip: "Đang tải danh sách tài khoản...",
      }}
      locale={{ emptyText: <Empty description="Chưa có người dùng" /> }}
      onChange={onChange}
      pagination={{
        current: pagination.current,
        pageSize: pagination.pageSize,
        total: pagination.total,
        showSizeChanger: true,
        pageSizeOptions: [5, 10, 20, 50],
        showTotal: (total, range) =>
          `${range[0]}-${range[1]} trên ${total} tài khoản`,
      }}
      scroll={{ x: 900 }}
      size="middle"
      sticky
    />
  );
}
export default UserTable;