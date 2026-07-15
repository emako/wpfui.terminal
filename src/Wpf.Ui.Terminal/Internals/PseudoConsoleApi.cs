using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Windows.Win32.System.Console;

namespace Wpf.Ui.Terminal.Internals;

/// <summary>
/// PInvoke signatures for Win32's PseudoConsole API.
/// </summary>
public static class PseudoConsoleApi
{
    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out nint phPC);

    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int ClosePseudoConsole(nint hPC);

    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int ResizePseudoConsole(nint hPC, COORD size);
}
