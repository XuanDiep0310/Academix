import { Button, Result } from "antd";
import { useNavigate } from "react-router";

const NotPermitted = () => {
  const navigate = useNavigate();
  return (
    <>
      <Result
        status="403"
        title="403"
        subTitle="Xin lỗi, Bạn không có quyền trùy cập vào page này."
        extra={
          <Button type="primary" onClick={() => navigate("/")}>
            Trở lại trang chủ
          </Button>
        }
      />
    </>
  );
};
export default NotPermitted;
