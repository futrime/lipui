using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
