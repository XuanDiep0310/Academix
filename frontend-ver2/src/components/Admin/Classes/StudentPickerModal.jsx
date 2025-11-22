import React, { useEffect, useState } from "react";
import { Modal, Checkbox, message, notification } from "antd";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import {
  callListStudentAPI,
  callListStudentOnClassesAPI,
  callAddStudentsToClassAPI,
} from "../../../services/api.service";

const MAX_STUDENTS = 100;

const StudentPickerModal = (props) =>{
  const { open, onCancel, classId, onSuccess } = props;
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
      // Fetch all students (limit 1000 for now or implement search later)
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
        onCancel();
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
    <Modal
      title="Chọn học sinh cho lớp"
      open={open}
      onCancel={onCancel}
      onOk={handleSave}
      okText="Lưu thay đổi"
      cancelText="Đóng"
      confirmLoading={saving}
      zIndex={1100}
      destroyOnClose
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
              style={loading || saving ? { opacity: 0.7 } : undefined}
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
    </Modal>
  );
}
export default StudentPickerModal;
