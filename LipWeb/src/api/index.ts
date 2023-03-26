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
    token: string
  ): Promise<ApiFailedResult | ApiAuthResult> {
    try {
      this.verifyToken();
      const result = await axios.post("/auth", { username, token });
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
