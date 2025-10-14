"use client";
import "@/assets/styles/responseHomePage.scss";
import { useTranslations } from "next-intl";
import diepAvatar from "@/assets/img/diep.jpg";
import Image from "next/image";
import { Rate } from "antd";
import SliderResponse from "@/components/home/SliderResponse";

const ResponseHomePage = () => {
  const t = useTranslations();
  const FEEDBACKS = [
    {
      id: 1,
      name: "Hà Vy",
      school: "ĐH Kinh Tế Kỹ Thuật CN Hà Nội",
      quote:
        "Phần mềm xứng đáng sử dụng nhất năm, dễ dùng mà hỗ trợ học rất tốt...",
      avatar: "/avatars/havy.png",
      moreHref: "/feedback/ha-vy",
    },
    {
      id: 2,
      name: "Nguyễn Nguyên",
      school: "Học viện Y Dược học cổ truyền",
      quote: "Mình dùng EduQuiz hơn 2 năm, thực sự hữu ích cho sinh viên...",
      avatar: "/avatars/nguyen.png",
      moreHref: "/feedback/nguyen-nguyen",
    },
    {
      id: 3,
      name: "Diễm Loan",
      school: "Học viện Dược, học cổ truyền VN",
      quote:
        "Giao diện đơn giản, trải nghiệm mượt, có phần làm lại câu sai rất hay...",
      avatar: "/avatars/loan.png",
    },
    {
      id: 4,
      name: "Võ",
      school: "ĐH Kinh Tế Kỹ Thuật CN Hà Nội",
      quote:
        "Phần mềm xứng đáng sử dụng nhất năm, dễ dùng mà hỗ trợ học rất tốt...",
      avatar: "/avatars/havy.png",
      moreHref: "/feedback/ha-vy",
    },
    {
      id: 5,
      name: "Điệp",
      school: "ĐH Kinh Tế Kỹ Thuật CN Hà Nội",
      quote:
        "Phần mềm xứng đáng sử dụng nhất năm, dễ dùng mà hỗ trợ học rất tốt...",
      avatar: "/avatars/havy.png",
      moreHref: "/feedback/ha-vy",
    },
    {
      id: 6,
      name: "Phát",
      school: "ĐH Kinh Tế Kỹ Thuật CN Hà Nội",
      quote:
        "Phần mềm xứng đáng sử dụng nhất năm, dễ dùng mà hỗ trợ học rất tốt...",
      avatar: "/avatars/havy.png",
      moreHref: "/feedback/ha-vy",
    },
    {
      id: 7,
      name: "Phú",
      school: "ĐH Kinh Tế Kỹ Thuật CN Hà Nội",
      quote:
        "Phần mềm xứng đáng sử dụng nhất năm, dễ dùng mà hỗ trợ học rất tốt...",
      avatar: "/avatars/havy.png",
      moreHref: "/feedback/ha-vy",
    },
  ];
  return (
    <>
      <section className="response-home">
        <div className="container">
          <div className="response-home__top">
            <div className="response-home__top--title">
              <div>
                <span>{t("UserPage.response")}</span>
              </div>
            </div>
            <div className="response-home__top--content">
              <div className="response-home__top--img1">
                <Image src={diepAvatar} alt="diep" />
              </div>
              <div className="response-home__top--img2">
                <Image src={diepAvatar} alt="diep" />
              </div>
              <div className="response-home__top--img3">
                <Image src={diepAvatar} alt="diep" />
              </div>
              <div className="response-home__top--img4">
                <Image src={diepAvatar} alt="diep" />
              </div>
              <div className="response-home__top--img">
                <Image src={diepAvatar} alt="diep" />
              </div>
              <div className="response-home__top--desc">
                <div className="response-home__top--rate">
                  <Rate disabled defaultValue={5} />
                  <span>200,000+ khách hàng</span>
                </div>
                <div className="response-home__top--line"></div>
              </div>
            </div>
            <div className="response-home__top--bottom1"></div>
            <div className="response-home__top--bottom2"></div>
          </div>
          <div className="response-home__bottom">
            <SliderResponse feedbacks={FEEDBACKS} />
          </div>
        </div>
      </section>
    </>
  );
};
export default ResponseHomePage;
