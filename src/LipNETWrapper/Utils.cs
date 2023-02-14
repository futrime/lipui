using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LipNETWrapper
{
    public static class Utils
    {
        /// <summary>
        /// Get lip.exe Path
        /// </summary>
        /// <returns></returns>
        public static (bool success, string? path) TryGetLipFromPath()
        {
            var lipPath = Environment.GetEnvironmentVariable("PATH");
            var lipPaths = lipPath.Split(';');
            foreach (var lip in lipPaths)
            {
                if (File.Exists(Path.Combine(lip, "lip.exe")))
                {
                    return (true, lip);
                }
            }
            return (false, null);
        }
    }
}
