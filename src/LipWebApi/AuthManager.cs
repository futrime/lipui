using HttpServerLite;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static LipWebApi.WebApi;
namespace LipWebApi;
internal static class AuthManager
{
    /// <summary>
    /// key : token
    /// value : expired time
    /// </summary>
    private static readonly ConcurrentDictionary<string, DateTime> _validTokens = new();
    /// <summary>
    /// key : username
    /// value : token
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> _userTokens = new();
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private static void StoreToken(string token)
    {
        RemoveExpiredTokens();
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
    private static string GenerateToken()
    {
        var bytes = new byte[16];
        _rng.GetBytes(bytes);
        return new Guid(bytes).ToString();
    }
    public static bool CheckToken(string token)
    {
        return _validTokens.TryGetValue(token, out DateTime expiration) && expiration > DateTime.UtcNow;
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
    [StaticRoute(HttpMethod.GET, "/auth")]
    public static async Task Auth(HttpContext ctx)
    {
        var username = ctx.Request.Query.Elements["username"];
        var passwordMd5 = ctx.Request.Query.Elements["passwordMd5"];
        bool CheckUsernameAndPassword()//检查用户名是否有效
        {
            if (_userTokens.TryGetValue(username, out string token))
            {
                if (_validTokens.TryGetValue(token, out DateTime expiration) && expiration > DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }
        bool isValid = CheckUsernameAndPassword();
        if (isValid)
        {
            string token = GenerateToken();
            StoreToken(token);
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
                message = "Invalid username or password"
            });
        }
    }
}