import { Col, Row } from "antd";
import Image from "next/image";
import HeaderMenuUser from "./HeaderMenuUser";

import logoAcademix from "@/assets/img/logo-Academix.png";

import "@/assets/styles/headerLayoutUser.scss";

const HeaderLayoutUser = () => {
  return (
    <>
      <div className="container-fluid">
        <Row gutter={[20, 20]}>
          <Col xl={6} lg={6} md={8} sm={12} xs={12}>
            <div className="header-user__left">
              <div className="header-user__logo">
                <Image src={logoAcademix} alt="logo-academix" />
              </div>
              <p>Academix</p>
            </div>
          </Col>
          <Col xl={18} lg={18} md={16} sm={12} xs={12}>
            <div className="header-user__right">
              <HeaderMenuUser />
            </div>
          </Col>
          {/* <div className="header-user"></div>  */}
        </Row>
      </div>
    </>
  );
};
export default HeaderLayoutUser;
