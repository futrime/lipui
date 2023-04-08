using System.Linq;
using HttpServerLite;
using System.Threading.Tasks;
using static LipWebApi.WebApi.Main;
namespace LipWebApi.WebApi;
public static class LocalTooth
{
    [StaticRoute(HttpMethod.GET, "/toothlist")]
    public static async Task GetToothList(HttpContext ctx)
    {
        var (success, user) = await AuthManager.CheckRequest(ctx);
        if (success)
        {
            var (packages, message) = await Launcher.GetLip(user).GetAllPackagesAsync();
            await SendResult(ctx.Response, new
            {
                message, packages
            });
        }
    }
}