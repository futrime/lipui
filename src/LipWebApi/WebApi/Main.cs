using System.Threading.Tasks;
using HttpServerLite;

namespace LipWebApi.WebApi;
public class Main
{
    public static async Task SendResult(HttpResponse response, object item)
    {
        string result = BuildResult(item);
        response.StatusCode = 200;
        response.ContentLength = result.Length;
        response.ContentType = "application/json"; 
        await response.SendAsync(result);
    }
    static string BuildResult(object item)
    {
#if DEBUG
        return Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented);
#else
        return Newtonsoft.Json.JsonConvert.SerializeObject(item);
#endif
    }
    /// <summary>
    /// 验证服务端是否连接通畅
    /// </summary>
    [StaticRoute(HttpMethod.GET, "/ping")]
    public static async Task Ping(HttpContext ctx)
    {
        await SendResult(ctx.Response, new { success = true });
    }
    //[ParameterRoute(HttpMethod.GET, "/{version}/api/{id}")]
    //public static async Task MyParameterRoute(HttpContext ctx)
    //{
    //    string resp = "Hello from parameter route version " + ctx.Request.Url.Parameters["version"] + " for ID " + ctx.Request.Url.Parameters["id"];
    //    ctx.Response.StatusCode = 200;
    //    ctx.Response.ContentType = "text/plain";
    //    ctx.Response.ContentLength = resp.Length;
    //    await ctx.Response.SendAsync(resp);
    //    return;
    //}
    //[DynamicRoute(HttpMethod.GET, "^/dynamic/\\d+$")]
    //public static async Task MyDynamicRoute(HttpContext ctx)
    //{
    //    string resp = "Hello from the dynamic route";
    //    ctx.Response.StatusCode = 200;
    //    ctx.Response.ContentType = "text/plain";
    //    ctx.Response.ContentLength = resp.Length;
    //    await ctx.Response.SendAsync(resp);
    //    return;
    //}
}
