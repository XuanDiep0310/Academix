import { UploadOutlined } from "@ant-design/icons";
import {
  Avatar,
  Button,
  Col,
  Form,
  Input,
  message,
  notification,
  Row,
  Upload,
} from "antd";
import { useForm } from "antd/es/form/Form";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  callUpdateAvatar,
  callUpdateUserInfo,
} from "../../services/api.service";
import {
  doUpdateUserInfoAction,
  doUploadAvatarAction,
} from "../../redux/account/accountSlice";

const UserInfo = () => {
  const userData = useSelector((state) => state.account.user);
  const [avatarUserData, setAvatarUserData] = useState(userData?.avatar ?? "");
  const [isSubmit, setIsSubmit] = useState(false);
  const [form] = useForm();
  const tempAvatar = useSelector((state) => state.account.tempAvatar);

  const dispatch = useDispatch();
  useEffect(() => {
    const init = {
      email: userData?.email,
      fullName: userData?.fullName,
      phone: userData?.phone,
    };
    form.setFieldsValue(init);
  }, []);

  const handleUploadAvatar = async ({ file, onSuccess, onError }) => {
    const res = await callUpdateAvatar(file);
    if (res && res.data) {
      const newAvatar = res.data.fileUploaded;
      dispatch(doUploadAvatarAction({ avatar: newAvatar }));
      setAvatarUserData(newAvatar);
      onSuccess("ok");
    } else {
      onError("Đã có lỗi xảy ra khi upload file");
    }
  };
  const propsUpload = {
    maxCount: 1,
    multiple: false,
    showUploadList: false,
    customRequest: handleUploadAvatar,
    onChange(info) {
      if (info.file.status !== "uploading") {
        console.log("upload");
      }
      if (info.file.status === "done") {
        message.success(`Tải file thành công!`);
      } else if (info.file.status === "error") {
        message.error(`Tải file thất bại!`);
      }
    },
  };
  const onFinish = async (values) => {
    setIsSubmit(true);
    const { fullName, phone } = values;
    const _id = userData?.id;
    const res = await callUpdateUserInfo(_id, phone, fullName, avatarUserData);
    if (res && res?.data) {
      dispatch(
        doUpdateUserInfoAction({ avatar: avatarUserData, phone, fullName })
      );
      message.success("Cập nhật thông tin người dùng thành công!");

      // xóa  access_token dẻ cập nhật thông tin access_token mới
      localStorage.removeItem("access_token");
    } else {
      notification.error({
        message: "Đã có lỗi xảy ra!",
        description: res.message,
      });
    }
    setIsSubmit(false);
  };

  return (
    <>
      <Form
        form={form}
        style={{
          //   maxWidth: 800,
          margin: " 0 auto",
          background: "#fff",
          borderRadius: "12px",
        }}
        onFinish={onFinish}
        layout="vertical"
        autoComplete="off"
      >
        <Row>
          <Col xs={24} sm={12} md={12} lg={10}>
            <div
              style={{
                height: "150px",
                display: "flex",
                justifyContent: "center",
              }}
            >
              <Avatar
                size={150}
                src={`${import.meta.env.VITE_BACKEND_URL}/images/avatar/${
                  tempAvatar || userData?.avatar
                }`}
              />
            </div>
            <div
              style={{
                marginTop: "20px",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
              }}
            >
              <Upload {...propsUpload}>
                <Button icon={<UploadOutlined />}>Tải lên Avatar</Button>
              </Upload>
            </div>
          </Col>
          <Col xs={24} sm={12} md={12} lg={14}>
            <Row gutter={[10, 10]}>
              <Col span={24}>
                <Form.Item
                  label="Email"
                  labelCol={{ span: 24 }}
                  name="email"
                  rules={[{ required: true, message: "Vui lòng nhập Email!" }]}
                >
                  <Input disabled={true} />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Form.Item
                  label="Tên hiển thị"
                  labelCol={{ span: 24 }}
                  name="fullName"
                  rules={[
                    { required: true, message: "Vui lòng nhập Tên hiển thị!" },
                  ]}
                >
                  <Input placeholder="Nhập tên hiển thị" />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Form.Item
                  label="Số điện thoại"
                  labelCol={{ span: 24 }}
                  name="phone"
                  rules={[
                    {
                      required: true,
                      pattern: new RegExp(/\d+/g),
                      message: "Vui lòng nhập Số điện thoại!",
                    },
                  ]}
                >
                  <Input />
                </Form.Item>
              </Col>
              <Col span={24}>
                <Button onClick={() => form.submit()} loading={isSubmit}>
                  Cập nhật
                </Button>
              </Col>
            </Row>
          </Col>
        </Row>
      </Form>
    </>
  );
};
export default UserInfo;
