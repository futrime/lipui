#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LipNETWrapper
{
    public class LipCommand
    {
        private readonly List<string> _commands = new();
        public static LipCommand Create()
        {
            return new LipCommand();
        }
        public static LipCommand Create(string cmd, bool quiet = false)
        {
            var instance = new LipCommand();
            if (quiet)
            {
                instance.Add("-q");
            }
            return instance.Add(cmd);
        }
        public LipCommand Add(string cmd)
        {
            _commands.Add(cmd);
            return this;
        }
        public LipCommand WithJson()
        {
            _commands.Add("--json");
            return this;
        }
        //public LipCommand Verbose()
        //{
        //    _commands.Add("--verbose");
        //    return this;
        //} 
        public static implicit operator string(LipCommand cmd) => string.Join(" ", cmd._commands);
        public override string ToString() => this;
        public static LipCommand operator +(LipCommand cmd, string s) => cmd.Add(s);
    }
    public class LipConsoleCommandInstance
    {
        private Process? _process;
        public bool HasExited => _process.HasExited;
        public int ExitCode => _process.ExitCode;
        private CancellationToken _tk;

        public LipConsoleCommandInstance(string exe, string? workingDir, string cmd, CancellationToken tk,
            Action<string> output, Action<string> outputErr, out Process process) : this(exe, workingDir, cmd, tk, output, outputErr)
        {
            process = _process!;
        }
        public LipConsoleCommandInstance(string exe, string? workingDir, string cmd, CancellationToken tk,
            Action<string> output, Action<string> outputErr)
        {
            _tk = tk;
            _process = new();
            _process.StartInfo = new(exe, cmd)
            {
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDir ?? Path.GetDirectoryName(exe)
            };
            _process.OutputDataReceived += (_, args) =>
            {
                if (!_tk.IsCancellationRequested)
                    output(args.Data);
            };
            _process.ErrorDataReceived += (_, args) =>
            {
                if (!_tk.IsCancellationRequested)
                    outputErr(args.Data);
            };
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }
        public void KillIfCanceled()
        {
            if (_tk.IsCancellationRequested)
            {
                try
                {
                    _process?.Kill();
                    _process = null;
                }
                catch
                {
                    // ignored
                }
                _tk.ThrowIfCancellationRequested();
            }
        }
        ~LipConsoleCommandInstance()
        {
            try
            {
                if (_process is { HasExited: false })
                {
                    _process?.Kill();
                }
            }
            catch
            {
                // ignored
            }
            _process?.Dispose();
        }
    }
    public class LipConsoleLoader
    {
        public LipConsoleLoader(string executablePath = "lip.exe", string? workingDir = null)
        {
            ExecutablePath = executablePath;
            WorkingPath = workingDir;
        }
        public string ExecutablePath { get; }
        public string? WorkingPath { get; }
        public async Task<int> Run(string cmd, Action<string>? output, CancellationToken tk = default)
        {
            var inst = new LipConsoleCommandInstance(ExecutablePath, WorkingPath, cmd, tk, s => output?.Invoke(s), s => output?.Invoke(s));
            while (!inst.HasExited)
            {
                await Task.Delay(100, tk);
            }
            tk.ThrowIfCancellationRequested();
            return inst.ExitCode;
        }
        public async Task<int> RunWithInput(string cmd, Action<string, Action<string>>? output, CancellationToken tk = default)
        {
            Process? process = default;
            var inst = new LipConsoleCommandInstance(ExecutablePath, WorkingPath, cmd, tk,
                s =>
                {
                    output?.Invoke(s, s =>
                    {
                        process.StandardInput.WriteLine(s);
                    });
                }, s =>
                {
                    output?.Invoke(s, s =>
                    {
                        process.StandardInput.WriteLine(s);
                    });
                }, out process);
            while (!inst.HasExited)
            {
                await Task.Delay(100, tk);
            }
            tk.ThrowIfCancellationRequested();
            return inst.ExitCode;
        }
        public async Task<int> Run(string cmd, Action<string> output, Action<string> outputError, CancellationToken tk = default)
        {
            var inst = new LipConsoleCommandInstance(ExecutablePath, WorkingPath, cmd, tk, output, outputError);
            while (!inst.HasExited)
            {
                await Task.Delay(100, tk);
            }
            tk.ThrowIfCancellationRequested();
            return inst.ExitCode;
        }
        public async Task<string> RunString(string cmd, Action<string>? output = null, CancellationToken tk = default)
        {
            var sb = new StringBuilder();
            var inst = new LipConsoleCommandInstance(ExecutablePath, WorkingPath, cmd, tk,
                s =>
                {
                    sb.AppendLine(s); output?.Invoke(s);
                }, s =>
                {
                    sb.AppendLine(s); output?.Invoke(s);
                });
            while (!inst.HasExited)
            {
                try
                {
                    await Task.Delay(100, tk);
                }
                catch
                {
                    // ignored
                }
                inst.KillIfCanceled();
            }
            tk.ThrowIfCancellationRequested();
            return sb.ToString();
        }
        public async Task<string> RunStringWithInput(string cmd, Action<string, Action<string>>? output = null, CancellationToken tk = default)
        {
            var sb = new StringBuilder();
            Process? process = default;
            var inst = new LipConsoleCommandInstance(ExecutablePath, WorkingPath, cmd, tk,
                s =>
                {
                    sb.AppendLine(s);
                    output?.Invoke(s, s =>
                    {
                        process.StandardInput.WriteLine(s);
                    });
                }, s =>
                {
                    sb.AppendLine(s);
                    output?.Invoke(s, s =>
                    {
                        process.StandardInput.WriteLine(s);
                    });
                }, out process);
            while (!inst.HasExited)
            {
                try
                {
                    await Task.Delay(100, tk);
                }
                catch
                {
                    // ignored
                }
                inst.KillIfCanceled();
            }
            tk.ThrowIfCancellationRequested();
            return sb.ToString();
        }
    }
}
