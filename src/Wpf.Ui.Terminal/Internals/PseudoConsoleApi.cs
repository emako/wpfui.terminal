using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Win32.System.Console;

namespace Wpf.Ui.Terminal.Internals;

/// <summary>
/// PInvoke signatures for Win32's PseudoConsole API.
/// </summary>
public static class PseudoConsoleApi
{
    static PseudoConsoleApi()
    {
        // Ensure conpty.dll can be loaded from runtimes/{rid}/native/ directories
        LoadConPtyLibrary();
    }

    private static void LoadConPtyLibrary()
    {
        try
        {
            var assemblyLocation = string.Empty;
            if (string.IsNullOrEmpty(assemblyLocation))
                assemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
            else
                assemblyLocation = Path.GetDirectoryName(assemblyLocation) ?? AppDomain.CurrentDomain.BaseDirectory;

            // Determine the runtime identifier
            string rid = GetRuntimeIdentifier();
            string conptyPath = Path.Combine(assemblyLocation, "runtimes", rid, "native", "conpty.dll");

            // Fallback: try to find conpty.dll in the base directory
            if (!File.Exists(conptyPath))
            {
                conptyPath = Path.Combine(assemblyLocation, "conpty.dll");
            }

            // Pre-load the library if found in runtimes directory
            if (File.Exists(conptyPath))
            {
#if NETFRAMEWORK
                // For .NET Framework, use LoadLibrary
                LoadLibrary(conptyPath);
#else
                // For .NET Core/.NET 5+, use NativeLibrary
                NativeLibrary.Load(conptyPath);
#endif
            }
        }
        catch
        {
            // If pre-loading fails, let the normal DllImport mechanism handle it
        }
    }

    private static string GetRuntimeIdentifier()
    {
        // Determine architecture
        string arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            _ => "x64" // Default to x64
        };

        // For Windows, use win10-{arch} as the RID
        return $"win10-{arch}";
    }

#if NETFRAMEWORK

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint LoadLibrary(string lpFileName);

#endif

    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out nint phPC);

    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int ClosePseudoConsole(nint hPC);

    [DllImport("conpty.dll", SetLastError = true)]
    internal static extern int ResizePseudoConsole(nint hPC, COORD size);
}
