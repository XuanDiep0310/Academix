import axios from "axios";
import { Mutex } from "async-mutex";
import { getCookie } from "./cookie";
import NProgress from "nprogress";
const mutex = new Mutex();
const baseURL = import.meta.env.VITE_BACKEND_URL;

NProgress.configure({
  showSpinner: false,
  trickleSpeed: 100,
  // minimum: 0.1,
  // trickle: true,
  // // easing: "ease",
  // speed: 600,
});
const instance = axios.create({
  baseURL: baseURL,
  // withCredentials: true, // xét cookie
});
let activeRequests = 0;

instance.defaults.headers.common = {
  Authorization: `Bearer ${localStorage.getItem("access_token")} `,
};

const handleRefreshToken = async () => {
  return await mutex.runExclusive(async () => {
    const refreshToken = getCookie("refresh_token");
    const res = await instance.post("/api/Auth/refresh-token", {
      refreshToken: refreshToken,
    });
    if (res && res?.data) {
      cookieStore.set("refresh_token", res.data.refreshToken);
      return res.data.accessToken;
    }
    return null;
  });
};

// Add a request interceptor
instance.interceptors.request.use(
  function (config) {
    // Do something before request is sent
    if (activeRequests === 0) {
      NProgress.start();
    }
    activeRequests++;
    if (
      typeof window !== "undefined" &&
      window &&
      window.localStorage &&
      window.localStorage.getItem("access_token")
    ) {
      config.headers.Authorization =
        "Bearer " + window.localStorage.getItem("access_token");
    }
    return config;
  },
  function (error) {
    // Do something with request error

    return Promise.reject(error);
  }
);

const NO_RETRY_HEADER = "x-no-retry";

// Add a response interceptor
instance.interceptors.response.use(
  function (response) {
    activeRequests--;
    if (activeRequests === 0) {
      NProgress.done();
    }
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response && response.data ? response.data : response;
  },
  async function (error) {
    activeRequests--;
    if (activeRequests === 0) {
      NProgress.done();
    }
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error
    if (
      error.config &&
      error.response &&
      +error.response.status === 401 &&
      !error.config.headers[NO_RETRY_HEADER]
    ) {
      // await handleRefreshToken();
      const access_token = await handleRefreshToken();
      error.config.headers[NO_RETRY_HEADER] = "true";
      if (access_token) {
        error.config.headers["Authorization"] = `Bearer ${access_token} `;
        localStorage.setItem("access_token", access_token);
        return axios.request(error.config);
        // return instance.request(originalRequest);
      }
    }

    // if (
    //   error.config &&
    //   error.response &&
    //   +error.response.status === 400 &&
    //   error.config.url === "/api/v1/auth/refresh"
    // ) {
    //   window.location.href = "/login";
    // }

    return error?.response.data ?? Promise.reject(error);
  }
);
export default instance;
