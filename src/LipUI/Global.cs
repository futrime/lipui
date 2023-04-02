using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LipNETWrapper;
using LipUI.Models;
using LipUI.ViewModels;
using LipUI.Views.Controls;
using LipUI.Views.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;

namespace LipUI
{
    internal static class Global
    {

        /// <summary>
        /// Disclaimer
        /// </summary>
        private const string eulaText = """
                本软件包管理器（以下简称“本软件”）是由LiteLDev（以下简称“开发者”）开发和提供的。本软件旨在帮助用户管理和安装各种软件包，但不对任何软件包的内容、质量、功能、安全性或合法性负责。用户在使用本软件时，应自行判断和承担相关风险。
                开发者不保证本软件的稳定性、可靠性、准确性或完整性。开发者不对本软件可能存在的任何缺陥、错误、病毒或其他有害成分负责。开发者不对用户使用本软件造成的任何直接或间接损失（包括但不限于数据丢失、设备损坏、利润损失等）负责。
                开发者保留随时修改、更新或终止本软件及其相关服务的权利，无需事先通知用户。用户应自行备份重要数据，并定期检查本软件是否有更新版本。
                用户在使用本软件时，应遵守相关法律法规，尊重他人的知识产权和隐私权，不得利用本软件进行任何违法或侵权行为。如用户违反上述规定，造成任何第三方损害或被第三方索赔，开发者不承担任何责任。
                如果用户对本免责条款有任何疑问或意见，请联系开发者：LiteLDev
                
                This software package manager (hereinafter referred to as "this software") is developed and provided by LiteLDev (hereinafter referred to as "the developer"). This software is designed to help users manage and install various software packages, but is not responsible for any content, quality, functionality, security or legality of any software package. Users should use this software at their own discretion and assume all related risks.
                The developer does not guarantee the stability, reliability, accuracy or completeness of this software. The developer is not liable for any defects, errors, viruses or other harmful components that may exist in this software. The developer is not liable for any direct or indirect damages (including but not limited to data loss, device damage, profit loss etc.) caused by the use of this software.
                The developer reserves the right to modify, update or terminate this software and its related services at any time without prior notice to users. Users should back up important data and check regularly for updates of this software.
                Users should comply with relevant laws and regulations when using this software, respect the intellectual property rights and privacy rights of others, and not use this software for any illegal or infringing activities. If users violate the above provisions and cause any damage to any third party or are claimed by any third party, the developer does not bear any responsibility.
                If you have any questions or comments about this disclaimer, please contact the developer: LiteLDev
                """;
        /// <summary>
        /// i18n
        /// </summary>
        internal static Language.Model I18N =>
           Application.Current.FindResource("I18N") as Language.Model
#if DEBUG
           ?? throw new NullReferenceException("undefined I18N");
#else
           ?? i18nFallback.Value;
        private static Lazy<Language.Model> i18nFallback = new(() => new());//资源获取失败时返回
#endif
        /// <summary>初始化语言</summary>
        private static void InitLanguage()
        {

        }
        /// <summary>初始化全局变量(初始化程序)</summary>
        internal static async void Init()
        {
            InitLanguage();
            var eulaPath = Path.Combine(ConfigFolder, "EULA.txt");
            if (File.Exists(eulaPath))
            {
                if (File.ReadAllText(eulaPath, Encoding.UTF8) == eulaText)//条款已读
                {
                    InitNext();
                    return;
                }
            }
            //显示条款
            {
                _ = ShowDialog("免责条款", await DispatcherInvokeAsync(() => new TextBlock
                {
                    Text = eulaText,
                    TextWrapping = TextWrapping.WrapWithOverflow
                }), ("不同意", _ =>
                        {
                            PopupSnackbarWarn("您拒绝了条款", "程序即将退出");
                            Task.Delay(2500).ContinueWith(_ =>
                            {
                                Environment.Exit(0);
                            });
                        }
                ), ("同意", hide =>
                        {
                            hide();
                            var directory = Path.GetDirectoryName(eulaPath);
                            if (directory is not null && !Directory.Exists(directory))
                                Directory.CreateDirectory(directory);
                            File.WriteAllText(eulaPath, eulaText, Encoding.UTF8);
                            InitNext();
                        }
                ), dialog =>
                {
                    dialog.DialogHeight = 400;
                    dialog.DialogWidth = 400;
                });
            }
        }

