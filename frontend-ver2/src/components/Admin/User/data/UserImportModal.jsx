// src/pages/.../UserImportModal.jsx
import { Modal, Table, Upload, message, notification } from "antd";
import { InboxOutlined } from "@ant-design/icons";
import * as XLSX from "xlsx";
import { useState } from "react";
import { callBulkCreateUser } from "../../../../services/api.service";
import templateFile from "./templateFile.xlsx?url"; // file mẫu

const { Dragger } = Upload;

const UserImportModal = (props) => {
  const { open, onClose, onSuccess } = props;
  const [dataExcel, setDataExcel] = useState([]);
  const [submitting, setSubmitting] = useState(false);

  const dummyRequest = ({ file, onSuccess }) => {
    // Không upload lên server, chỉ giả thành công để onChange chạy
    setTimeout(() => {
      onSuccess("ok");
    }, 0);
  };

  const uploadProps = {
    name: "file",
    multiple: false,
    maxCount: 1,
    accept:
      ".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel",
    customRequest: dummyRequest,
    onChange(info) {
      const { status } = info.file;

      if (info.fileList && info.fileList.length < 1) {
        setDataExcel([]);
      }

      if (status === "done") {
        if (info.fileList && info.fileList.length > 0) {
          const file = info.fileList[0].originFileObj;
          const reader = new FileReader();

          reader.readAsArrayBuffer(file);
          reader.onload = () => {
            const data = new Uint8Array(reader.result);
            const workbook = XLSX.read(data, { type: "array" });
            const sheet = workbook.Sheets[workbook.SheetNames[0]];

            // Header dòng 1: FullName | Email | Password | Role
            const json = XLSX.utils.sheet_to_json(sheet, {
              header: ["fullName", "email", "password", "role"],
              range: 1, // bỏ dòng header
            });

            if (json && json.length > 0) {
              setDataExcel(json);
            } else {
              setDataExcel([]);
              notification.warning({
                message: "File rỗng",
                description: "Không đọc được dữ liệu từ file Excel",
              });
            }
          };
        }

        message.success(`${info.file.name} tải lên thành công.`);
      } else if (status === "error") {
        message.error(`${info.file.name} tải lên thất bại!`);
      }
    },
    onDrop(e) {
      console.log("Dropped files", e.dataTransfer.files);
    },
  };

  const handleSubmit = async () => {
    if (!dataExcel.length) {
      notification.warning({
        message: "Chưa có dữ liệu",
        description: "Vui lòng chọn file Excel trước khi import",
      });
      return;
    }

    try {
      setSubmitting(true);

      // Map sang format API
      const apiUsers = dataExcel.map((item) => ({
        fullName: item.fullName,
        email: item.email,
        password: item.password || "123456@Academix",
        role: item.role || "Student", // hoặc "Teacher" tuỳ file
      }));

      const res = await callBulkCreateUser({ users: apiUsers });

      if (res && res.success) {
        notification.success({
          message: "Import thành công",
          description: `Import thành công ${res.message}`,
        });

        setDataExcel([]);
        onClose();
        onSuccess && onSuccess();
      } else {
        notification.error({
          message: "Đã xảy ra lỗi!",
          description:
            (res && JSON.stringify(res.message)) ||
            "Có lỗi xảy ra khi import dữ liệu",
        });
      }
    } catch (err) {
      console.error(err);
      notification.error({
        message: "Lỗi",
        description: "Không thể import dữ liệu, vui lòng thử lại",
      });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Modal
      title="Import tài khoản từ Excel"
      width="60vw"
      open={open}
      onOk={handleSubmit}
      okText="Import dữ liệu"
      cancelText="Hủy"
      confirmLoading={submitting}
      onCancel={() => {
        if (submitting) return;
        onClose();
        setDataExcel([]);
      }}
    >
      <Dragger {...uploadProps}>
        <p className="ant-upload-drag-icon">
          <InboxOutlined />
        </p>
        <p className="ant-upload-text">
          Nhấp hoặc kéo tệp vào khu vực này để tải lên
        </p>
        <p className="ant-upload-hint">
          Hỗ trợ: .csv, .xls, .xlsx. Hoặc&nbsp;
          <a onClick={(e) => e.stopPropagation()} href={templateFile} download>
            Tải xuống file mẫu
          </a>
        </p>
      </Dragger>

      <Table
        title={() => <>Dữ liệu đọc từ Excel</>}
        columns={[
          {
            title: "Họ tên",
            dataIndex: "fullName",
            key: "fullName",
          },
          {
            title: "Email",
            dataIndex: "email",
            key: "email",
          },
          {
            title: "Mật khẩu",
            dataIndex: "password",
            key: "password",
          },
          {
            title: "Role",
            dataIndex: "role",
            key: "role",
          },
        ]}
        rowKey={(record, idx) => idx}
        dataSource={dataExcel}
        style={{ marginTop: 20 }}
        pagination={false}
      />
    </Modal>
  );
};

export default UserImportModal;
