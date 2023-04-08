
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServerLite;
using static LipWebApi.WebApi.Main;
namespace LipWebApi.WebApi;
public static class LipSettings
{
    [StaticRoute(HttpMethod.GET, "/version")]
    public static async Task GetVersion(HttpContext ctx)
    {
        var (success, user) = await AuthManager.CheckRequest(ctx);
        if (success)
        {
            await SendResult(ctx.Response, new
            {
                lip = await Launcher.GetLip(user).GetLipVersion(),
                backend = Launcher.backendInfo
            });
        }
    }
    [StaticRoute(HttpMethod.GET, "/directories")]
    public static async Task GetWorkingDirectory(HttpContext ctx)
    {
        var (success, user) = await AuthManager.CheckRequest(ctx);
        if (success)
        {
            await SendResult(ctx.Response, new
            {
                directories = from x in Launcher.UsersData.WorkingDirectories
                              select new { name = x.Name, value = x.Directory },
                current = Launcher.UsersData.UserToDirectory.TryGetValue(user, out var v)
                    ? v
                    : null
            });
        }
    }
    [StaticRoute(HttpMethod.POST, "/directory")]
    public static async Task SetWorkingDirectory(HttpContext ctx)
    {
        var (success, user) = await AuthManager.CheckRequest(ctx);
        if (success)
        {
            var dir = GetBody<string>(ctx);
            Launcher.SetUserWorkingDir(user, dir);
            await SendResult(ctx.Response, new
            {
                success = true,
                value = dir,
                directories = from x in Launcher.UsersData.WorkingDirectories
                              select new { name = x.Name, value = x.Directory }
            });
        }
    }
}
