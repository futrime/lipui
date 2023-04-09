open System.Diagnostics
open System.IO
open type System.Environment
open System.Text.RegularExpressions
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
let join(s:string)(v:System.Collections.Generic.IEnumerable<string>)=
    (s,v)|>System.String.Join
let build=Path.Combine(__SOURCE_DIRECTORY__,"LipWeb","dist")
let wwwroot=Path.Combine(__SOURCE_DIRECTORY__,"src","LipWebApi","wwwroot")
let fixname(s:string)=
    s
        .Replace("-","_")
        .Replace(".","_")
if wwwroot|>Directory.Exists then
    let sb=System.Text.StringBuilder()
    Directory.Delete(wwwroot,true)
    wwwroot|>Directory.CreateDirectory|>ignore
    for x in (build,"*.*",SearchOption.AllDirectories)|>Directory.GetFiles do
        let relativePath =x.[build.Length..].TrimStart(Path.DirectorySeparatorChar)
        let isText=Array.contains (relativePath|>Path.GetExtension) [|".js";".html";".css"|]
        let data=x|>File.ReadAllBytes 
        let className=relativePath|>Path.GetFileName|>fixname;
        let target=Path.Combine(wwwroot,relativePath|>Path.GetDirectoryName,className+".cs")
        let dir=target|>Path.GetDirectoryName
        if dir|>Directory.Exists|>not then
            dir|>Directory.CreateDirectory|>ignore
        let sep=NewLine+"\"\"\""+NewLine
        let mutable count=0;
        let datacs=
            if isText then
                $"""System.Text.Encoding.UTF8.GetBytes({sep}{System.Text.Encoding.UTF8.GetString(data).Replace(";",";"+sep+"+\"\"\""+NewLine)}{sep.TrimEnd()})"""
            else
                $"""{{{join "," ([for x in data do
                                    count<-count+1
                                    "0x"+x.ToString("X2") + (if (count % 10) = 0 then NewLine else "")])}}}""" 
        printfn "%s" className
        (target,
            $"""
namespace LipWebApi.wwwroot;
public static class {className}{{
    public static byte[] data = {datacs};
}}
                """)|>File.WriteAllText 
        printfn "Copied %s" relativePath
        sb. Append("{\""+relativePath.Replace(Path.DirectorySeparatorChar,'/')+"\"") |>ignore  
        sb.Append(", () => ") |>ignore 
        sb.Append(className+".data") |>ignore 
        sb.AppendLine("},")|>ignore
    File.WriteAllText(Path.Combine(wwwroot,"wwwMap.cs"),$"""
using System;
using System.Collections.Generic;
using System.Text;

namespace LipWebApi.wwwroot;
internal static class wwwMap
{{
    internal static Dictionary<string, Func<byte[]>> dict = new()
    {{
        {sb}
    }};
}}
    """)
