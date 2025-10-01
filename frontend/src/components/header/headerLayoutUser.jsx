import { Col, Row } from "antd";
import Image from "next/image";
import HeaderMenuUser from "./headerMenuUser";
import LocaleSwitcher from "@/components/LocaleSwitcher";
import ThemeToggle from "@/components/ThemeToggle";
import "@/styles/headerLayoutUser.scss";

const HeaderLayoutUser = () => {
  return (
    <>
      <div className="container-fluid">
        <Row gutter={[20, 20]} className="header-user">
          <Col span={6}>
            <div className="header-user__left">
              <div className="header-user__logo">{/* <Image /> */}</div>
              <div>Academix</div>
            </div>
          </Col>
          <Col span={10}>
            <div className="header-user__right">
              <HeaderMenuUser />
            </div>
          </Col>
          <Col span={4} className="header-user__settings">
            <LocaleSwitcher />
            <ThemeToggle />
          </Col>
        </Row>
      </div>
    </>
  );
};
export default HeaderLayoutUser;
