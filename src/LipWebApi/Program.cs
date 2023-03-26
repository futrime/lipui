using System;
using System.Threading.Tasks;
using HttpServerLite;

Webserver server = new Webserver("localhost", 9000, false, null, null, DefaultRoute);
server.Settings.Headers.Host = "https://localhost:9000";
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