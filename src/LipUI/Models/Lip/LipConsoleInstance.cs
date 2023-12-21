using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace LipUI.Models.Lip;

internal class LipConsoleInstance : IDisposable
{
    public Process? Process { get; private set; }
    public bool HasExited => Process?.HasExited ?? false;
    public int ExitCode => Process?.ExitCode ?? -1;
    private readonly CancellationToken _tk;

    public LipConsoleInstance(string exe, string? workingDir, string cmd, CancellationToken tk,
        Action<string> output, Action<string> outputErr, out Process process) : this(exe, workingDir, cmd, tk, output, outputErr)
    {
        process = this.Process!;
    }
    public LipConsoleInstance(string exe, string? workingDir, string cmd, CancellationToken tk,
        Action<string> output, Action<string> outputErr)
    {
        _tk = tk;
        Process = new()
        {
            StartInfo = new(exe, cmd)
            {
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDir ?? Path.GetDirectoryName(exe)
            }
        };
        Process.OutputDataReceived += (_, args) =>
        {
            if (!_tk.IsCancellationRequested)
                output(args.Data ?? "");
        };
        Process.ErrorDataReceived += (_, args) =>
        {
            if (!_tk.IsCancellationRequested)
                outputErr(args.Data ?? "");
        };
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
    }
    public void KillIfCanceled()
    {
        if (_tk.IsCancellationRequested)
        {
            try
            {
                Process?.Kill();
                Process = null;
            }
            catch { }
            _tk.ThrowIfCancellationRequested();
        }
    }

    public void Dispose()
    {
        try
        {
            if (Process is { HasExited: false })
                Process?.Kill();
        }
        catch { }
        Process?.Dispose();
        GC.SuppressFinalize(this);
    }

    ~LipConsoleInstance()
    {
        Dispose();
    }
}
//public class __LipConsole
//{
//    public __LipConsole(string executablePath = "lip.exe", string? workingDir = null)
//    {
//        ExecutablePath = executablePath;
//        WorkingPath = workingDir;
//    }
//    public string ExecutablePath { get; }
//    public string? WorkingPath { get; }
//    public async Task<int> Run(string cmd, Action<string>? output, CancellationToken tk = default)
//    {
//        var inst = new LipConsoleInstance(ExecutablePath, WorkingPath, cmd, tk, s => output?.Invoke(s), s => output?.Invoke(s));
//        while (!inst.HasExited)
//        {
//            await Task.Delay(100, tk);
//        }
//        tk.ThrowIfCancellationRequested();
//        return inst.ExitCode;
//    }
//    public async Task<int> RunWithInput(string cmd, Action<string, Action<string>>? output, CancellationToken tk = default)
//    {
//        Process? process = default;
//        var inst = new LipConsoleInstance(ExecutablePath, WorkingPath, cmd, tk,
//            s =>
//            {
//                output?.Invoke(s, s =>
//                {
//                    process?.StandardInput.WriteLine(s);
//                });
//            }, s =>
//            {
//                output?.Invoke(s, s =>
//                {
//                    process?.StandardInput.WriteLine(s);
//                });
//            }, out process);
//        while (!inst.HasExited)
//        {
//            await Task.Delay(100, tk);
//        }
//        tk.ThrowIfCancellationRequested();
//        return inst.ExitCode;
//    }
//    public async Task<int> Run(string cmd, Action<string> output, Action<string> outputError, CancellationToken tk = default)
//    {
//        var inst = new LipConsoleInstance(ExecutablePath, WorkingPath, cmd, tk, output, outputError);
//        while (!inst.HasExited)
//        {
//            await Task.Delay(100, tk);
//        }
//        tk.ThrowIfCancellationRequested();
//        return inst.ExitCode;
//    }
//    public async Task<string> RunString(string cmd, Action<string>? output = null, CancellationToken tk = default)
//    {
//        var sb = new StringBuilder();
//        var inst = new LipConsoleInstance(ExecutablePath, WorkingPath, cmd, tk,
//            s =>
//            {
//                sb.AppendLine(s); output?.Invoke(s);
//            }, s =>
//            {
//                sb.AppendLine(s); output?.Invoke(s);
//            });
//        while (!inst.HasExited)
//        {
//            try
//            {
//                await Task.Delay(100, tk);
//            }
//            catch
//            {
//                // ignored
//            }
//            inst.KillIfCanceled();
//        }
//        tk.ThrowIfCancellationRequested();
//        return sb.ToString();
//    }
//    public async Task<string> RunStringWithInput(string cmd, Action<string, Action<string>>? output = null, CancellationToken tk = default)
//    {
//        var sb = new StringBuilder();
//        Process? process = default;
//        var inst = new LipConsoleInstance(ExecutablePath, WorkingPath, cmd, tk,
//            s =>
//            {
//                sb.AppendLine(s);
//                output?.Invoke(s, s =>
//                {
//                    process?.StandardInput.WriteLine(s);
//                });
//            }, s =>
//            {
//                sb.AppendLine(s);
//                output?.Invoke(s, s =>
//                {
//                    process?.StandardInput.WriteLine(s);
//                });
//            }, out process);
//        while (!inst.HasExited)
//        {
//            try
//            {
//                await Task.Delay(100, tk);
//            }
//            catch
//            {
//                // ignored
//            }
//            inst.KillIfCanceled();
//        }
//        tk.ThrowIfCancellationRequested();
//        return sb.ToString();
//    }
//}
