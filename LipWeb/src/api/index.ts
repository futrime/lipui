export interface ApiResult {
  success: boolean;
  message: string;
}
export interface ApiAuthResult extends ApiResult {
  token: string;
}
const api = {
  //  async   auth(username:string,token:string): ApiAuthResult{
  //         axios.post('/api/auth', {
  //             username: username,
  //             token: token
  //         })
  //     }
  //   },
};
export default api;
