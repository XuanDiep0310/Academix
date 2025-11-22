import { useEffect, useState } from "react";
import { message, notification } from "antd";
import * as XLSX from "xlsx";

import styles from "../../../assets/styles/UserManagement.module.scss";
import {
  callListUserAPI,
  deleteUserAPI,
  editUserStatusAPI,
} from "../../../services/api.service";
import UserDetail from "./UserDetail";
import UserImportModal from "./data/UserImportModal";
import UserFilter from "./UserFilter";
import UserTable from "./UserTable";
import TeacherModal from "./TeacherModal";
import StudentBulkModal from "./StudentBulkModal";

const mapApiUserToRow = (u) => {
  return {
    id: u.userId, // dùng userId của API làm rowKey
    email: u.email,
    name: u.fullName,
    role:
      u.role === "Teacher"
        ? "teacher"
        : u.role === "Student"
        ? "student"
        : u.role?.toLowerCase(),
    status: u.isActive ? "active" : "locked",
    createdAt: u.createdAt, // "2025-11-13"
    updateAt: u.updatedAt, // "2025-11-13"
  };
};

const UserManagement = () => {

  const [users, setUsers] = useState([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);

 
  const [q, setQ] = useState("");


  const [pageSize, setPageSize] = useState(10);
  const [current, setCurrent] = useState(1);

  
  const [openTeacherModal, setOpenTeacherModal] = useState(false);
  const [editingUser, setEditingUser] = useState(null);

  const [openStudentBulk, setOpenStudentBulk] = useState(false);


  const [userDetail, setUserDetail] = useState(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);


  const [isImportOpen, setIsImportOpen] = useState(false);

  const openUserDetail = (row) => {
    setUserDetail(row);
    setIsDetailOpen(true);
  };


  const fetchUsers = async () => {
    try {
      const params = new URLSearchParams();
      params.append("page", String(current));
      params.append("pageSize", String(pageSize));

      if (q.trim()) {
        params.append("search", q.trim());
      }

      const query = params.toString();
      console.log("Fetching users with query:", query);
      setLoading(true);
      const res = await callListUserAPI(query);
      const data = res.data;

      const items = data.users || [];
      const totalItems = data.totalCount || 0;

      const mapped = items.map(mapApiUserToRow);
      setUsers(mapped);
      setTotal(totalItems);

      
      if (data.page && data.page !== current) {
        setCurrent(data.page);
      }
      if (data.pageSize && data.pageSize !== pageSize) {
        setPageSize(data.pageSize);
      }
    } catch (err) {
      console.error(err);
      message.error("Không tải được danh sách người dùng");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [current, pageSize, q]);



  const handleOnChangePagi = (pagination, filters, sorter) => {
    if (
      pagination &&
      pagination.pageSize &&
      +pagination.pageSize !== +pageSize
    ) {
      setPageSize(+pagination.pageSize);
      setCurrent(1); 
    }

    if (pagination && pagination.current && +pagination.current !== +current) {
      setCurrent(+pagination.current);
    }
  };


  const openCreateTeacher = () => {
    setEditingUser(null);
    setOpenTeacherModal(true);
  };

  const openEditUser = (row) => {
    setEditingUser(row);
    setOpenTeacherModal(true);
  };

  const toggleStatus = async (row) => {
    console.log("Toggling status for", row);
    const res = await editUserStatusAPI(
      row.id,
      row.name,
      row.email,
      row.status === "active" ? false : true
    );
    if (res && res.success === true) {
      message.success(
        `Đã ${row.status === "active" ? "khóa" : "mở khóa"} tài khoản`
      );
      await fetchUsers();
    }
  };

  const deleteUser = async (id) => {
    const res = await deleteUserAPI(id);
    if (res && res.success === true) {
      message.success("Đã xóa tài khoản");
      await fetchUsers();
    } else {
      notification.error({
        message: "Error",
        description:
          JSON.stringify(res?.message) || "Có lỗi xảy ra khi xóa tài khoản",
      });
    }
  };

  const handleExportExcel = () => {
    if (!users || users.length === 0) {
      notification.warning({
        message: "Không có dữ liệu",
        description: "Hiện chưa có tài khoản nào để xuất Excel",
      });
      return;
    }

    const data = users.map((u) => ({
      Email: u.email,
      "Họ và tên": u.name,
      Role: u.role,
      Trạng_thái: u.status,
    }));

    const worksheet = XLSX.utils.json_to_sheet(data);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Users");

    XLSX.writeFile(workbook, "users.xlsx");
  };

  return (
    <>
      <div className={styles.wrap}>
        <UserFilter
          q={q}
          setQ={setQ}
          setCurrent={setCurrent}
          onExport={handleExportExcel}
          onImport={() => setIsImportOpen(true)}
          onAddStudent={() => setOpenStudentBulk(true)}
          onAddTeacher={openCreateTeacher}
        />

        <div className={styles.tableCard}>
          <UserTable
            users={users}
            loading={loading}
            pagination={{ current, pageSize, total }}
            onChange={handleOnChangePagi}
            onEdit={openEditUser}
            onDelete={deleteUser}
            onToggleStatus={toggleStatus}
            onViewDetail={openUserDetail}
          />
        </div>

        <TeacherModal
          open={openTeacherModal}
          onCancel={() => setOpenTeacherModal(false)}
          onSuccess={fetchUsers}
          editingUser={editingUser}
        />

        <StudentBulkModal
          open={openStudentBulk}
          onCancel={() => setOpenStudentBulk(false)}
          onSuccess={() => {
            setCurrent(1);
            fetchUsers();
          }}
        />
      </div>
      <UserImportModal
        open={isImportOpen}
        onClose={() => setIsImportOpen(false)}
        onSuccess={() => {
          setCurrent(1);
          fetchUsers();
        }}
      />

      <UserDetail
        userDetail={userDetail}
        setUserDetail={setUserDetail}
        isDetailOpen={isDetailOpen}
        setIsDetailOpen={setIsDetailOpen}
      />
    </>
  );
}

export default UserManagement;