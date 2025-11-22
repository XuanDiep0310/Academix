import { Table, Tag, Badge, Space, Button, Popconfirm, Typography, Empty } from "antd";
import { Pencil, Trash2, Users, UserPlus } from "lucide-react";

const { Text } = Typography;
const MAX_TEACHERS = 2;
const MAX_STUDENTS = 100;

const ClassTable   = (props) => {
  const { classes, loading, pagination, onChange, onEdit, onDelete, onOpenMembers, onOpenTeacherDrawer, onOpenStudentDrawer } = props;
  const columns = [
    {
      title: "Tên lớp",
      dataIndex: "name",
      key: "name",
      render: (text) => <Text strong>{text}</Text>,
    },
    {
      title: "Mã lớp",
      dataIndex: "code",
      key: "code",
      render: (code) => <Tag>{code}</Tag>,
      width: 120,
    },
    {
      title: "Giáo viên",
      key: "teachers",
      render: (_, row) => (
        <Space>
          <Badge count={`${row.teacherCountApi}/${MAX_TEACHERS}`} />
          <Button
            type="text"
            size="small"
            onClick={() => onOpenTeacherDrawer(row)}
            icon={<UserPlus size={16} />}
          >
            Thêm
          </Button>
        </Space>
      ),
    },
    {
      title: "Học sinh",
      key: "students",
      render: (_, row) => (
        <Space>
          <Badge count={`${row.studentCountApi}/${MAX_STUDENTS}`} />
          <Button
            type="text"
            size="small"
            onClick={() => onOpenStudentDrawer(row)}
            icon={<UserPlus size={16} />}
          >
            Thêm
          </Button>
        </Space>
      ),
    },
    {
      title: "Ngày tạo",
      dataIndex: "createdAt",
      key: "createdAt",
      width: 180,
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
      render: (_, row) => (
        <Space>
          <Button
            size="small"
            type="default"
            icon={<Users size={16} />}
            onClick={() => onOpenMembers(row)}
          />
          <Button
            size="small"
            type="primary"
            ghost
            icon={<Pencil size={16} />}
            onClick={() => onEdit(row)}
          />
          <Popconfirm
            title="Xóa lớp học?"
            okText="Xóa"
            cancelText="Hủy"
            onConfirm={() => onDelete(row.id)}
          >
            <Button size="small" danger icon={<Trash2 size={16} />} />
          </Popconfirm>
        </Space>
      ),
      width: 160,
    },
  ];

  return (
    <Table
      rowKey="id"
      dataSource={classes}
      columns={columns}
      loading={{
        spinning: loading,
        tip: "Đang tải danh sách lớp học...",
      }}
      locale={{ emptyText: <Empty description="Chưa có dữ liệu" /> }}
      onChange={onChange}
      pagination={{
        current: pagination.current,
        pageSize: pagination.pageSize,
        total: pagination.total,
        showSizeChanger: true,
        pageSizeOptions: [5, 10, 20, 50],
        showTotal: (total, range) =>
          `${range[0]}-${range[1]} trên ${total} lớp học`,
      }}
      scroll={{ x: 900 }}
      size="middle"
      sticky
    />
  );
}
export default ClassTable;