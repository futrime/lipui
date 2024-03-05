using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

using var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger("AutoUpdate");

Option<AutoUpdate.Type> option = new("--type", "The type of update");
Option<string?> lipPath = new("--lip-path", "The path to your Lip.exe");
Option<string?> workingDir = new("--working-dir", "The path to your executing program");

void Lipui_Handler(string? lipPath, string? workingDir)
{
    if (lipPath is null || workingDir is null)
    {
        logger.LogError("--lip-path and --working-dir are required");
        return;
    }

    if (File.Exists(lipPath) is false)
        logger.LogError("Lip.exe not found");

    if (Directory.Exists(workingDir) is false)
        logger.LogError("LipUI directory not found");

    var autoupdateDir = Path.Combine(workingDir, ".autoupdate");
    if (Directory.Exists(autoupdateDir))
        Directory.Delete(autoupdateDir, true);
    Directory.CreateDirectory(autoupdateDir);

    File.WriteAllText(Path.Combine(autoupdateDir, "current_lipui_path.txt"), workingDir);
    logger.LogInformation("Wrote {workingDir} to {autoupdateDir}", workingDir, autoupdateDir);

    var process = new Process()
    {
        StartInfo = new(lipPath)
        {
            WorkingDirectory = workingDir,
            Arguments = "install --yes --force-reinstall --no-dependencies github.com/lippkg/LipUI",
            WindowStyle = ProcessWindowStyle.Normal,
        }
    };
    logger.LogInformation("Running: {lipPath} install github.com/lippkg/LipUI", lipPath);
    process.Start();
    Environment.Exit(0);
}

void Lip_Handler()
{
    var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
    var path = Path.Combine(currentDir.FullName, "current_lipui_path.txt");

    if (File.Exists(path) is false)
        logger.LogError("LipUI path not found");

    var lipuiWorkingDir = File.ReadAllText(path);
    File.Delete(path);


    foreach (var file in currentDir.EnumerateFiles())
    {
        File.Move(file.FullName, Path.Combine(lipuiWorkingDir, file.Name), true);
        logger.LogInformation("Moved {file} to {lipuiWorkingDir}", file.Name, lipuiWorkingDir);
    }

    foreach (var dir in currentDir.EnumerateDirectories())
    {
        var temp = Path.Combine(lipuiWorkingDir, dir.Name);
        if (Directory.Exists(temp))
            Directory.Delete(temp, true);
        Directory.Move(dir.FullName, temp);
        logger.LogInformation("Moved {dir} to {lipuiWorkingDir}", dir.Name, lipuiWorkingDir);
    }

    logger.LogInformation("LipUI updated");

    Process.Start(Path.Combine(lipuiWorkingDir, "LipUI.exe"));
};

RootCommand rootCommand = [option, lipPath, workingDir];
rootCommand.SetHandler((AutoUpdate.Type type, string? lipPath, string? workingDir) =>
{
    switch (type)
    {
        case AutoUpdate.Type.lip_postinstall:
            Lip_Handler();
            break;
        case AutoUpdate.Type.lipui_autoupdate:
            Lipui_Handler(lipPath, workingDir);
            break;
    }
}, option, lipPath, workingDir);

await Task.Delay(2000);

return await rootCommand.InvokeAsync(args);
