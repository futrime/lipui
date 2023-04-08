using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HttpServerLite;
using LipWebApi.Models;
using Newtonsoft.Json;

namespace LipWebApi;
public class Launcher
{
    private static string CalcMd5(string sDataIn)
    {
        var md5 = MD5.Create();
        byte[] bytHash = md5.ComputeHash(Encoding.UTF8.GetBytes(sDataIn));
        md5.Clear();
        var sb = new StringBuilder();
        for (int i = 0, loopTo = bytHash.Length - 1; i <= loopTo; i++)
        {
            sb.Append(bytHash[i].ToString("X").PadLeft(2, '0'));
        }
        return sb.ToString();
    }
    public static Webserver? Server;
    public static AuthData Auth = default!;
    public static ConfigData Config = default!;
    public static UsersData UsersData = default!;
    private static void LoadData<T>(string name, ref T data) where T : new()
    {
        var path = Path.Combine(Global.ConfigFolder, name);
        data = File.Exists(path) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path))! : new T();
    }
    private static void SaveData<T>(string name, T data)
    {
        var path = Path.Combine(Global.ConfigFolder, name);
        string dataStr = JsonConvert.SerializeObject(data);
        if (Path.GetDirectoryName(path) is { } dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, dataStr);
        }
    }
    private const string PathAuth = "auth.json";
    private const string PathUsersData = "users.json";
    private const string PathConfig = "config.json";
    public static void LoadUsersData() => LoadData(PathUsersData, ref UsersData);
    public static void SaveUsersData() => SaveData(PathUsersData, UsersData);
    public static void LoadAuth() => LoadData(PathAuth, ref Auth);
    public static void SaveAuth() => SaveData(PathAuth, Auth);
    public static void LoadConfig() => LoadData(PathConfig, ref Config);
    public static void SaveConfig() => SaveData(PathConfig, Config);
    private static Func<string, LipNETWrapper.ILipWrapper> LipFunc = default!;
    public static LipNETWrapper.ILipWrapper GetLip(string user)
    {
        if (!UsersData.UserToDirectory.TryGetValue(user, out var workingDir))
        {
            return LipFunc(UsersData.WorkingDirectories.FirstOrDefault()?.Directory ??
                           throw new NullReferenceException("Working directory not found"));
        }
        return LipFunc(workingDir);
    }
    public static void SetUserWorkingDir(string user, string workingDir)
    {
        if (UsersData.UserToDirectory.ContainsKey(user))
        {
            UsersData.UserToDirectory[user] = workingDir;
        }
        else
        {
            UsersData.UserToDirectory.TryAdd(user, workingDir);
        }
    }
    public static string backendInfo = "";
    public void UpdateWorkingDir(IEnumerable<UsersData.DirectoryInfo> workingDirs)
    {
        UsersData.WorkingDirectories.Clear();
        UsersData.WorkingDirectories.AddRange(workingDirs);
        SaveUsersData();
    }
    public Launcher(Func<string, LipNETWrapper.ILipWrapper> getLip, string v)
    {
        LoadConfig();
        LoadAuth();
        LoadUsersData();
        LipFunc = getLip;
        backendInfo = v;
    }
    public static void WriteLine(string v)
    {
        Server?.Events.Logger(v);
    }
    public Task Load(Action<object> output, bool debug = true)
    {
        return Task.Run([MethodImpl(MethodImplOptions.Synchronized)] () =>
        {
            var host = Config.Host;
            ushort port = Config.Port;
            if (Server is not null)
            {
                Server.Stop();
                Server.Dispose();
                Server = null;
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
            SaveUsersData();
        });
    }
    public Task Stop()
    {
        return Task.Run(() =>
        {
            Server?.Stop();
            Server?.Dispose();
            Server = null;
        });
    }
    public static bool AddUser(string? username, string? password)
    {
        lock (Auth)
        {
            if (username is null || password is null) return false;
            if (Auth.ContainsKey(username))
            {
                return false;
            }
            string passwordMd5 = CalcMd5(password);
            //if (Auth.ContainsKey(username))
            //    DeleteUser(username);
            if (!Auth.ContainsKey(username))
            {
                Auth.Add(username, passwordMd5);
            }
            else
            {
                Auth[username] = passwordMd5;
            }
            // AuthManager.RefreshToken(username);
            SaveAuth();
            return true;
        }
    }
    public static bool DeleteUser(string? username)
    {
        lock (Auth)
        {
            if (username is null) return false;
            var result = Auth.Remove(username);
            SaveAuth();
            return result;
        }
    }
}
