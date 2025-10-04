import { Col, Row } from "antd";
import Image from "next/image";
import HeaderMenuUser from "./HeaderMenuUser";
import LocaleSwitcher from "@/components/LocaleSwitcher";
import ThemeToggle from "@/components/ThemeToggle";
import logoAcademix from "@/assets/img/logo-Academix.png";
import "@/assets/styles/headerLayoutUser.scss";

const HeaderLayoutUser = () => {
  return (
    <>
      <div className="container-fluid">
        <Row gutter={[20, 20]} className="header-user">
          <Col xl={6} lg={6} md={8} sm={12} xs={12}>
            <div className="header-user__left">
              <div className="header-user__logo">
                <Image src={logoAcademix} alt="logo-academix" />
              </div>
              <p>Academix</p>
            </div>
          </Col>
          <Col xl={18} lg={18} md={16} sm={12} xs={12}>
            <Row className="header-user__right">
              <Col>
                <HeaderMenuUser />
              </Col>
              <Col>
                <div className="header-user__settings">
                  <LocaleSwitcher />
                  <ThemeToggle />
                </div>
              </Col>
            </Row>
          </Col>
        </Row>
      </div>
    </>
  );
};
export default HeaderLayoutUser;
