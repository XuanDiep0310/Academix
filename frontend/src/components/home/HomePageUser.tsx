import HeroHomePage from "@/components/home/HeroHomePage";
import "@/assets/styles/homePage.scss";
import WhoForHomePage from "@/components/home/WhoForHomePage";
import TitleHomePage from "@/components/home/TitleHomePage";
import ContentHomePage from "@/components/home/ContentHomePage";
import ResponseHomePage from "@/components/home/ResponseHomePage";
import { getServerSession } from "next-auth/next";
import { authOptions } from "@/app/api/auth/[...nextauth]/route";

const HomePageUser = async () => {
  const session = await getServerSession(authOptions);
  console.log(">> check session Server", session);
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
