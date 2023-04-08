using HttpServerLite;
using System.Threading.Tasks;
using static LipWebApi.WebApi.Main;
namespace LipWebApi.WebApi;
public static class LocalTooth
{
    [StaticRoute(HttpMethod.GET, "/local")]
    public static async Task Locol(HttpContext ctx)
    { 
        await SendResult(ctx.Response, new { success = true });
    }
}