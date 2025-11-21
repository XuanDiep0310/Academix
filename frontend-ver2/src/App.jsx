import { createBrowserRouter, RouterProvider } from "react-router";
import LoginPage from "./pages/login";
import RegisterPage from "./pages/register";
import Home from "./components/Home/index";
import { useEffect } from "react";
import { callFetchAccount } from "./services/api.service";
import { useDispatch, useSelector } from "react-redux";
import { doGetAccountAction } from "./redux/account/accountSlice";
import Loading from "./components/Loading";
import NotFound from "./components/NotFound/index";
import LayoutAdmin from "./components/Admin/LayoutAdmin";
import ClassManagement from "./components/Admin/Classes/ClassManagement";
import UserManagement from "./components/Admin/User/UserManagement";
import LayoutTeacher from "./components/Teacher/LayoutTeacher";
import ClassList from "./components/Teacher/Classes/ClassList";
import MaterialManagement from "./components/Teacher/Materia/MaterialManagement";
import QuestionBank from "./components/Teacher/QuestionBank/QuestionBank";
import ResultsView from "./components/Teacher/Result/ResultsView";
import TestManagement from "./components/Teacher/TestManagement/TestManagement";
import LayoutStudent from "./components/Student/LayoutStudent";
import { StudentClassList } from "./components/Student/ClassesView/StudentClassList";
import MaterialView from "./components/Student/MaterialView";
import { StudentResults } from "./components/Student/StudentResults";
import { TestTaking } from "./components/Student/TestTaking";
import AdminPage from "./pages/admin";
import ProtectedRoute from "./components/ProtectedRoute";
import RoleRedirect from "./components/RoleRedirect";
import TeacherPage from "./pages/teacher";
import MaterialPreview from "./components/Teacher/Materia/MaterialPreview";
import "nprogress/nprogress.css";
import ForgotPasswordPage from "./pages/forgotPassword";
import ResetPasswordPage from "./pages/resetPassword";

const Layout = () => {
  return (
    <>
      <RoleRedirect />
    </>
  );
};
let router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    errorElement: <NotFound />,
    children: [
      {
        index: true,
        element: <Home />,
      },
    ],
  },
  {
    path: "/admin",
    element: (
      <ProtectedRoute>
        <LayoutAdmin />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <AdminPage />,
      },
      {
        path: "classes",
        element: <ClassManagement />,
      },
      {
        path: "users",
        element: <UserManagement />,
      },
    ],
  },
  {
    path: "/teacher",
    element: (
      <ProtectedRoute>
        <LayoutTeacher />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <TeacherPage />,
      },
      {
        path: "classes",
        element: <ClassList />,
      },
      {
        path: "materials",
        element: <MaterialManagement />,
      },
      {
        path: "materials/:id",
        element: <MaterialPreview />,
      },
      {
        path: "questions",
        element: <QuestionBank />,
      },
      {
        path: "tests",
        element: <TestManagement />,
      },
      {
        path: "results",
        element: <ResultsView />,
      },
    ],
  },
  {
    path: "/student",
    element: (
      <ProtectedRoute>
        <LayoutStudent />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        // path: "classes",
        element: <StudentClassList />,
      },
      {
        path: "materials",
        element: <MaterialView />,
      },
      {
        path: "tests",
        element: <TestTaking />,
      },
      {
        path: "results",
        element: <StudentResults />,
      },
    ],
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  { path: "/forgot-password", element: <ForgotPasswordPage /> },
  { path: "/reset-password", element: <ResetPasswordPage /> },
]);
const App = () => {
  const dispatch = useDispatch();
  const isLoading = useSelector((state) => state.account.isLoading);
  const getAccount = async () => {
    if (
      window.location.pathname === "/login" ||
      window.location.pathname === "/register"
    )
      return;
    const res = await callFetchAccount();
    if (res && res?.success === true) {
      dispatch(doGetAccountAction(res.data));
    }
  };
  useEffect(() => {
    getAccount();
  }, []);
  return (
    <>
      {isLoading === false ||
      window.location.pathname === "/login" ||
      window.location.pathname === "/register" ||
      window.location.pathname === "/forgot-password" ||
      window.location.pathname === "/reset-password" ||
      window.location.pathname === "/" ? (
        <RouterProvider router={router} />
      ) : (
        <Loading />
      )}
    </>
  );
};
export default App;
