using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;
using Hyperlink = Wpf.Ui.Controls.Hyperlink;

namespace LipUI.ViewModels
{
    public record InstallInfo(string Tooth, ToothInfoPanelViewModel data, string? Version);
    public partial class InstallPageViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        ObservableCollection<string> _outPut = new();
        [ObservableProperty]
        string _toothName = string.Empty;
        partial void OnToothNameChanged(string value)
        {
            ToothInfoPanel = null;
            OutPut.Clear();
        }
        [NotifyCanExecuteChangedFor(nameof(InstallCommand))]
        [NotifyPropertyChangedFor(nameof(InfoLoaded))]
        [ObservableProperty]
        ToothInfoPanelViewModel? _toothInfoPanel;
        public bool InfoLoaded => _toothInfoPanel is not null;
        [ObservableProperty] private bool _installing = false;
        /// <summary>
        /// 执行安装
        /// </summary>
        [RelayCommand(CanExecute = nameof(InfoLoaded))]
        public async Task Install()
        {
            OutPut.Clear();
            Ctk = new CancellationTokenSource();
            Installing = true;
            try
            {
                var fullname = ToothName;
                if (!string.IsNullOrWhiteSpace(SelectedVersion))
                    fullname += "@" + SelectedVersion; 
                var exitCode = await Global.Lip.InstallPackageAsync(fullname,! string.IsNullOrWhiteSpace(ToothInfoPanel?.Version), Ctk.Token, (x, input) =>
                {
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        if (x.EndsWith("(y/n)", true, CultureInfo.InvariantCulture))//条款
                        {
                            Task.Delay(1000).ContinueWith(async _ =>
                            {
                                var fullEula =
                                    //string.Join(Environment.NewLine, OutPut)
                                    OutPut.Last()
                                        .Replace("(http", Environment.NewLine + "http");
                                //.Replace("http", Environment.NewLine + "http");
                                if (fullEula.EndsWith("(y/n)", true, CultureInfo.InvariantCulture))
                                {
                                    //remove 
                                    fullEula = fullEula[..^5].Trim();
                                }
                                if (fullEula.EndsWith(")", true, CultureInfo.InvariantCulture))
                                {
                                    //remove 
                                    fullEula = fullEula[..^1];
                                }
                                ////remove str before === 
                                //{
                                //    var index = fullEula.IndexOf("\r\n", StringComparison.Ordinal);
                                //    if (index != -1) { fullEula = fullEula[(index + 8)..]; }
                                //}
                                _ = Global.ShowDialog("提示", await Global.DispatcherInvokeAsync(() =>
                                    {
                                        try
                                        {
                                            StackPanel content = new() { Margin = new(0, 20, 0, 0) };
                                            //foreach (var data in fullEula.Split(new[] { "https" }, StringSplitOptions.None))
                                            //{
                                            //    content.Children.Add(new TextBlock() { Text = data });
                                            //}
                                            //highlight all http url in eula
                                            while (Regex.Match(fullEula, @"https?://[^\s]+") is { Success: true } match)
                                            {
                                                var text = match.Value;
                                                var index = match.Index;
                                                var length = match.Length;
                                                var before = fullEula[..index];
                                                var after = fullEula[(index + length)..];
                                                var link = new Hyperlink { NavigateUri = text, Content = text };
                                                link.Click += (sender, e) => { System.Diagnostics.Process.Start(text); };
                                                content.Children.Add(new TextBlock() { Text = before });
                                                content.Children.Add(link);
                                                fullEula = after;
                                            }
                                            content.Children.Add(new TextBlock() { Text = fullEula });
                                            return content;
                                        }
                                        catch
                                        {
                                            return new StackPanel
                                            {
                                                Children =
                                                {
                                                    new TextBlock
                                                    {
                                                        Text = fullEula,
                                                        TextWrapping = TextWrapping.WrapWithOverflow
                                                    }
                                                }
                                            };
                                        }
                                    }), ("取消", hide =>
                                    {
                                        hide();
                                        Global.PopupSnackbarWarn("取消", "安装已取消");
                                        Task.Delay(1000).ContinueWith(_ =>
                                        {
                                            input("n");
                                        });
                                    }
                                ), ("好的", hide =>
                                    {
                                        hide();
                                        input("y");
                                    }
                                ), modify: dialog =>
                                {
                                    Global.DispatcherInvoke(() =>
                                    {
                                        dialog.DialogHeight = 300;
                                        dialog.DialogWidth = 500;
                                    });
                                });
                            });
                            Global.DispatcherInvoke(() => OutPut.Add(x));
                        }
                        else if (Regex.Match(x, @"(?:Downloading|Unziping).+?(\d+%.*)") is { Success: true } match)
                        {
                            var str = match.Groups[1].Value;
                            //去除匹配并去除"[==>   ] "
                            Percentage = Regex.Replace(str, @"\[(=|>|\s)+?\]", "");
                        }
                        else if (x.Trim().EndsWith("|"))
                            Percentage = x.Replace("|", "").Trim();
                        else
                        {
                            Global.DispatcherInvoke(() => OutPut.Add(x));
                            if (x.StartsWith("Successfully installed all tooth files"))
                            {
                                Global.PopupSnackbar("安装完成", "Successfully installed all tooth files.");
                            }
                            else if(x.StartsWith("[Info] Generating BDS Please wait for few minutes"))
                            {
                                Percentage = "生成BDS...";
                            }
                            else if (x.StartsWith("Error", true, CultureInfo.InvariantCulture))
                            {
                                Global.PopupSnackbarWarn("小错误", x[6..]);
                            }
                        }
                    }
                });
                OutPut.Add($"ExitCode：{exitCode}");
            }
            catch (Exception ex)
            {
                OutPut.Add(ex.ToString());
            }
            Ctk = null;
            Installing = false;
            Percentage = "";
        }
        [RelayCommand]
        public async Task FetchInfo()
        {
            OutPut.Clear();
            Ctk = new CancellationTokenSource();
            try
            {
                var (success, package, message) = await Global.Lip.GetPackageInfoAsync(ToothName, _ctk?.Token ?? default, x =>
                {
                    if (!x.StartsWith("{"))
                    {
                        Global.DispatcherInvoke(() => OutPut.Add(x));
                    }
                });
                if (success)
                {
                    ToothInfoPanel = new ToothInfoPanelViewModel(package!)
                    {
                        Tooth = ToothName
                    };
                }
                else
                {
                    Global.PopupSnackbarWarn("获取失败", message);
                }
            }
            catch (Exception ex)
            {
                OutPut.Add(ex.ToString());
            }
            Ctk = null;
        }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanCancel))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        CancellationTokenSource? _ctk = null;
        [ObservableProperty]
        string? _selectedVersion;
        [ObservableProperty]
        string _percentage = string.Empty;
        partial void OnPercentageChanged(string value)
        {
            try
            {
                if (Regex.Match(value, @"(\d+?(?:\.\d+?)?)%") is { Success: true } match
                    && double.TryParse(match.Groups[1].Value.Trim(), out var pn))
                {
                    _tmpPercentageNumber = pn;
                }
                else
                {
                    _tmpPercentageNumber = 0;
                }
            }
            catch
            {
                _tmpPercentageNumber = 0;
            }
            OnPropertyChanged(nameof(PercentageIsIndeterminate));
            OnPropertyChanged(nameof(PercentageNumber));
        }
        private double _tmpPercentageNumber = 0;
        public double PercentageNumber => PercentageIsIndeterminate ? 80 : _tmpPercentageNumber;
        public bool PercentageIsIndeterminate => _tmpPercentageNumber is <= 0 or >= 100;
        public bool CanCancel => Ctk is not null;
        [RelayCommand(CanExecute = nameof(CanCancel))]
        public void Cancel()
        {
            Ctk?.Cancel();
            Ctk = null;
        }
        public void OnNavigatedTo()
        {
            if (Global.TryDequeueItem<InstallInfo>(out var item))
            {
                ToothName = item.Tooth;
                ToothInfoPanel = item.data;
                SelectedVersion = item.Version ?? item.data.Versions?.FirstOrDefault();
            }
        }
        public void OnNavigatedFrom()
        {
        }
    }
}
