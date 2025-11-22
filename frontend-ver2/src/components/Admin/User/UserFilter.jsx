import { Input, Button, Space, Typography } from "antd";
import { Users, Plus } from "lucide-react";
import styles from "../../../assets/styles/UserManagement.module.scss";

const { Title, Text } = Typography;

const UserFilter = (props) => {
  const { q, setQ, setCurrent, onExport, onImport, onAddStudent, onAddTeacher } = props;
  return (
    <div className={styles.header}>
      <div className={styles.headerLeft}>
        <Title level={4} className={styles.title}>
          Quản lý tài khoản
        </Title>
        <Text type="secondary">Quản lý tài khoản giáo viên và học sinh</Text>
      </div>

      <Space wrap>
        <Input
          allowClear
          placeholder="Tìm theo tên/email/role/status..."
          value={q}
          onChange={(e) => {
            setQ(e.target.value);
            setCurrent(1);
          }}
          style={{ width: 280 }}
        />
        <Button onClick={onExport}>Xuất Excel</Button>

        <Button onClick={onImport}>Thêm bằng Excel</Button>
        <Button icon={<Users size={16} />} onClick={onAddStudent}>
          Thêm học sinh
        </Button>

        <Button type="primary" icon={<Plus size={16} />} onClick={onAddTeacher}>
          Thêm giáo viên
        </Button>
      </Space>
    </div>
  );
}
export default UserFilter;
