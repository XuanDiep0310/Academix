import { useSelector } from "react-redux";
import { Navigate } from "react-router";
import NotPermitted from "./NotPermitted";

const RoleBaseRoute = (props) => {
  const isAdminRoute = window.location.pathname.startsWith("/admin");
  const user = useSelector((state) => state.account.user);
  const userRole = user.role;
  if (
    (isAdminRoute && userRole === "ADMIN") ||
    (!isAdminRoute && (userRole === "USER" || userRole === "ADMIN"))
  ) {
    return <>{props.children}</>;
  } else {
    return <NotPermitted />;
  }
};

const ProtectedRoute = (props) => {
  const isAuthenticated = useSelector((state) => state.account.isAuthenticated);
  return (
    <>
      {isAuthenticated === true ? (
        <RoleBaseRoute>{props.children}</RoleBaseRoute>
      ) : (
        <Navigate to="/login" />
      )}
    </>
  );
};
export default ProtectedRoute;
