import { Modal, Tabs } from "antd";
import { useForm } from "antd/es/form/Form";
import UserInfo from "./UserInfo";
import ChangePassword from "./ChangePassword";

const ManageAccount = (props) => {
  const { isModalOpenUser, setIsModalOpenUser } = props;
  const itemManageUser = [
    {
      key: "1",
      label: "Cập nhật thông tin",
      children: <UserInfo />,
    },
    {
      key: "2",
      label: "Đổi mật khẩu",
      children: <ChangePassword />,
    },
  ];
  return (
    <>
      <Modal
        title="Quản lí tài khoản"
        closable={{ "aria-label": "Custom Close Button" }}
        open={isModalOpenUser}
        // onOk={() => form.submit()}
        onCancel={() => {
          setIsModalOpenUser(false);
        }}
        // confirmLoading={isSubmit}
        footer={false}
        width={800}
      >
        <Tabs defaultActiveKey="1" items={itemManageUser} />
      </Modal>
    </>
  );
};
export default ManageAccount;
