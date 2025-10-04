import HeroHomePage from "@/components/home/HeroHomePage";
import "@/assets/styles/homePage.scss";
import WhoForHomePage from "@/components/home/WhoForHomePage";
const HomePageUser = () => {
  return (
    <>
      <div className="home-page-user">
        <HeroHomePage />
        <WhoForHomePage />
      </div>
    </>
  );
};
export default HomePageUser;
