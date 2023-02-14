using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LipNETWrapper;
using LipUI.Models;
using LipUI.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace LipUI
{
    internal static class Global
    {
        /// <summary>初始化全局变量(初始化程序)</summary>
        internal static void Init()
        {
            // 从配置获取Lip路径
            if (!TryRefreshLipPath())
            {
                var vm = new LipInstallerViewModel();
                _ = ShowDialog("未找到 lip.exe", new Views.Controls.LipInstaller(vm), ("完成", hide =>
                {
                    Config.AutoLipPath = !vm.ManualConfig;
                    Config.LipPath = vm.LipPath;
                    if (TryRefreshLipPath())
                        hide();
                    else
                        vm.Tip = "未找到lip.exe，如已安装请重新启动LipUI";
                }
                ), ("下载安装程序", hide =>
                {
                    Config.AutoLipPath = !vm.ManualConfig;
                    if (Config.AutoLipPath)
                    {
                        if (vm.ShowProgressBar == true)
                        {
                            vm.Tip = "正在下载，请等待...";
                        }
                        else
                        {
                            vm.ShowProgressBar = true;
                            Task.Run(async () =>
                            {
                                try
                                {
                                    var bytes = await Utils.DownloadLipInstaller(e =>
                                    {
                                        DispatcherInvoke(() =>
                                        {
                                            vm.Progress = e.ProgressPercentage;
                                            string BytesToStr(long val) => val switch
                                            {
                                                < 1024 => $"{val}B",
                                                < 1024 * 1024 => $"{val / 1024}KB",
                                                < 1024 * 1024 * 1024 => $"{val / 1024 / 1024}MB",
                                                _ => $"{val / 1024 / 1024 / 1024}GB"
                                            };
                                            vm.Tip = $"下载中...{BytesToStr(e.BytesReceived)}/{BytesToStr(e.TotalBytesToReceive)}";
                                        });
                                    });
                                    await DispatcherInvokeAsync(() => vm.Tip = $"下载完成.");
                                    var tmpDir = Path.Combine(Path.GetDirectoryName(ConfigPath)!, "temp");
                                    if (!Directory.Exists(tmpDir)) Directory.CreateDirectory(tmpDir);
                                    var installer = Path.Combine(tmpDir, "lip_installer.exe");
                                    File.WriteAllBytes(installer, bytes);
                                    //等待安装程序退出
                                    Process.Start(installer)
                                        ?.WaitForExit();
                                    //开启新的LipUI 
                                    Process.Start(Environment.GetCommandLineArgs()[0],
                                        string.Join(" ",
                                            from arg in Environment.GetCommandLineArgs().Skip(1)
                                            select $"\"{arg}\""));
                                    //删除安装包
                                    try
                                    {
                                        File.Delete(installer);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                    //退出当前实例
                                    Environment.Exit(0);
                                }
                                catch (Exception e)
                                {
                                    await DispatcherInvokeAsync(() => vm.Tip = "失败：" + e);
#if DEBUG
                                    throw;
#endif
                                }
                            });
                            vm.Progress = 0;
                        }
                    }
                    else
                        vm.Tip = "请开启'自动获取Lip路径'";
                }
                ));
            }
        }
        static bool TryRefreshLipPath()
        {
            if (Config.AutoLipPath)//自动获取
            {
                //当前目录
                var current = Path.GetFullPath("lip.exe");
                if (File.Exists(current))
                {
                    Lip.ExecutablePath = current;
                    return true;
                }
                //全局Path
                var (success, path) = Utils.TryGetLipFromPath();
                if (success)
                {
                    Lip.ExecutablePath = path;
                    return true;
                }
            }
            else if (File.Exists(Config.LipPath))//设定了自定义路径
            {
                Lip.ExecutablePath = Config.LipPath;
                return true;
            }
            else
            {
                var file = Path.Combine(Config.LipPath, "lip.exe");
                if (File.Exists(file))//设定了自定义路径
                {
                    Lip.ExecutablePath = file;
                    return true;
                }
            }
            return false;
        }
        /// <summary>配置文件路径</summary>
        private static readonly string ConfigPath = Path.Combine(".lip", "config", "lipui", "config.json");
        private static Lazy<AppConfig> _config = //延迟加载的配置文件对象
            new(() =>
        {
            var fp = Path.GetFullPath(ConfigPath);
            var result = File.Exists(fp)
                ? AppConfig.FromString(File.ReadAllText(fp))
                : new AppConfig();
            //订阅属性更改，并应用属性
            result.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)//修改后应用配置
                {
                    case nameof(result.WorkingDirectory) when result.WorkingDirectory is not null:
                        Lip.WorkingPath = result.WorkingDirectory;
                        break;
                    case nameof(result.LipPath) when result.LipPath is not null:
                        Lip.ExecutablePath = result.LipPath;
                        TryRefreshLipPath();
                        break;
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
                catch
                {
                    // ignored
                }
            };
            return result;
        });
        /// <summary>
        /// 程序配置文件实例
        /// </summary>
        internal static AppConfig Config => _config.Value;
        /// <summary>
        /// 实现Lip接口的实例
        /// </summary>
        internal static ILipWrapper Lip = new LipConsoleWrapper();
        /// <summary>
        /// UI线程封送消息，异步可等待
        /// </summary>
        /// <param name="act">方法对象</param>
        /// <returns>异步任务</returns>
        internal static async Task DispatcherInvokeAsync(Action act)
        {
            await Application.Current.Dispatcher.InvokeAsync(act);
        }
        /// <summary>
        /// UI线程封送消息，异步可等待
        /// </summary>
        /// <param name="act">异步方法对象</param>
        /// <returns>异步任务</returns>
        internal static async Task DispatcherInvokeAsync(Func<Task> act)
        {
            await Application.Current.Dispatcher.InvokeAsync(act);
        }
        /// <summary>
        /// UI线程封送消息
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="act">方法对象</param>
        /// <returns>返回对象</returns>
        internal static async Task<T> DispatcherInvokeAsync<T>(Func<T> act)
        {
            return await Application.Current.Dispatcher.InvokeAsync(act);
        }
        /// <summary>
        /// UI线程封送消息
        /// </summary>
        /// <param name="act">方法委托</param>
        internal static void DispatcherInvoke(Action act)
        {
            Application.Current.Dispatcher.Invoke(act);
        }
        /// <summary>
        /// 导航到页面
        /// </summary>
        /// <typeparam name="T">页面的类，要求继承ui:Page</typeparam>
        /// <typeparam name="TV">页面的ViewModel，要求继承ObservableObject且实现INavigationAware</typeparam>
        public static void Navigate<T, TV>()
            where T : INavigableView<TV>
            where TV : ObservableObject, INavigationAware
        {
            DispatcherInvoke(() =>
            {
                ((Views.Windows.MainWindow)Application.Current.MainWindow!).Navigate(typeof(T));
            });
        }
        /// <summary>
        /// 弹出底部提示
        /// </summary>
        public static void PopupSnackbar(string title, object content, SymbolRegular icon = SymbolRegular.Info16, ControlAppearance appearance = ControlAppearance.Secondary)
        {
            DispatcherInvoke(() =>
            {
                var snackbar = ((Views.Windows.MainWindow)Application.Current.MainWindow!).Snackbar;
                snackbar.Show(title, "", icon, ControlAppearance.Primary);
                snackbar.Content = content;
            });
        }
        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容（如果是WPF控件需要DispatcherInvoke在UI线程创建）</param>
        /// <param name="right">左按钮内容和回调</param>
        /// <param name="left">右按钮内容和回调</param>
        /// <param name="modify">对dialog控件的额外修改（通常无需操作）</param>
        /// <returns></returns>
        public static async Task ShowDialog(
            string title,
            object content,
            (string, Action<Action>)? right = null,
            (string, Action<Action>)? left = null,
            Action<Wpf.Ui.Controls.Dialog>? modify = null)
        {
            await DispatcherInvokeAsync(async () =>
            {
                var dialog = ((Views.Windows.MainWindow)Application.Current.MainWindow!).Dialog;
                RoutedEventHandler onDialogOnButtonLeftClick = (_, e) => dialog.Hide();
                RoutedEventHandler onDialogOnButtonRightClick = (_, e) => dialog.Hide();
                {
                    dialog.ButtonRightVisibility = Visibility.Hidden;
                    if (right is var (k, v))
                    {
                        dialog.ButtonRightName = k;
                        dialog.ButtonRightVisibility = Visibility.Visible;
                        onDialogOnButtonRightClick = (_, e) => v(() => dialog.Hide());
                    }
                }
                {
                    dialog.ButtonLeftVisibility = Visibility.Hidden;
                    if (left is var (k, v))
                    {
                        dialog.ButtonLeftName = k;
                        dialog.ButtonLeftVisibility = Visibility.Visible;
                        onDialogOnButtonLeftClick = (_, e) => v(() => dialog.Hide());
                    }
                }
                dialog.DialogHeight = 250;
                dialog.DialogWidth = 400;
                modify?.Invoke(dialog);
                //if (content is Control ct)
                //{
                //    //if (ct.MinHeight > 100)
                //    //{ 
                //    //dialog.DialogHeight = double.NaN-400;
                //    //dialog.DialogWidth = double.NaN;
                //    //} 
                //}
                dialog.ButtonLeftClick += onDialogOnButtonLeftClick;
                dialog.ButtonRightClick += onDialogOnButtonRightClick;
                dialog.Title = title;
                dialog.Content = content;
                await dialog.ShowAndWaitAsync();
                dialog.ButtonLeftClick -= onDialogOnButtonLeftClick;
                dialog.ButtonRightClick -= onDialogOnButtonRightClick;
            });
        }
        #region 共享队列（页面之间传递信息）
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
        #endregion
    }
}
