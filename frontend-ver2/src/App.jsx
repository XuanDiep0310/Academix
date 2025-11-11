import { createBrowserRouter, RouterProvider } from "react-router";
import LoginPage from "./pages/login";
import RegisterPage from "./pages/register";
import { Outlet } from "react-router";
import Home from "./components/Home/index";
import Header from "./components/Header";
import Footer from "./components/Footer";
import { useEffect, useState } from "react";
import { callFetchAccount } from "./services/api.service";
import { useDispatch, useSelector } from "react-redux";
import { doGetAccountAction } from "./redux/account/accountSlice";
import Loading from "./components/Loading";
import NotFound from "./components/NotFound/index";
import LayoutAdmin from "./components/Admin/LayoutAdmin";
import ClassManagement from "./components/Admin/ClassManagement";
import UserManagement from "./components/Admin/UserManagement";
import LayoutTeacher from "./components/Teacher/LayoutTeacher";
import ClassList from "./components/Teacher/ClassList";
import MaterialManagement from "./components/Teacher/MaterialManagement";
import QuestionBank from "./components/Teacher/QuestionBank";
import ResultsView from "./components/Teacher/ResultsView";
import TestManagement from "./components/Teacher/TestManagement";
import LayoutStudent from "./components/Student/LayoutStudent";
import { StudentClassList } from "./components/Student/StudentClassList";
import MaterialView from "./components/Student/MaterialView";
import { StudentResults } from "./components/Student/StudentResults";
import { TestTaking } from "./components/Student/TestTaking";

const Layout = () => {
  const [searchTerm, setSearchTerm] = useState("");
  return (
    <>
      <div
        className="layout-app"
        style={{
          display: "flex",
          flexDirection: "column",
          minHeight: "100vh",
        }}
      >
        <Header setSearchTerm={setSearchTerm} searchTerm={searchTerm} />
        <div style={{ flex: "1", background: "#ddd" }}>
          <div className="container">
            <Outlet context={[searchTerm, setSearchTerm]} />
          </div>
        </div>
        <Footer />
      </div>
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
    element: <LayoutAdmin />,
    children: [
      {
        index: true,
        element: <>Hihi</>,
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
    element: <LayoutTeacher />,
    children: [
      {
        index: true,
        element: <>Hihi</>,
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
    element: <LayoutStudent />,
    children: [
      {
        index: true,
        element: <>Hihi</>,
      },
      {
        path: "classes",
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
]);
const App = () => {
  const dispatch = useDispatch();
  // const isLoading = useSelector((state) => state.account.isLoading);
  const isLoading = false;
  // const getAccount = async () => {
  //   if (
  //     window.location.pathname === "/login" ||
  //     window.location.pathname === "/register"
  //   )
  //     return;
  //   const res = await callFetchAccount();
  //   if (res && res?.data) {
  //     dispatch(doGetAccountAction(res.data));
  //   }
  // };
  // useEffect(() => {
  //   getAccount();
  // }, []);
  return (
    <>
      {isLoading === false ||
      window.location.pathname === "/login" ||
      window.location.pathname === "/register" ||
      window.location.pathname === "/" ? (
        <RouterProvider router={router} />
      ) : (
        <Loading />
      )}
    </>
  );
};
export default App;
