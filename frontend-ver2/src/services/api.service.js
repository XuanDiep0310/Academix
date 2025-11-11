import axios from "../utils/axios-customize";

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
  const URL_BACKEND = "/api/v1/auth/login";
  const data = {
    username: username,
    password: password,
    // delay: 2000,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const callFetchAccount = () => {
  const URL_BACKEND = "/api/v1/auth/account";
  return axios.get(URL_BACKEND);
};
const callLogout = () => {
  const URL_BACKEND = "/api/v1/auth/logout";
  return axios.post(URL_BACKEND);
};
const callListUserAPI = (query) => {
  const URL_BACKEND = `/api/v1/user?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};

const createUserAPI = (fullName, email, password, phone) => {
  const URL_BACKEND = "/api/v1/user";
  const data = {
    fullName: fullName,
    email: email,
    password: password,
    phone: phone,
  };
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const callBulkCreateUser = (data) => {
  const URL_BACKEND = "/api/v1/user/bulk-create";
  const res = axios.post(URL_BACKEND, data);
  return res;
};
const deleteUserAPI = (id) => {
  const URL_BACKEND = `/api/v1/user/${id}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};
const editUserAPI = (id, fullName, phone) => {
  const URL_BACKEND = `/api/v1/user/`;
  const data = {
    _id: id,
    fullName,
    phone,
  };
  const res = axios.put(URL_BACKEND, data);
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
};
