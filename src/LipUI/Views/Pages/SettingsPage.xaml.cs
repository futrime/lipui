using System;
using System.Linq;
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
                            Global.PopupSnackbar(Global.I18N.DeveloperSnackbarTitle, Global.I18N.DeveloperAlreadyIn);
                        }
                        else
                            Global.PopupSnackbar(Global.I18N.DeveloperSnackbarTitle, string.Format(Global.I18N.DeveloperSnackbarSubtitle, 3));
                        break;
                    case 3:
                        Global.PopupSnackbar(Global.I18N.DeveloperSnackbarTitle, string.Format(Global.I18N.DeveloperSnackbarSubtitle, 2));
                        break;
                    case 4:
                        Global.PopupSnackbar(Global.I18N.DeveloperSnackbarTitle, string.Format(Global.I18N.DeveloperSnackbarSubtitle, 1));
                        break;
                    case >= 5:
                        _ = Global.ShowDialog(Global.I18N.DeveloperTitle, Global.I18N.DeveloperDialog
                        , (Global.I18N.DeveloperDialogCancel, hide => hide()), (Global.I18N.DeveloperDialogConfirm, hide =>
                       {
                           Global.Config.DeveloperMode = true;
                           Global.PopupSnackbar(Global.I18N.DeveloperSnackbarTitle, Global.I18N.DeveloperEnterSuccess);
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