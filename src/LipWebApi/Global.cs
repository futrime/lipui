using System;
using System.IO;

namespace LipWebApi
{
    internal class Global
    {
        internal static readonly string ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".lip", "config", "lipweb");
    }
}
