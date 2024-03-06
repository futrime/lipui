using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

using var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger("AutoUpdate");

var lipuiDirOption = new Option<string>("--lipui-dir", "LipUI directory")
{
    IsRequired = true
};

RootCommand rootCommand = [lipuiDirOption];
rootCommand.SetHandler((lipuiPath) =>
{
    var lipuiDir = new DirectoryInfo(lipuiPath);
    var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

    if (lipuiDir.FullName == currentDir.FullName)
    {
        logger.LogError("LipUI directory cannot be the same as the current directory");
        return;
    }

    var lipuiExeFile = new FileInfo(Path.Combine(lipuiDir.FullName, "LipUI.exe"));
    if (lipuiExeFile.Exists is false)
    {
        logger.LogError("LipUI.exe file not found");
        return;
    }

    foreach (var dir in currentDir.EnumerateDirectories())
    {
        var temp = Path.Combine(lipuiDir.FullName, dir.Name);
        if (Directory.Exists(temp))
            Directory.Delete(temp, true);
        Directory.Move(dir.FullName, temp);
        logger.LogInformation("Moved {dir} to {lipuiDir}", dir.Name, lipuiDir.FullName);
    }

    foreach (var file in currentDir.EnumerateFiles())
    {
        File.Move(file.FullName, Path.Combine(lipuiDir.FullName, file.Name), true);
        logger.LogInformation("Moved {file} to {lipuiWorkingDir}", file.Name, lipuiDir.FullName);
    }

    Task.Delay(1000);

    Process.Start(lipuiPath);
    Environment.Exit(0);

}, lipuiDirOption);

rootCommand.Invoke(args);