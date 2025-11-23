import  { useEffect, useState } from "react";
import {
  Modal,
  Space,
  Typography,
  Button,
  Divider,
  Table,
  Popconfirm,
  message,
  notification,
} from "antd";
import { UserPlus } from "lucide-react";
import styles from "../../../assets/styles/ClassManagement.module.scss";
import {
  callListTeacherOnClassesAPI,
  callListStudentOnClassesAPI,
  deleteMemberOutClassAPI,
} from "../../../services/api.service";
import TeacherPickerModal from "./TeacherPickerModal";
import StudentPickerModal from "./StudentPickerModal";

const { Title, Text } = Typography;
const MAX_TEACHERS = 2;
const MAX_STUDENTS = 100;

const  ClassMembersModal = (props) => {
  const { open, onCancel, managingClass, onUpdate } = props;
  const [teachers, setTeachers] = useState([]);
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(false);

  const [openTeacherPicker, setOpenTeacherPicker] = useState(false);
  const [openStudentPicker, setOpenStudentPicker] = useState(false);

  const fetchMembers = async () => {
    if (!managingClass) return;
    try {
      setLoading(true);
      const [tRes, sRes] = await Promise.all([
        callListTeacherOnClassesAPI(managingClass.id),
        callListStudentOnClassesAPI(managingClass.id),
      ]);

      if (tRes && tRes.success) {
        setTeachers(
          tRes.data?.map((t) => ({
            id: t.userId,
            name: t.fullName,
            email: t.email,
          })) || []
        );
      }
      if (sRes && sRes.success) {
        setStudents(
          sRes.data?.map((s) => ({
            id: s.userId,
            name: s.fullName,
            email: s.email,
          })) || []
        );
      }
    } catch (error) {
      console.error("Fetch members error:", error);
      message.error("Không thể tải danh sách thành viên");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (open && managingClass) {
      fetchMembers();
    }
  }, [open, managingClass]);

  const handleRemove = async (userId, type) => {
    try {
      const res = await deleteMemberOutClassAPI(managingClass.id, userId);
      if (res && res.success === true) {
        message.success("Đã xóa thành viên khỏi lớp");
        if (type === "teacher") {
          setTeachers((prev) => prev.filter((t) => t.id !== userId));
        } else {
          setStudents((prev) => prev.filter((s) => s.id !== userId));
        }
        if (onUpdate) onUpdate();
      } else {
        notification.error({
          message: "Xóa thất bại",
          description: res?.message || "Có lỗi xảy ra",
        });
      }
    } catch (error) {
      console.error("Remove member error:", error);
      message.error("Thao tác thất bại");
    }
  };

  const handleAddSuccess = () => {
    fetchMembers();
    if (onUpdate) onUpdate();
  };

  return (
    <Modal
      title={
        <Space direction="vertical" size={0}>
          <Text strong>Thành viên lớp</Text>
          <Text type="secondary">{managingClass?.name}</Text>
        </Space>
      }
      open={open}
      onCancel={onCancel}
      footer={null}
      width={900}
      destroyOnClose
    >
      <div className={styles.membersWrap}>
        <div className={styles.memberBlock}>
          <div className={styles.memberHeader}>
            <Title level={5} style={{ margin: 0 }}>
              Giáo viên ({teachers.length}/{MAX_TEACHERS})
            </Title>
            <Button
              size="middle"
              icon={<UserPlus size={16} />}
              onClick={() => setOpenTeacherPicker(true)}
            >
              Thêm giáo viên
            </Button>
          </div>
          <Divider style={{ margin: "12px 0" }} />
          <Table
            size="small"
            rowKey="id"
            dataSource={teachers}
            pagination={false}
            loading={loading}
            columns={[
              {
                title: "Họ tên",
                dataIndex: "name",
              },
              {
                title: "Email",
                dataIndex: "email",
              },
              {
                title: "Thao tác",
                align: "right",
                render: (_, record) => (
                  <Popconfirm
                    title="Xóa giáo viên khỏi lớp?"
                    okText="Xóa"
                    cancelText="Hủy"
                    onConfirm={() => handleRemove(record.id, "teacher")}
                  >
                    <Button type="text" danger>
                      Xóa
                    </Button>
                  </Popconfirm>
                ),
              },
            ]}
            locale={{ emptyText: "Chưa có giáo viên" }}
          />
        </div>

        <div className={styles.memberBlock}>
          <div className={styles.memberHeader}>
            <Title level={5} style={{ margin: 0 }}>
              Học sinh ({students.length}/{MAX_STUDENTS})
            </Title>
            <Button
              size="middle"
              icon={<UserPlus size={16} />}
              onClick={() => setOpenStudentPicker(true)}
            >
              Thêm học sinh
            </Button>
          </div>
          <Divider style={{ margin: "12px 0" }} />
          <div className={styles.studentTable}>
            <Table
              size="small"
              rowKey="id"
              dataSource={students}
              pagination={{ pageSize: 8 }}
              loading={loading}
              columns={[
                {
                  title: "Họ tên",
                  dataIndex: "name",
                },
                {
                  title: "Email",
                  dataIndex: "email",
                },
                {
                  title: "Thao tác",
                  align: "right",
                  render: (_, record) => (
                    <Popconfirm
                      title="Xóa học sinh khỏi lớp?"
                      okText="Xóa"
                      cancelText="Hủy"
                      onConfirm={() => handleRemove(record.id, "student")}
                    >
                      <Button type="text" danger>
                        Xóa
                      </Button>
                    </Popconfirm>
                  ),
                },
              ]}
              locale={{ emptyText: "Chưa có học sinh" }}
            />
          </div>
        </div>
      </div>

      <TeacherPickerModal
        open={openTeacherPicker}
        onCancel={() => setOpenTeacherPicker(false)}
        classId={managingClass?.id}
        onSuccess={handleAddSuccess}
      />

      <StudentPickerModal
        open={openStudentPicker}
        onCancel={() => setOpenStudentPicker(false)}
        classId={managingClass?.id}
        onSuccess={handleAddSuccess}
      />
    </Modal>
  );
}
export default ClassMembersModal;