import { useEffect, useMemo, useState } from "react";
import { message } from "antd";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import { callListClassAPI, deleteClassAPI } from "../../../services/api.service";

import ClassFilter from "./ClassFilter";
import ClassTable from "./ClassTable";
import ClassEditModal from "./ClassEditModal";
import ClassMembersModal from "./ClassMembersModal";
import TeacherPickerDrawer from "./TeacherPickerDrawer";
import StudentPickerDrawer from "./StudentPickerDrawer";

export default function ClassManagement() {
  const [classes, setClasses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [q, setQ] = useState("");
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);

  const [openEditor, setOpenEditor] = useState(false);
  const [editingClass, setEditingClass] = useState(null);

  const [openMembers, setOpenMembers] = useState(false);
  const [managingClass, setManagingClass] = useState(null);

  const [openTeacherDrawer, setOpenTeacherDrawer] = useState(false);
  const [openStudentDrawer, setOpenStudentDrawer] = useState(false);
  const [pickerClass, setPickerClass] = useState(null);

  const fetchClasses = async () => {
    try {
      setLoading(true);
      const query = `page=${current}&pageSize=${pageSize}&sortBy=CreatedAt&sortOrder=desc`;
      const res = await callListClassAPI(query);
      if (res && res.success === true) {
        const apiData = res.data;
        const mapped = (apiData.classes || []).map((c) => ({
          id: c.classId,
          name: c.className,
          code: c.classCode,
          description: c.description,
          createdBy: c.createdBy,
          createdByName: c.createdByName,
          isActive: c.isActive,
          createdAt: c.createdAt,
          updatedAt: c.updatedAt,
          teacherCountApi: c.teacherCount,
          studentCountApi: c.studentCount,
        }));

        setClasses(mapped);
        setTotal(apiData.totalCount || mapped.length);
      } else {
        message.error("Không thể tải danh sách lớp học");
      }
    } catch (err) {
      console.error("fetchClasses error:", err);
      message.error("Có lỗi xảy ra khi tải danh sách lớp học");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [current, pageSize]);

  const filtered = useMemo(() => {
    if (!q?.trim()) return classes;
    const key = q.toLowerCase();
    return classes.filter(
      (c) =>
        c.name?.toLowerCase().includes(key) ||
        c.code?.toLowerCase().includes(key) ||
        c.description?.toLowerCase().includes(key)
    );
  }, [classes, q]);

  const openCreate = () => {
    setEditingClass(null);
    setOpenEditor(true);
  };

  const openEdit = (row) => {
    setEditingClass(row);
    setOpenEditor(true);
  };

  const handleDelete = async (id) => {
    try {
      const res = await deleteClassAPI(id);
      if (res && res.success === true) {
        message.success("Đã xóa lớp học");
        fetchClasses();
      } else {
        message.error(res.message || "Xóa lớp học thất bại");
      }
    } catch {
      message.error("Xóa thất bại");
    }
  };

  const openMembersModal = (row) => {
    setManagingClass(row);
    setOpenMembers(true);
  };

  const openTeacherDrawerPicker = (row) => {
    setPickerClass(row);
    setOpenTeacherDrawer(true);
  };

  const openStudentDrawerPicker = (row) => {
    setPickerClass(row);
    setOpenStudentDrawer(true);
  };

  const handleOnChangePagi = (pagination) => {
    if (pagination && pagination.pageSize && +pagination.pageSize !== +pageSize) {
      setPageSize(+pagination.pageSize);
      setCurrent(1);
    }
    if (pagination && pagination.current && +pagination.current !== +current) {
      setCurrent(+pagination.current);
    }
  };

  return (
    <>
      <div className={styles.wrapper}>
        <ClassFilter
          q={q}
          setQ={setQ}
          setCurrent={setCurrent}
          onOpenCreate={openCreate}
        />

        <div className={styles.tableCard}>
          <ClassTable
            classes={filtered}
            loading={loading}
            pagination={{ current, pageSize, total }}
            onChange={handleOnChangePagi}
            onEdit={openEdit}
            onDelete={handleDelete}
            onOpenMembers={openMembersModal}
            onOpenTeacherDrawer={openTeacherDrawerPicker}
            onOpenStudentDrawer={openStudentDrawerPicker}
          />
        </div>

        <ClassEditModal
          open={openEditor}
          onCancel={() => setOpenEditor(false)}
          onSuccess={fetchClasses}
          editingClass={editingClass}
        />

        <ClassMembersModal
          open={openMembers}
          onCancel={() => {
            setOpenMembers(false);
            setManagingClass(null);
          }}
          managingClass={managingClass}
          onUpdate={fetchClasses}
        />

        <TeacherPickerDrawer
          open={openTeacherDrawer}
          onClose={() => setOpenTeacherDrawer(false)}
          classId={pickerClass?.id}
          onSuccess={fetchClasses}
        />

        <StudentPickerDrawer
          open={openStudentDrawer}
          onClose={() => setOpenStudentDrawer(false)}
          classId={pickerClass?.id}
          onSuccess={fetchClasses}
        />
      </div>
    </>
  );
}

