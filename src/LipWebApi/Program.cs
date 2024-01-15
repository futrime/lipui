using HttpServerLite;
using System;
using System.Threading.Tasks;

Webserver server = new Webserver("localhost", 9000, false, null, null, DefaultRoute);
server.Settings.Headers.Host = "https://localhost:3000";
server.Events.Logger = Console.WriteLine;
server.Routes.Preflight = async (ctx) =>
{
    ctx.Response.StatusCode = 200;
    var headerValue = ctx.Request.Headers.GetValues("Access-Control-Request-Headers");
    if (headerValue?.Length > 0)
    {
        ctx.Response.Headers.Add("Access-Control-Allow-Headers", string.Join(", ", headerValue));
    }
    await ctx.Response.SendAsync(0);
};
//server.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
server.Settings.Debug.Responses = true;
server.Settings.Debug.Routing = true;
server.Settings.Debug.Tcp = true;
server.Settings.Debug.AccessControl = true;
server.Settings.Debug.Connections = true;
//server.Events.Exception += Events_Exception;

void Events_Exception(object sender, ExceptionEventArgs e)
{
    Console.WriteLine(e.Exception);
}

//server.Settings.Headers.AccessControlAllowOrigin = "http://localhost:3000/";
//server.Settings.Headers.AccessControlAllowHeaders = "http://localhost:3000/";
server.Start();
Console.WriteLine("HttpServerLite listening on http://localhost:9000");
Console.WriteLine("ENTER to exit");
Console.ReadLine();
static async Task DefaultRoute(HttpContext ctx)
{
    string resp = "404 - NotFound";
    ctx.Response.StatusCode = 404;
    ctx.Response.ContentLength = resp.Length;
    ctx.Response.ContentType = "text/plain";
    await ctx.Response.SendAsync(resp);
}