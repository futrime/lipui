using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

using var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger("AutoUpdate");

var lipuiDirOption = new Option<string>("--lipui-dir", "LipUI directory")
{
    IsRequired = true,
};
var lipuiPid = new Option<int>("--lipui-pid", "LipUI process ID")
{
    IsRequired = true
};

RootCommand rootCommand = [lipuiDirOption, lipuiPid];
rootCommand.SetHandler((lipuiPath, pid) =>
{
    var lipuiProcess = Process.GetProcessById(pid);

    lipuiProcess.Kill();
    lipuiProcess.WaitForExit();
    logger.LogInformation("LipUI process {pid} exited", pid);

    var currentDir = new FileInfo(Environment.ProcessPath!).Directory ?? throw new InvalidOperationException("Current directory not found");

    var lipuiDir = new DirectoryInfo(lipuiPath);

    if (lipuiDir.FullName == currentDir.FullName)
    {
        logger.LogError("LipUI directory cannot be the same as the current directory");
        logger.LogError("lipUIDir: {lipuiDir.FullName} currentDir: {currentDir.FullName}", lipuiDir.FullName, currentDir.FullName);
        return;
    }

    var lipuiExeFile = new FileInfo(Path.Combine(lipuiDir.FullName, "LipUI.exe"));
    if (!lipuiExeFile.Exists)
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

    Process.Start(lipuiExeFile.FullName);

}, lipuiDirOption, lipuiPid);

await rootCommand.InvokeAsync(args);