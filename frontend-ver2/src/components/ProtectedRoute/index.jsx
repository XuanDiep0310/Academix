// src/components/ProtectedRoute.jsx
import { useSelector } from "react-redux";
import { Navigate } from "react-router"; // hoặc "react-router-dom" nếu bạn dùng v6
import NotPermitted from "./NotPermitted";

const RoleBaseRoute = ({ children }) => {
  const user = useSelector((state) => state.account.user);
  const userRole = user?.role;
  const path = window.location.pathname;

  const isAdminRoute = path.startsWith("/admin");
  const isTeacherRoute = path.startsWith("/teacher");
  const isStudentRoute = path.startsWith("/student");

  let canAccess = false;

  if (isAdminRoute) {
    canAccess = userRole === "Admin";
  } else if (isTeacherRoute) {
    canAccess = userRole === "Teacher" || userRole === "Admin";
  } else if (isStudentRoute) {
    canAccess = userRole === "Student" || userRole === "Admin";
  } else {
    canAccess = true;
  }

  return canAccess ? <>{children}</> : <NotPermitted />;
};

const ProtectedRoute = ({ children }) => {
  const isAuthenticated = useSelector((state) => state.account.isAuthenticated);

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  return <RoleBaseRoute>{children}</RoleBaseRoute>;
};

export default ProtectedRoute;
