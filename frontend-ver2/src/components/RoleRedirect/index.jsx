import { useSelector } from "react-redux";
import { Navigate } from "react-router";

const RoleRedirect = () => {
  const isAuthenticated = useSelector((state) => state.account.isAuthenticated);
  const user = useSelector((state) => state.account.user);

  // Chưa đăng nhập -> về /login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const role = user?.role;

  // Đã đăng nhập -> điều hướng theo role
  if (role === "Admin") {
    return <Navigate to="/admin" replace />;
  }

  if (role === "Teacher") {
    return <Navigate to="/teacher" replace />;
  }

  if (role === "Student") {
    return <Navigate to="/student" replace />;
  }

  // Nếu role lạ, fallback về /login hoặc /
  return <Navigate to="/login" replace />;
};

export default RoleRedirect;
