using HttpServerLite;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static LipWebApi.WebApi.Main;
namespace LipWebApi;
internal static class AuthManager
{
    /// <summary>
    /// key : token
    /// value : expired time
    /// </summary>
    private static readonly ConcurrentDictionary<Guid, DateTime> _validTokens = new();
    /// <summary>
    /// key : username
    /// value : token
    /// </summary>
    private static readonly ConcurrentDictionary<string, Guid> _userTokens = new();
    private static void StoreToken(string username,Guid token)
    {
        RemoveExpiredTokens();
        _userTokens[username] = token;
        _validTokens[token] = DateTime.UtcNow.AddHours(1);
    }
    private static void RemoveExpiredTokens()
    {
        var expiredTokens = _validTokens.Where(x => x.Value < DateTime.UtcNow).Select(x => x.Key).ToArray();
        if (expiredTokens.Length > 0)
        {
            foreach (var token in expiredTokens)
            {
                _validTokens.TryRemove(token, out _);
            }
            var expiredUsers = _userTokens.Where(x => expiredTokens.Contains(x.Value)).Select(x => x.Key).ToArray();
            foreach (var user in expiredUsers)
            {
                _userTokens.TryRemove(user, out _);
            }
        }
    }
    private static Guid GenerateToken()
    {
        return Guid.NewGuid();
    }
    private static bool CheckToken(Guid token)
    {
        return _validTokens.TryGetValue(token, out DateTime expiration) && expiration > DateTime.UtcNow;
    }
    public static async Task<(bool success, string user)> CheckRequest(HttpContext v)
    {
        try
        {
            //Launcher.WriteLine(v.Request.Headers["Authorization"]);
            var token = Guid.Parse(v.Request.Headers["Authorization"]);
            if (!CheckToken(token))
            {
                await SendResult(v.Response, new
                {
                    error = true,
                    message = "token not valid"
                });
                return (false, null!);
            }
            return (true, _userTokens.First(x => x.Value == token).Key);
        }
        catch (Exception e)
        {
            Launcher.WriteLine(e.ToString());
            await SendResult(v.Response, new
            {
                error = true,
                message = "auth error : " + e.Message
            });
            return (false, null!);
        }
    }
    //sample:
    //     http://locolhost:9000/auth?username=admin&passwordMd5=pwd
    //result(success):
    //      {
    //          "success": true,
    //          "token": "ba205422-20e0-4291-a97f-5536658df223"
    //      }
    //result(fail):
    //      {
    //          "success": false,
    //          "message": "Invalid username or password"
    //      }
    //{username: "a", passwordMd5: "03c7c0ace395d80182db07ae2c30f034"}
    [StaticRoute(HttpMethod.POST, "/auth")]
    public static async Task Auth(HttpContext ctx)
    {
        //string resp = "Hello from the dynamic route";
        //ctx.Response.StatusCode = 200;
        //ctx.Response.ContentType = "text/plain";
        //ctx.Response.ContentLength = resp.Length;
        //await ctx.Response.SendAsync(resp);
        //return;
        var raw = Encoding.UTF8.GetString(ctx.Request.DataAsBytes);
        var jobj = JObject.Parse(raw);
        var username = jobj.Value<string>("username")!;
        var passwordMd5 = jobj.Value<string>("passwordMd5")!;
        bool CheckUsernameAndPassword()//检查用户名和密码
        {
            //if (_userTokens.TryGetValue(username, out string token))//已经登录
            //{
            //    if (_validTokens.TryGetValue(token, out DateTime expiration) && expiration > DateTime.UtcNow)
            //    {
            //        //验证用户
            //        return true;
            //    }
            //}
            //_userTokens.TryRemove(username, out _);//移除旧验证
            if (Launcher.Auth.TryGetValue(username, out string? pwd))
            {
                if (string.Equals(pwd, passwordMd5, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        bool isValid = CheckUsernameAndPassword();
        Launcher.WriteLine(jobj.ToString());

        //bool isValid = true;
        if (isValid)
        {
            var token = GenerateToken();
            StoreToken( username,token);
            await SendResult(ctx.Response, new
            {
                success = true,
                token
            });
        }
        else
        {
            await SendResult(ctx.Response, new
            {
                success = false,
                message = "用户名或密钥无效"
            });
        }
    } 
    [StaticRoute(HttpMethod.POST, "/verify")]
    public static async Task VerifyToken(HttpContext ctx)
    {

    }
}