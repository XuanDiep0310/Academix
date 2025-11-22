import React, { useEffect, useState } from "react";
import { Drawer, Space, Typography, Button, Checkbox, message, notification } from "antd";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import {
  callListTeacherAPI,
  callListTeacherOnClassesAPI,
  callAddTeachersToClassAPI,
} from "../../../services/api.service";

const { Text } = Typography;
const MAX_TEACHERS = 2;

const TeacherPickerDrawer = (props) => {
    const { open, onClose, classId, onSuccess } = props;
  const [allTeachers, setAllTeachers] = useState([]);
  const [selectedTeachers, setSelectedTeachers] = useState([]);
  const [baseTeacherIds, setBaseTeacherIds] = useState([]);
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
      const [allRes, classRes] = await Promise.all([
        callListTeacherAPI(),
        callListTeacherOnClassesAPI(classId),
      ]);

      if (allRes && allRes.success) {
        setAllTeachers(
          allRes.data.users?.map((u) => ({
            id: u.userId,
            name: u.fullName,
            email: u.email,
          })) || []
        );
      }

      if (classRes && classRes.success) {
        const currentIds = classRes.data?.map((t) => String(t.userId)) || [];
        setSelectedTeachers(currentIds);
        setBaseTeacherIds(currentIds);
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
    if (baseTeacherIds.includes(s)) return;

    setSelectedTeachers((prev) => {
      if (prev.includes(s)) {
        return prev.filter((x) => x !== s);
      }
      if (prev.length >= MAX_TEACHERS) {
        message.error(`Tối đa ${MAX_TEACHERS} giáo viên`);
        return prev;
      }
      return [...prev, s];
    });
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      const res = await callAddTeachersToClassAPI(classId, selectedTeachers);
      if (res && res.success === true) {
        message.success("Đã cập nhật giáo viên");
        if (onSuccess) onSuccess();
        onClose();
      } else {
        notification.error({
          message: "Lưu thất bại",
          description: res?.message,
        });
      }
    } catch (error) {
      console.error("Save teachers error:", error);
      message.error("Lỗi khi lưu");
    } finally {
      setSaving(false);
    }
  };
  return (
    <Drawer
      title="Thêm giáo viên"
      open={open}
      onClose={onClose}
      extra={
        <Space>
          <Text type="secondary">
            Đã chọn: {selectedTeachers.length}/{MAX_TEACHERS}
          </Text>
          <Button type="primary" onClick={handleSave} loading={saving}>
            Lưu thay đổi
          </Button>
        </Space>
      }
      width={420}
    >
      <div className={styles.pickList}>
        {allTeachers.map((t) => {
          const idStr = String(t.id);
          const checked = selectedTeachers.includes(idStr);
          const isBase = baseTeacherIds.includes(idStr);

          const disabled =
            loading ||
            saving ||
            isBase ||
            (selectedTeachers.length >= MAX_TEACHERS && !checked);

          return (
            <label
              key={t.id}
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
                onChange={() => handleToggle(t.id)}
              />
              <div className={styles.pickMeta}>
                <span>{t.name}</span>
                <small>{t.email}</small>
              </div>
            </label>
          );
        })}
      </div>
    </Drawer>
  );
}
export default TeacherPickerDrawer;
