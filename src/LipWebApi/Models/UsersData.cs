using System.Collections.Concurrent;
using System.Collections.Generic;
namespace LipWebApi.Models;
public class UsersData
{
    public ConcurrentDictionary<string, string> UserToDirectory = new();
    public List<DirectoryInfo> WorkingDirectories = new();
    public class DirectoryInfo
    {
        public string Directory = string.Empty;
        public string Name = "undefined";
    }
}