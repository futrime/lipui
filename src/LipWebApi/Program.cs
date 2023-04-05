using System;
using LipWebApi;
var launcher= new Launcher();
launcher.Load(v =>
{
    foreach (var x in v.ToString().Replace("\r", "").Split('\n'))
    {
        Console.WriteLine(x);
    }
});
Console.WriteLine("LipUI started.");
Console.WriteLine("ENTER to exit");
Console.ReadLine();