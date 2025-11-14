import axios from "../utils/axios-customize";
import { getCookie } from "../utils/cookie";

const registerUserAPI = (fullName, email, password, phone) => {
  const URL_BACKEND = "/api/v1/user/register";
  const data = {
    fullName: fullName,
    email: email,
    password: password,
    phone: phone,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};

const loginUserAPI = (username, password) => {
  const URL_BACKEND = "/api/Auth/login";
  const data = {
    email: username,
    password: password,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const callFetchAccount = () => {
  const URL_BACKEND = "/api/Auth/profile";
  return axios.get(URL_BACKEND);
};
const callLogout = () => {
  const URL_BACKEND = "/api/Auth/logout";
  const refreshToken = getCookie("refresh_token");
  return axios.post(URL_BACKEND, {
    refreshToken: refreshToken,
  });
};

const callListUserAPI = (query) => {
  const URL_BACKEND = `/api/Users?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};

const createUserAPI = (fullName, email, password, role) => {
  const URL_BACKEND = "/api/Users";
  const data = {
    fullName: fullName,
    email: email,
    password: password,
    role: role,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const callBulkCreateUser = (data) => {
  const URL_BACKEND = "/api/Users/bulk";
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const deleteUserAPI = (id) => {
  const URL_BACKEND = `/api/Users/${id}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};
const editUserAPI = (id, fullName, email) => {
  const URL_BACKEND = `/api/Users/${id}`;
  const data = {
    fullName,
    email,
    isActive: true,
  };
  const res = axios.put(URL_BACKEND, data);
  return res;
};
const editUserStatusAPI = (id, fullName, email, isActive) => {
  const URL_BACKEND = `/api/Users/${id}`;
  const data = {
    fullName,
    email,
    isActive: isActive,
  };
  const res = axios.put(URL_BACKEND, data);
  return res;
};
const callDashboardUsersAPI = () => {
  const URL_BACKEND = `/api/Users/statistics`;
  const res = axios.get(URL_BACKEND);
  return res;
};

const callDashboardClassesAPI = () => {
  const URL_BACKEND = `/api/Classes/statistics`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListTeacherAPI = () => {
  const URL_BACKEND = `/api/Users?role=Teacher`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListStudentAPI = () => {
  const URL_BACKEND = `/api/Users?role=Student`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListClassAPI = (query) => {
  const URL_BACKEND = `/api/Classes?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const createClassAPI = (className, classCode, description) => {
  const URL_BACKEND = `/api/Classes`;
  const data = {
    className,
    classCode,
    description,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const deleteClassAPI = (id) => {
  const URL_BACKEND = `/api/Classes/${id}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};
const callAddTeachersToClassAPI = (classId, data) => {
  console.log("API Service - Adding teachers to class:", classId, data);
  const URL_BACKEND = `/api/Classes/${classId}/members/teachers`;
  const dataTeachers = {
    userIds: data,
  };
  const res = axios.post(URL_BACKEND, dataTeachers);
  return res;
};
const callAddStudentsToClassAPI = (classId, data) => {
  const URL_BACKEND = `/api/Classes/${classId}/members/students`;
  const dataStudents = {
    userIds: data,
  };
  const res = axios.post(URL_BACKEND, dataStudents);
  return res;
};
const callListTeacherOnClassesAPI = (id) => {
  const URL_BACKEND = `/api/Classes/${id}/teachers`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListStudentOnClassesAPI = (id) => {
  const URL_BACKEND = `/api/Classes/${id}/students`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const editClassesAPI = (id, className, description) => {
  const URL_BACKEND = `/api/Classes/${id}`;
  const data = {
    className,
    description,
    isActive: true,
  };
  const res = axios.put(URL_BACKEND, data);
  return res;
};
const deleteMemberOutClassAPI = (id, userId) => {
  const URL_BACKEND = `/api/Classes/${id}/members/${userId}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};

const callListMyClassesAPI = () => {
  const URL_BACKEND = `/api/Classes/my-classes`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListMaterialsByClassAPI = (classId, query) => {
  const URL_BACKEND = `/api/classes/${classId}/materials?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};

const callListBookAPI = (query) => {
  const URL_BACKEND = `/api/v1/book?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callListCategoryAPI = () => {
  const URL_BACKEND = "/api/v1/database/category";
  const res = axios.get(URL_BACKEND);
  return res;
};
const callUploadBookImg = (fileImg) => {
  const bodyFormData = new FormData();
  bodyFormData.append("fileImg", fileImg);
  return axios({
    method: "post",
    url: "/api/v1/file/upload",
    data: bodyFormData,
    headers: {
      "Content-Type": "multipart/form-data",
      "upload-type": "book",
    },
  });
};
const createBookAPI = (
  thumbnail,
  slider,
  mainText,
  author,
  price,
  sold,
  quantity,
  category
) => {
  const URL_BACKEND = "/api/v1/book";
  const data = {
    thumbnail: thumbnail,
    slider: slider,
    mainText: mainText,
    author: author,
    price: price,
    sold: sold,
    quantity: quantity,
    category: category,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const deleteBookAPI = (id) => {
  const URL_BACKEND = `/api/v1/book/${id}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};
const editBookAPI = (
  _id,
  thumbnail,
  slider,
  mainText,
  author,
  price,
  sold,
  quantity,
  category
) => {
  const URL_BACKEND = `/api/v1/book/${_id}`;
  const data = {
    thumbnail: thumbnail,
    slider: slider,
    mainText: mainText,
    author: author,
    price: price,
    sold: sold,
    quantity: quantity,
    category: category,
  };
  const res = axios.put(URL_BACKEND, data);
  return res;
};
const getBookAPI = (id) => {
  const URL_BACKEND = `/api/v1/book/${id}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callOrderAPI = (data) => {
  const URL_BACKEND = `/api/v1/order`;
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const callOrderHistory = () => {
  const URL_BACKEND = `/api/v1/history`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callUpdateAvatar = (fileImg) => {
  const bodyFormData = new FormData();
  bodyFormData.append("fileImg", fileImg);
  return axios({
    method: "post",
    url: "/api/v1/file/upload",
    data: bodyFormData,
    headers: {
      "Content-Type": "multipart/form-data",
      "upload-type": "avatar",
    },
  });
};
const callUpdateUserInfo = (_id, phone, fullName, avatarUser) => {
  const URL_BACKEND = `/api/v1/user`;
  const data = {
    _id,
    phone,
    fullName,
    avatar: avatarUser,
  };
  const res = axios.put(URL_BACKEND, data);
  return res;
};
const callOnChangePassWord = (email, oldpass, newpass) => {
  const URL_BACKEND = `/api/v1/user/change-password`;
  const data = {
    email,
    oldpass,
    newpass,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};

const callOrderApi = (query) => {
  const URL_BACKEND = `/api/v1/order?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callFetchDashBoard = () => {
  const URL_BACKEND = `/api/v1/database/dashboard`;
  const res = axios.get(URL_BACKEND);
  return res;
};
export {
  registerUserAPI,
  loginUserAPI,
  callFetchAccount,
  callLogout,
  callListUserAPI,
  createUserAPI,
  callBulkCreateUser,
  deleteUserAPI,
  editUserAPI,
  callListBookAPI,
  callListCategoryAPI,
  callUploadBookImg,
  createBookAPI,
  deleteBookAPI,
  editBookAPI,
  getBookAPI,
  callOrderAPI,
  callOrderHistory,
  callUpdateAvatar,
  callUpdateUserInfo,
  callOnChangePassWord,
  callOrderApi,
  callFetchDashBoard,
  editUserStatusAPI,
  callDashboardUsersAPI,
  callDashboardClassesAPI,
  callListClassAPI,
  callListTeacherAPI,
  callListStudentAPI,
  createClassAPI,
  deleteClassAPI,
  callAddTeachersToClassAPI,
  callAddStudentsToClassAPI,
  callListTeacherOnClassesAPI,
  callListStudentOnClassesAPI,
  deleteMemberOutClassAPI,
  editClassesAPI,
  callListMyClassesAPI,
  callListMaterialsByClassAPI,
};
