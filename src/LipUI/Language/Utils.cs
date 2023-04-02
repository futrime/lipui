using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace LipUI.Language
{
    internal static class Utils
    {
        private static void Deconstruct(this KeyValuePair<string, string> dict, out string key, out string value)
        {
            key = dict.Key;
            value = dict.Value;
        }
        public static string GetLanguageName(string language)
        {
            return language switch
            {
                "zh-CN" => "简体中文",
                "en-US" => "English",
                _ => "Unknown",
            };
        }
        public static Dictionary<string, string> SerializeLanguage()
        {
            var i18n = Global.I18N;
            var type = typeof(Model);
            var properties = type.GetProperties();
            var dict = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(i18n);
                if (value is not string str)
                {
                    Debug.WriteLine($"value '{value ?? "null"}' is not string");
                    continue;
                }
                var key = property.Name;
                dict.Add(key, str);
            }
            return dict;
        }
        public static async Task PopulateLanguage(Dictionary<string, string> dict)
        {
            var i18n = Global.I18N;
            var type = typeof(Model);
            foreach (var (key, value) in dict)
            {
                var prop = type.GetProperty(key);
                if (prop is null)
                {
                    Debug.WriteLine($"key '{key}' not found in Language.Model");
                }
                else
                {
                    await Global.DispatcherInvokeAsync(() =>
                    {
                        prop.SetValue(i18n, value);
                    });
                }
            }
        }
    }
}
