using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
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
            await (Application.Current.Windows.OfType<Views.Windows.MainWindow>()
                    .FirstOrDefault()?
                    .Dispatcher ??
                   throw new Exception("MainWindow not found!")
                    )
                .InvokeAsync(act);
        }
        internal static void DispatcherInvoke(Action act)
        {
            (Application.Current.Windows.OfType<Views.Windows.MainWindow>()
                  .FirstOrDefault()?
                  .Dispatcher ??
                 throw new Exception("MainWindow not found!")
                  )
              .Invoke(act);
        }
    }
}
