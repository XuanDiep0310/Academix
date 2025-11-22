import React from "react";
import { Input, Button, Space, Typography } from "antd";
import { Plus } from "lucide-react";
import styles from "../../../assets/styles/ClassManagement.module.scss";

const { Title, Text } = Typography;

export default function ClassFilter({ q, setQ, setCurrent, onOpenCreate }) {
  return (
    <div className={styles.header}>
      <div>
        <Title level={4} className={styles.title}>
          Quản lý lớp học
        </Title>
        <Text type="secondary">Quản lý tài khoản lớp học</Text>
      </div>

      <Space>
        <Input
          allowClear
          placeholder="Tìm kiếm tên/mã lớp..."
          value={q}
          onChange={(e) => {
            setQ(e.target.value);
            setCurrent(1);
          }}
          style={{ width: 260 }}
        />
        <Button
          type="primary"
          icon={<Plus size={16} />}
          onClick={onOpenCreate}
          className={styles.createBtn}
        >
          Tạo lớp học
        </Button>
      </Space>
    </div>
  );
}
