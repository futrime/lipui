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
import axios from "axios";
import md5 from "./tools/md5";
const api = {
  setAxiosConfig(baseURL: string) {
    axios.defaults.baseURL = baseURL;
  },
  verifyToken() {
    const store = useGlobal();
    if (!store.token) {
      throw new Error("No token");
    }
    return (axios.defaults.headers.common["Authorization"] = store.token);
  },
  async verifyBackend(): Promise<boolean> {
    const result = await axios.get("/ping");
    return result.data.success;
  },
  async auth(
    username: string,
    password: string
  ): Promise<ApiFailedResult | ApiAuthResult> {
    try {
      console.log("login : " + username);
      const passwordMd5 = md5(password);
      const result = await axios.post("/auth", {
        username: username,
        passwordMd5: passwordMd5,
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
};
export default api;
