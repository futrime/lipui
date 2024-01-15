using System;
using System.Runtime.InteropServices;
using WinRT;
using static PInvoke.User32;

namespace LipUI;

internal partial class MainWindow
{
    private readonly int MinWidth = 800;
    private readonly int MinHeight = 450;

    private NativeMethods.WinProc? newWndProc = null;
    private nint oldWndProc = nint.Zero;

    private void SubClassing()
    {
        var hwnd = this.As<NativeMethods.IWindowNative>().WindowHandle;
        if (hwnd == nint.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to get window handler: error code {error}");
        }

        newWndProc = new(NewWindowProc);

        // Here we use the NativeMethods class 👇
        oldWndProc = NativeMethods.SetWindowLong(hwnd, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        if (oldWndProc == nint.Zero)
        {
            int error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to set GWL_WNDPROC: error code {error}");
        }
    }

    private nint NewWindowProc(nint hWnd, WindowMessage Msg, nint wParam, nint lParam)
    {
        [DllImport("user32.dll")]
        static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, WindowMessage Msg, nint wParam, nint lParam);

        switch (Msg)
        {
            case WindowMessage.WM_GETMINMAXINFO:
                var dpi = GetDpiForWindow(hWnd);
                float scalingFactor = (float)dpi / 96;

                MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                minMaxInfo.ptMinTrackSize.x = (int)(MinWidth * scalingFactor);
                minMaxInfo.ptMinTrackSize.y = (int)(MinHeight * scalingFactor);
                Marshal.StructureToPtr(minMaxInfo, lParam, true);
                break;

        }
        return CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
    }

    private static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MINMAXINFO
        {
            public PInvoke.POINT ptReserved;
            public PInvoke.POINT ptMaxSize;
            public PInvoke.POINT ptMaxPosition;
            public PInvoke.POINT ptMinTrackSize;
            public PInvoke.POINT ptMaxTrackSize;
        }


        public delegate nint WinProc(nint hWnd, WindowMessage Msg, nint wParam, nint lParam);

        // We have to handle the 32-bit and 64-bit functions separately.
        // 'SetWindowLongPtr' is the 64-bit version of 'SetWindowLong', and isn't available in user32.dll for 32-bit processes.
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern nint SetWindowLong32(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern nint SetWindowLong64(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        public static nint SetWindowLong(nint hWnd, WindowLongIndexFlags nIndex, WinProc newProc)
            => nint.Size is 4 ? SetWindowLong32(hWnd, nIndex, newProc) : SetWindowLong64(hWnd, nIndex, newProc);

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            nint WindowHandle { get; }
        }
    }
}
