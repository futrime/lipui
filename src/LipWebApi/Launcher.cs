using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HttpServerLite;
using LipWebApi.Models;
using Newtonsoft.Json;

namespace LipWebApi;
public class Launcher
{
    public static Webserver? Server;
    public static AuthData Auth;
    public static ConfigData Config;
    private void LoadData<T>(string name, ref T data) where T : new()
    {
        var path = Path.Combine(Global.ConfigFolder, name);
        if (File.Exists(path))
        {
            data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path))!;
        }
        else
        {
            data = new T();
        }
    }
    private void SaveData<T>(string name, T data)
    {
        var path = Path.Combine(Global.ConfigFolder, name);
        string dataStr = JsonConvert.SerializeObject(data);
        File.WriteAllText(path, dataStr);
    }
    private const string PathAuth = "auth.json";
    private const string PathConfig = "config.json";
    public void LoadAuth() => LoadData(PathAuth, ref Auth);
    public void SaveAuth() => SaveData(PathAuth, Auth);
    public void LoadConfig() => LoadData(PathConfig, ref Config);
    public void SaveConfig() => SaveData(PathConfig, Config);
    public Launcher()
    {
        LoadConfig();
        LoadAuth();
    }
    public Task Load(Action<object> output, bool debug = true)
    {
        return Task.Run([MethodImpl(MethodImplOptions.Synchronized)] () =>
        {
            var host = Config.Host;
            short port = Config.Port;
            if (Server is not null)
            {
                Server.Stop();
                Server.Dispose();
            }

            Server = new Webserver(host, port, false, null, null,
                async Task (ctx) =>
                {
                    string resp = "404 - NotFound";
                    ctx.Response.StatusCode = 404;
                    ctx.Response.ContentLength = resp.Length;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.SendAsync(resp);
                });
            Server.Settings.Headers.Host = host;
            Server.Events.Logger = output;
            Server.Routes.Preflight = async ctx =>
            {
                ctx.Response.StatusCode = 200;
                var headerValue = ctx.Request.Headers.GetValues("Access-Control-Request-Headers");
                if (headerValue?.Length > 0)
                {
                    ctx.Response.Headers.Add("Access-Control-Allow-Headers", string.Join(", ", headerValue));
                }

                await ctx.Response.SendAsync(0);
            };
            //Server.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
            Server.Settings.Debug.Responses = debug;
            Server.Settings.Debug.Routing = debug;
            Server.Settings.Debug.Tcp = debug;
            Server.Settings.Debug.AccessControl = debug;
            Server.Settings.Debug.Connections = debug;
            Server.Events.Exception += (sender, e) =>
            {
                output(e.Exception);
            };
            //Server.Settings.Headers.AccessControlAllowOrigin = "http://localhost:3000/";
            //Server.Settings.Headers.AccessControlAllowHeaders = "http://localhost:3000/";
            Server.Start();
            SaveConfig();
            SaveAuth();
        });
    }
}
