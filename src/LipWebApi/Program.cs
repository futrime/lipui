using System;
using LipNETWrapper;
using LipWebApi;

var (success, path) = LipNETWrapper.Utils.TryGetLipFromPath();
var launcher = new Launcher(workingDir =>
    success
        ? new LipConsoleWrapper(path!, workingDir)
        : new LipConsoleWrapper(workingDir: workingDir), "LipWebConsole");
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