import LayoutStudent from "@/components/Students/LayoutStudent";

// "use client";
export const metadata = {
  title: "Student | Next.js",
  description: "Khu vực học sinh",
};

export default function StudentLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <LayoutStudent>{children}</LayoutStudent>;
}
