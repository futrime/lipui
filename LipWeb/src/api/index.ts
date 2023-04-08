export interface ApiResult {
  success: boolean;
}
export interface ApiFailedResult extends ApiResult {
  success: false;
  message: string;
}
export interface ApiSuccessResult extends ApiResult {
  success: true;
}
export interface ApiAuthResult extends ApiSuccessResult {
  token: string;
}
import { useGlobal } from "@/store";
import axiosbase from "axios";
import md5 from "./tools/md5";
import { ToothItemResult } from "./models";
const axios = axiosbase.create();
axios.interceptors.request.use(function (config) {
  // 在发送请求之前做些什么
  const store = useGlobal();
  if (!store.token) {
    throw new Error("No token");
  }
  config.headers.Authorization = store.token;
  return config;
});

// 添加响应拦截器
axios.interceptors.response.use(
  function (response) {
    // 2xx 范围内的状态码都会触发该函数。
    if (response.data.error === true) {
      // useGlobal().message = response.data.message;
      throw new Error(response.data.message);
    }
    return response;
  },
  function (error) {
    //错误码2xx意外
    // useGlobal().message = error.toString();
    return Promise.reject(error);
  }
);
const api = {
  base: {
    setAxiosConfig(baseURL: string) {
      axiosbase.defaults.baseURL = baseURL;
      axios.defaults.baseURL = baseURL;
    },
    // verifyToken() {
    //   const store = useGlobal();
    //   if (!store.token) {
    //     throw new Error("No token");
    //   }
    //   return (axios.defaults.headers.common["Authorization"] = store.token);
    // },
    async verifyBackend(): Promise<boolean> {
      const result = await axiosbase.get("/ping");
      return result.data.success;
    },
    async auth(
      username: string,
      password: string
    ): Promise<ApiFailedResult | ApiAuthResult> {
      try {
        console.log("login : " + username);
        const passwordMd5 = md5(password);
        const result = await axiosbase.post("/auth", {
          username,
          passwordMd5,
        });
        console.log("login result : " + result);
        return result.data as ApiAuthResult;
      } catch (error) {
        return {
          success: false,
          message: (error as Error).toString(),
        };
      }
    },
  },
  async getVersion(): Promise<{
    lip: string;
    backend: string;
  }> {
    const result = await axios.get("/version");
    return result.data;
  },
  async getWorkingDirectory(): Promise<{
    directories: { name: string; value: string }[];
    current: string;
  }> {
    const result = await axios.get("/directories");
    return result.data;
  },
  async setWorkingDirectory(dir: string): Promise<{
    success: true;
    value: string;
    directories: { name: string; value: string }[];
  }> {
    const result = await axios.post("/directory", dir);
    return result.data;
  },
  async getToothList(): Promise<ToothItemResult> {
    const result = await axios.get("/toothlist");
    return result.data;
  },
};
export default api;
