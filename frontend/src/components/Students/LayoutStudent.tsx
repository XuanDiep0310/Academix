"use client";

import { usePathname, useRouter } from "next/navigation";
import styles from "@/assets/styles/StudentDashboard.module.scss";
import { Sidebar } from "@/components/Students/Sidebar";
import { SEED_CLASSES } from "@/data/seed";

const LayoutStudent = ({ children }: { children: React.ReactNode }) => {
  const pathname = usePathname();
  const router = useRouter();

  // Nếu Sidebar cũ của bạn nhận props currentView/setCurrentView,
  // bạn có thể ánh xạ từ pathname -> view để highlight:
  const pathToView = (p: string) => {
    if (p.startsWith("/student/documentviewer")) return "documents";
    if (p.startsWith("/student/chatbox")) return "chatbot";
    if (p.startsWith("/student/qaforum")) return "qa";
    if (p.startsWith("/student/examtaking")) return "exam";
    return "dashboard";
  };
  const currentView = pathToView(pathname);

  const handleNavigate = (view: string) => {
    // Điều hướng theo view mong muốn
    if (view === "documents") router.push("/student/documentviewer");
    else if (view === "chatbot") router.push("/student/chatbox");
    else if (view === "qa") router.push("/student/qaforum");
    else router.push("/student"); // dashboard mặc định
  };

  return (
    <section className={styles.layoutWrapper}>
      <Sidebar
        // Nếu Sidebar của bạn cần 2 props dưới, giữ lại; nếu không, bỏ đi
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        currentView={currentView as any}
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        setCurrentView={handleNavigate as any}
        classes={SEED_CLASSES}
      />
      <div className={styles.mainContent}>{children}</div>
    </section>
  );
};
export default LayoutStudent;
