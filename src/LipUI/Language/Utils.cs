using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LipUI.Language;
public static class Utils
{
    #region Helper
    internal static void Deconstruct(this KeyValuePair<string, string> dict, out string key, out string value)
    {
        key = dict.Key;
        value = dict.Value;
    }
    #endregion
    public record LanguageDescriptionItem(LangId Id, string Name);
    public enum LangId
    {
        Default,
        zh_Hans,
        en
    }
    public static LanguageDescriptionItem[] AvailableLanguages = new LanguageDescriptionItem[]
    {
        new (LangId.Default,"Default (System)"),
        new (LangId.zh_Hans,"简体中文"),
        new (LangId.en,"English")
    };
    public static async Task SwitchLanguage(LangId target)
    {
        if (target != CurrentLangId)
        {
            Global.Config.Language = target;
        }
        async Task LoadLang(LangId id)
        {
            if (id == LangId.zh_Hans)
            {//build-in language
                await PopulateLanguage(SerializeToDict(new()));
            }
            else
            {
                var dict = GetLangDictionaryFromResource(id);
                await PopulateLanguage(dict);
            }
        }
        if (target == LangId.Default)
            await LoadLang(GetSystemLanguage());
        else await LoadLang(target);
    }
    /// <summary>
    /// 获取系统语言
    /// </summary>
    /// <returns>语言ID</returns>
    private static LangId GetSystemLanguage()
    {
        var lang = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();
        if (lang == "zh-CN")
            return LangId.zh_Hans;
        if (lang.StartsWith("zh"))
            return LangId.zh_Hans;
        return LangId.en;
    }
    public static Dictionary<string, string> GetLangDictionaryFromResource(LangId target)
    {
        var res = target switch
        {
            LangId.en => LanguageFiles.en,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };
        string str;
        if (res.Length >= 3 && res[0] == 0xEF && res[1] == 0xBB && res[2] == 0xBF)
        {//UTF-8 with BOM
            str = Encoding.UTF8.GetString(res, 3, res.Length - 3);
        }
        else
        {//UTF-8 without BOM
            str = Encoding.UTF8.GetString(res);
        }
        var dict = DeserializeFromStr(str);
        return dict;
    }

    public static LangId CurrentLangId
    {
        get => Global.Config.Language;
        set => _=SwitchLanguage(value);
    }

    public static Model DeserializeFromDict(Dictionary<string, string> dict)
    {
        Model i18N = new();
        foreach (var (key, value) in dict)
        {
            if (GetProp(key, out _, out var set))
            {
                set(i18N, value);
            }
            else
            {
                Debug.WriteLine($"[Language] [DeserializeFromDict] Property {key} not found.");
            }
        }
        return i18N;
    }
    public static Dictionary<string, string> DeserializeFromStr(string langStr) =>
        (
            from x in langStr
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            where !string.IsNullOrWhiteSpace(x)
                  && !x.StartsWith("//")
                  && !x.StartsWith("#")
                  && x.Contains('=')
            select x.Split(new[] { '=' }, 2)
        )
        //split by "=" to get key and value
        //remove all element that has same key
        .GroupBy(x => x[0].Trim(), (key, group) =>
        {
            var gp = group.ToArray();
#if DEBUG
            //check if only has one value ,otherwise write log
            if (gp.Count() > 1)
            {
                //Duplicate key
                Debug.WriteLine("[Language] [DeserializeFromStr] Duplicate key " + key + " in language");
                //print all value
                foreach (var item in gp)
                {
                    Debug.WriteLine("\t=" + item[1]);
                }
            }
#endif
            //get last element
            return new { key, value = gp.Last()[1] };
        })
        .ToDictionary(x => x.key, x => x.value.Replace("\\n", "\n"));

    internal static string SerializeToStr(Dictionary<string, string> dict)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in dict)
        {
            sb.Append(key);
            sb.Append('=');
            sb.Append(value.Replace("\n", "\\n"));
            sb.AppendLine();
        }
        return sb.ToString();
    }
    public static Dictionary<string, string> SerializeToDict(Model i18n)
    {
        var type = typeof(Model);
        var properties = type.GetProperties();
        var dict = new Dictionary<string, string>();
        foreach (var property in properties)
        {
            var value = property.GetValue(i18n);
            if (value is not string str)
            {
                Debug.WriteLine($"[Language] [SerializeToDict] value '{value ?? "null"}' is not string");
                continue;
            }
            var key = property.Name;
            dict.Add(key, str);
        }
        return dict;
    }
    ////缓存模式，减少反射速度更快，但是多占用了点内存
    //private static Lazy<Dictionary<string, PropertyInfo>> allProp =
    //    new(() =>
    //        (from x in typeof(Model).GetProperties()
    //         where x.CanRead && x.CanWrite && x.PropertyType == typeof(string)
    //         select x)
    //        .ToDictionary(x => x.Name.Trim(), x => x));
    //private static bool GetProp(string key, out PropertyInfo prop)
    //{
    //    return allProp.Value.TryGetValue(key.Trim(), out prop);
    //}
    private static bool GetProp(string key,
        [NotNullWhen(true)] out Func<Model, string>? get,
        [NotNullWhen(true)] out Action<Model, string>? set)
    {
        var prop = typeof(Model).GetProperty(key.Trim());
        if (prop is null)
        {
            get = null;
            set = null;
            return false;
        }
        get = x => (string)prop.GetValue(x)!;
        set = (x, v) =>
        {
            prop.SetValue(x, v);
            x.OnPropertyChanged(key);
        };
        return true;
    }

    public static async Task PopulateLanguage(Dictionary<string, string> dict)
    {
        var i18N = Global.I18N;
        foreach (var (key, value) in dict)
        {
            if (GetProp(key, out _, out var set))
            {
                await Global.DispatcherInvokeAsync(() =>
                {
                    set(i18N, value);
                });
            }
            else
            {
                Debug.WriteLine($"[Language] [PopulateLanguage] key '{key}' not found in Language.Model");
            }
        }
    }
}