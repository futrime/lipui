using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace LipUI
{
    internal static class Global
    {
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
    }
}
