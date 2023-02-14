using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using LipNETWrapper;
using LipUI.Models;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;

namespace LipUI
{
    internal static class Global
    {
        /// <summary>初始化全局变量</summary>
        internal static void Init()
        { 
            //从配置获取Lip路径
            if (!TryRefreshLipPath())
            {
                //todo 未找到Lip
            }
        }
        static bool TryRefreshLipPath()
        {
            if (Config.AutoLipPath)//自动获取
            {
                //当前目录
                var current = Path.GetFullPath("lip.ex");
                if (File.Exists(current))
                {
                    Lip.WorkingPath = current;
                    return true;
                }
                //全局Path
                var (success, path) = Utils.TryGetLipFromPath();
                if (success)
                {
                    Lip.WorkingPath = path;
                    return true;
                }
            }
            else if (Directory.Exists(Config.WorkingDirectory))//设定了自定义路径
            {
                Lip.WorkingPath = Config.WorkingDirectory;
                return true;
            }
            return false;
        }
        private static readonly string ConfigPath = Path.Combine(".lip", "config", "lipui", "config.json");
        private static Lazy<AppConfig> _config = new(() =>
        {
            var fp = Path.GetFullPath(ConfigPath);
            var result = File.Exists(fp)
                ? AppConfig.FromString(File.ReadAllText(fp))
                : new AppConfig();
            result.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)//修改后应用配置
                {
                    case nameof(result.WorkingDirectory) when result.WorkingDirectory is not null:
                        Lip.WorkingPath = result.WorkingDirectory;
                        break;
                    case nameof(result.LipPath) when result.LipPath is not null:
                    case nameof(result.AutoLipPath):
                        TryRefreshLipPath();
                        break;
                }

                try
                {
                    var dir = Path.GetDirectoryName(fp);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllText(fp, result.ToString());
                }
                catch (Exception ex)
                {
                }
            };
            return result;
        });
        internal static AppConfig Config => _config.Value;
        internal static LipNETWrapper.ILipWrapper Lip = new LipNETWrapper.LipConsoleWrapper(
#if DEBUG
            "A:\\Documents\\GitHub\\BDS\\Latest\\lip.exe"
#endif
            );
        internal static async Task DispatcherInvokeAsync(Action act)
        {
            await Application.Current.Dispatcher.InvokeAsync(act);
        }
        internal static async Task DispatcherInvokeAsync(Func<Task> act)
        {
            await Application.Current.Dispatcher.InvokeAsync(act);
        }
        internal static void DispatcherInvoke(Action act)
        {

            Application.Current.Dispatcher.Invoke(act);
        }
        public static void Navigate<T, TV>()
            where TV : ObservableObject, INavigationAware
            where T : INavigableView<TV>
        {
            DispatcherInvoke(() =>
            {
                ((Views.Windows.MainWindow)Application.Current.MainWindow!).Navigate(typeof(T));
            });
        }

        private static readonly List<object> _queuedItems = new();
        public static void EnqueueItem<T>(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var index = _queuedItems.FindIndex(x => x.GetType() == item.GetType());
            if (index != -1)
            {
                _queuedItems[index] = item;
            }
            else
            {
                _queuedItems.Add(item);
            }
        }
        public static bool TryDequeueItem<T>([NotNullWhen(true)] out T? val)
        {
            val = default;
            var isFound = false;
            foreach (var item in _queuedItems)
            {
                if (item is T v)
                {
                    val = v;
                    isFound = true;
                    break;
                }
            }
            if (isFound)
            {
                _queuedItems.Remove(val!);
            }
            return isFound;
        }
    }
}
