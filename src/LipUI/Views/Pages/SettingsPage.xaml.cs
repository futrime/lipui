using System;
using System.Windows.Input;
using LipUI.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace LipUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private int clickCount = 0;
        private long lastClickTime = 0;
        private void VersionNumber_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (currentTime - lastClickTime < 2)
            {
                clickCount++;
                switch (clickCount)
                {
                    case 2:
                        if (Global.Config.DeveloperMode)
                        {
                            clickCount = 0;
                            Global.PopupSnackbar("开发者模式", "您已经在开发者模式了。");
                        }
                        else
                            Global.PopupSnackbar("开发者模式", "再点 3 次进入开发者模式。");
                        break;
                    case 3:
                        Global.PopupSnackbar("开发者模式", "再点 2 次进入开发者模式。");
                        break;
                    case 4:
                        Global.PopupSnackbar("开发者模式", "再点 1 次进入开发者模式。");
                        break;
                    case >= 5:
                        _ = Global.ShowDialog("开发者模式", "是否进入开发者模式？\n开发者模式下部分页面会有一些额外的选项，请谨慎使用。", ("取消", hide => hide()), ("确认", hide =>
                       {
                           Global.Config.DeveloperMode = true;
                           Global.PopupSnackbar("开发者模式", "已进入开发者模式。");
                           hide();
                       }
                        ));
                        clickCount = 0; break;
                }
            }
            else
            { clickCount = 0; }
            lastClickTime = currentTime;
        }
    }
}