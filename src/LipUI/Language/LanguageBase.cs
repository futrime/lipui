using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LipUI.Language
{
    internal class Utils
    {
        public static string GetLanguageName(string language)
        {
            return language switch
            {
                "zh-CN" => "简体中文",
                "en-US" => "English",
                _ => "Unknown",
            };
        }
    } 
}
