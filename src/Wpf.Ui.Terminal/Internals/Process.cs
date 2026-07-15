using System;
using Windows.Win32;
using Windows.Win32.System.Threading;

namespace Wpf.Ui.Terminal.Internals;

/// <summary>
/// Represents an instance of a process.
/// </summary>
internal sealed class Process(STARTUPINFOEXW startupInfo, PROCESS_INFORMATION processInfo) : IDisposable
{
    public STARTUPINFOEXW StartupInfo { get; } = startupInfo;
    public PROCESS_INFORMATION ProcessInfo { get; } = processInfo;

    private bool disposedValue = false; // To detect redundant calls

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects).
            }

            // dispose unmanaged state

            // Free the attribute list
            if (StartupInfo.lpAttributeList != default)
            {
                PInvoke.DeleteProcThreadAttributeList(StartupInfo.lpAttributeList);
            }
            // Close process and thread handles
            if (ProcessInfo.hProcess != IntPtr.Zero)
            {
                PInvoke.CloseHandle(ProcessInfo.hProcess);
            }
            if (ProcessInfo.hThread != IntPtr.Zero)
            {
                PInvoke.CloseHandle(ProcessInfo.hThread);
            }

            disposedValue = true;
        }
    }

    ~Process()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
