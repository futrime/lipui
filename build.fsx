open System.Diagnostics
open System.IO

let executeCommand (command: string)(workingDir:string) =
    let processInfo = new ProcessStartInfo("cmd.exe", "/C " + command,WorkingDirectory=workingDir)
    processInfo.CreateNoWindow <- false
    processInfo.UseShellExecute <- false
    let proc = new Process()
    proc.StartInfo <- processInfo
    proc.Start() |> ignore
    proc.WaitForExit()
    proc.ExitCode
executeCommand "npm run build" (Path.Combine(__SOURCE_DIRECTORY__,"LipWeb"))
