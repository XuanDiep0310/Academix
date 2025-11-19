import instance from "../utils/axios-customize";
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

const callListQuestionBankAPI = (query) => {
  const URL_BACKEND = `/api/Questions?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callCreateQuestionAPI = (payload) => {
  const URL_BACKEND = `/api/Questions`;
  return axios.post(URL_BACKEND, payload);
};
const callBulkCreateQuestionAPI = (payload) => {
  const URL_BACKEND = `/api/Questions/bulk`;
  const data = {
    questions: payload,
  };
  return axios.post(URL_BACKEND, data);
};
const deleteQuestionAPI = (id) => {
  const URL_BACKEND = `/api/Questions/${id}`;
  const res = axios.delete(URL_BACKEND);
  return res;
};
const editQuestionAPI = (id, payload) => {
  const URL_BACKEND = `/api/Questions/${id}`;
  const res = axios.put(URL_BACKEND, payload);
  return res;
};
const callListMaterialsByClassAPI = (classId, query) => {
  const URL_BACKEND = `/api/classes/${classId}/materials?${query}`;
  const res = axios.get(URL_BACKEND);
  return res;
};
const callUploadMaterialAPI = (classId, formData) => {
  return axios.post(`/api/classes/${classId}/materials/upload`, formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
const callDownloadMaterialAPI = (classId, materialId) => {
  return instance.get(
    `/api/classes/${classId}/materials/${materialId}/download`,
    {
      responseType: "blob",
    }
  );
};

const callListMyClassesAPI = () => {
  const URL_BACKEND = `/api/Classes/my-classes`;
  const res = axios.get(URL_BACKEND);
  return res;
};

const callListExamsByClassAPI = (classId, queryString) => {
  const URL_BACKEND = `/api/classes/${classId}/exams${
    queryString ? `?${queryString}` : ""
  }`;
  return axios.get(URL_BACKEND);
};

const callCreateExamAPI = (classId, body) => {
  const URL_BACKEND = `/api/classes/${classId}/exams`;
  return axios.post(URL_BACKEND, body);
};

const callUpdateExamAPI = (classId, examId, body) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}`;
  return axios.put(URL_BACKEND, body);
};

const callDeleteExamAPI = (classId, examId) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}`;
  return axios.delete(URL_BACKEND);
};

const callPublishExamAPI = (classId, examId) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}/publish`;
  return axios.patch(URL_BACKEND);
};
const callGetExamQuestionsAPI = (classId, examId) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}/questions`;
  return axios.get(URL_BACKEND);
};
const callUpsertExamQuestionsAPI = (classId, examId, body) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}/questions`;
  return axios.post(URL_BACKEND, body);
};

const callDeleteExamQuestionAPI = (classId, examId, questionId) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}/questions/${questionId}`;
  return axios.delete(URL_BACKEND);
};
export const callMaterialsStatisticsGlobalAPI = () => {
  return axios.get("/api/materials/statistics");
};

const callMaterialsStatisticsAPI = (classId) => {
  return axios.get(`/api/classes/${classId}/materials/statistics`);
};
// Lấy danh sách bài kiểm tra mà học sinh được làm trong 1 lớp
export const callStudentListExamsByClassAPI = (classId) => {
  const URL_BACKEND = `/api/student/exams?classId=${classId}`;
  return axios.get(URL_BACKEND);
};

// Đã gửi ở trên nhưng nhắc lại cho đủ bộ:
export const callStudentStartExamAPI = (examId) => {
  const URL_BACKEND = `/api/student/exams/${examId}/start`;
  return axios.post(URL_BACKEND);
};

export const callStudentSaveAnswerAPI = (attemptId, body) => {
  // body: { questionId, selectedOptionId }
  const URL_BACKEND = `/api/student/exams/attempts/${attemptId}/answer`;
  return axios.post(URL_BACKEND, body);
};

export const callStudentSubmitAttemptAPI = (attemptId, body) => {
  // body: { answers: [{questionId, selectedOptionId}] }
  const URL_BACKEND = `/api/student/exams/attempts/${attemptId}/submit`;
  return axios.post(URL_BACKEND, body);
};

export const callStudentGetAttemptResultAPI = (attemptId) => {
  const URL_BACKEND = `/api/student/exams/attempts/${attemptId}/result`;
  return axios.get(URL_BACKEND);
};

export const callGetExamResultsAPI = (classId, examId, query) => {
  const URL_BACKEND = `/api/classes/${classId}/exams/${examId}/results?${query}`;
  return axios.get(URL_BACKEND);
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
  callListQuestionBankAPI,
  callCreateQuestionAPI,
  deleteQuestionAPI,
  editQuestionAPI,
  callDeleteExamQuestionAPI,
  callUpsertExamQuestionsAPI,
  callGetExamQuestionsAPI,
  callPublishExamAPI,
  callDeleteExamAPI,
  callUpdateExamAPI,
  callCreateExamAPI,
  callListExamsByClassAPI,
  callMaterialsStatisticsAPI,
  callUploadMaterialAPI,
  callDownloadMaterialAPI,
  callBulkCreateQuestionAPI,
};
