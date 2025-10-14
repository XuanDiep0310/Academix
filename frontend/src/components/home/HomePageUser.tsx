import HeroHomePage from "@/components/home/HeroHomePage";
import "@/assets/styles/homePage.scss";
import WhoForHomePage from "@/components/home/WhoForHomePage";
import TitleHomePage from "@/components/home/TitleHomePage";
import ContentHomePage from "@/components/home/ContentHomePage";
import ResponseHomePage from "@/components/home/ResponseHomePage";
const HomePageUser = () => {
  return (
    <>
      <div className="home-page-user">
        <HeroHomePage />
        <WhoForHomePage />
        <TitleHomePage />
        <ContentHomePage />
        <ResponseHomePage />
      </div>
    </>
  );
};
export default HomePageUser;