        internal static void InitNext()
        {
            // 从配置获取Lip路径
            if (!TryRefreshLipPath())
            {
                var vm = new LipInstallerViewModel();
                _ = ShowDialog("需要配置 lip.exe", new LipInstaller(vm), ("完成", hide =>
                {
                    Config.AutoLipPath = !vm.ManualExe;
                    Config.LipPath = vm.LipPath;
                    if (TryRefreshLipPath())
                    {
                        hide();
                        CheckWorkDir();
                    }
                    else
                        PopupSnackbar("未找到lip.exe", "如已安装请重新启动LipUI");
                }
                ), ("下载", hide =>
                {
                    Config.AutoLipPath = !vm.ManualExe;
                    if (vm.GlobalExe)
                    {
                        if (vm.ShowProgressBar)
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
                                    await DispatcherInvokeAsync(() => vm.Tip = "下载完成.");
                                    var tmpDir = Path.Combine(ConfigFolder, "temp");
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
                    else if (vm.PortableExe)
                    {
                        if (vm.ShowProgressBar)
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
                                    var bytes = await Utils.DownloadLipPortable(e =>
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
                                    await DispatcherInvokeAsync(() => vm.Tip = "下载完成.");
                                    //解压缩 
                                    // Create a memory stream from the byte array
                                    using MemoryStream ms = new MemoryStream(bytes);
                                    // Create a zip archive from the memory stream
                                    using ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read);
                                    ZipArchiveEntry entry = zip.Entries.First(x => Path.GetFileName(x.Name) == "lip.exe")
                                                            ?? throw new Exception("压缩包中未找到 lip.exe");
                                    entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), entry.Name));
                                    //开启新的LipUI
                                    Process.Start(Environment.GetCommandLineArgs()[0],
                                        string.Join(" ",
                                            from arg in Environment.GetCommandLineArgs().Skip(1)
                                            select $"\"{arg}\""));
                                    //hide();
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
                        PopupSnackbar("无效操作", "请选中安装到'全局'或者'当前目录'以下载 lip.exe", SymbolRegular.Warning16, ControlAppearance.Danger);
                }
                ), modify: dialog =>
                {
                    dialog.DialogHeight = 300;
                });
            }
            else
            {
                CheckWorkDir();
            }
        }
        internal static void CheckWorkDir()
        {
            if (!Directory.Exists(Config.WorkingDirectory?.Directory??""))
            {//保存的WorkingDirectory不合法，需要手动选择 
                _ = ShowDialog("需要指定有效的工作路径", new WorkingPathSelector(), ("完成", hide =>
                        {
                            if (Directory.Exists(Config.WorkingDirectory?.Directory ?? ""))
                            {
                                hide();
                            }
                            else
                            {
                                PopupSnackbar("请选择有效的工作路径", Config.WorkingDirectory?.Directory ?? "", SymbolRegular.Warning16, ControlAppearance.Caution);
                            }
                        }
                ), modify: dialog =>
                {
                    dialog.DialogWidth = 750;
                    dialog.DialogHeight = 370;
                });
            }
            else
            {
                Lip.WorkingPath = Config.WorkingDirectory?.Directory ?? "";
            }
        }
        static bool TryRefreshLipPath()
        {
            if (Config.AutoLipPath)//是否设置了自动获取
            {
                //当前目录优先
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
                    Lip.ExecutablePath = path!;
                    return true;
                }
            }
            else if (File.Exists(Config.LipPath))//使用自定义路径
            {
                Lip.ExecutablePath = Config.LipPath;
                return true;
            }
            else
            {
                var file = Path.Combine(Config.LipPath, "lip.exe");
                if (File.Exists(file))//使用自定义路径文件夹下的lip.exe
                {
                    Lip.ExecutablePath = file;
                    return true;
                }
            }
            return false;//最终没能找到
        }
        /// <summary>配置文件路径</summary>
        private static readonly string ConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".lip", "config", "lipui");
        internal static readonly string ConfigPath = Path.Combine(ConfigFolder, "config.json");
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
                    case nameof(result.WorkingDirectory) when result!.WorkingDirectory is not null:
                        Lip!.WorkingPath = result!.WorkingDirectory.Directory;
                        break;
                    case nameof(result.LipPath) when result!.LipPath is not null:
                        Lip!.ExecutablePath = result!.LipPath;
                        TryRefreshLipPath();
                        break;
                    case nameof(result.AutoLipPath):
                        TryRefreshLipPath();
                        break;
                }
                //保存
                try
                {
                    var dir = Path.GetDirectoryName(fp)!;
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
                ((MainWindow)Application.Current.MainWindow!).Navigate(typeof(T));
            });
        }
        /// <summary>
        /// 弹出底部提示
        /// </summary>
        public static void PopupSnackbar(string title, string content, SymbolRegular icon = SymbolRegular.Info16, ControlAppearance appearance = ControlAppearance.Secondary, int timeout = 2000)
        {
            DispatcherInvoke(() =>
            {
                var snackbar = ((MainWindow)Application.Current.MainWindow!).Snackbar;
                snackbar.Timeout = timeout;
                snackbar.Show(title, content, icon, appearance);
            });
        }
        public static void PopupSnackbarWarn(string title, string content)
            => PopupSnackbar(title, content, SymbolRegular.Warning16, ControlAppearance.Caution);
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
            Action<Dialog>? modify = null)
        {
            await DispatcherInvokeAsync(async () =>
            {
                var dialog = ((MainWindow)Application.Current.MainWindow!).Dialog;
                bool successAndHide = false;
                void Hide()
                {
                    successAndHide = true;
                    dialog.Hide();
                }
                RoutedEventHandler onDialogOnButtonLeftClick = (_, e) => Hide();
                RoutedEventHandler onDialogOnButtonRightClick = (_, e) => Hide();
                {
                    dialog.ButtonRightVisibility = Visibility.Hidden;
                    if (right is var (k, v))
                    {
                        dialog.ButtonRightName = k;
                        dialog.ButtonRightVisibility = Visibility.Visible;
                        onDialogOnButtonRightClick = (_, e) => v(Hide);
                    }
                }
                {
                    dialog.ButtonLeftVisibility = Visibility.Hidden;
                    if (left is var (k, v))
                    {
                        dialog.ButtonLeftName = k;
                        dialog.ButtonLeftVisibility = Visibility.Visible;
                        onDialogOnButtonLeftClick = (_, e) => v(() => Hide());
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
                //dialog.Title = title;
                if (content is not UIElement)
                {
                    content = new TextBlock
                    {
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        Text = content.ToString()
                    };
                }

                var textBlock = new TextBlock { FontSize = 20, Text = title, Margin = new Thickness(-2, -5, -2, 5) };
                textBlock.SetValue(DockPanel.DockProperty, Dock.Top);
                dialog.Content = new DockPanel()
                {
                    Children =
                    {
                        textBlock,
                       (UIElement)content
                    }
                };
                while (!successAndHide)
                {
                    await dialog.ShowAndWaitAsync();
                }
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
