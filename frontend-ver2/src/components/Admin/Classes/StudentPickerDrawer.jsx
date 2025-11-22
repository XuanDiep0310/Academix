import  { useEffect, useState } from "react";
import { Drawer, Space, Typography, Button, Checkbox, message, notification } from "antd";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import {
  callListStudentAPI,
  callListStudentOnClassesAPI,
  callAddStudentsToClassAPI,
} from "../../../services/api.service";

const { Text } = Typography;
const MAX_STUDENTS = 100;

const StudentPickerDrawer = (props) => {
  const { open, onClose, classId, onSuccess } = props;
  const [allStudents, setAllStudents] = useState([]);
  const [selectedStudents, setSelectedStudents] = useState([]);
  const [baseStudentIds, setBaseStudentIds] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (open && classId) {
      fetchData();
    }
  }, [open, classId]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const query = `page=1&pageSize=1000&sortBy=CreatedAt&sortOrder=desc`;
      const [allRes, classRes] = await Promise.all([
        callListStudentAPI(query),
        callListStudentOnClassesAPI(classId),
      ]);

      if (allRes && allRes.success) {
        setAllStudents(
          allRes.data.users?.map((u) => ({
            id: u.userId,
            name: u.fullName,
            email: u.email,
          })) || []
        );
      }

      if (classRes && classRes.success) {
        const currentIds = classRes.data?.map((s) => String(s.userId)) || [];
        setSelectedStudents(currentIds);
        setBaseStudentIds(currentIds);
      }
    } catch (error) {
      console.error("Fetch picker data error:", error);
      message.error("Lỗi tải dữ liệu");
    } finally {
      setLoading(false);
    }
  };

  const handleToggle = (id) => {
    const s = String(id);
    if (baseStudentIds.includes(s)) return;

    setSelectedStudents((prev) => {
      if (prev.includes(s)) {
        return prev.filter((x) => x !== s);
      }
      if (prev.length >= MAX_STUDENTS) {
        message.error(`Tối đa ${MAX_STUDENTS} học sinh`);
        return prev;
      }
      return [...prev, s];
    });
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      const res = await callAddStudentsToClassAPI(classId, selectedStudents);
      if (res && res.success === true) {
        message.success("Đã cập nhật học sinh");
        if (onSuccess) onSuccess();
        onClose();
      } else {
        notification.error({
          message: "Lưu thất bại",
          description: res?.message,
        });
      }
    } catch (error) {
      console.error("Save students error:", error);
      message.error("Lỗi khi lưu");
    } finally {
      setSaving(false);
    }
  };
  return (
    <Drawer
      title="Thêm học sinh"
      open={open}
      onClose={onClose}
      extra={
        <Space>
          <Text type="secondary">
            Đã chọn: {selectedStudents.length}/{MAX_STUDENTS}
          </Text>
          <Button type="primary" onClick={handleSave} loading={saving}>
            Lưu thay đổi
          </Button>
        </Space>
      }
      width={420}
    >
      <div className={styles.pickList}>
        {allStudents.map((s) => {
          const idStr = String(s.id);
          const checked = selectedStudents.includes(idStr);
          const isBase = baseStudentIds.includes(idStr);

          const disabled =
            loading ||
            saving ||
            isBase ||
            (selectedStudents.length >= MAX_STUDENTS && !checked);

          return (
            <label
              key={s.id}
              className={`${styles.pickRow} ${
                checked
                  ? isBase
                    ? styles.pickRowBase
                    : styles.pickRowSelected
                  : ""
              }`}
            >
              <Checkbox
                checked={checked}
                disabled={disabled}
                onChange={() => handleToggle(s.id)}
              />
              <div className={styles.pickMeta}>
                <span>{s.name}</span>
                <small>{s.email}</small>
              </div>
            </label>
          );
        })}
      </div>
    </Drawer>
  );
}
export default StudentPickerDrawer;