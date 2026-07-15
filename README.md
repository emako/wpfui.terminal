# WPF UI Terminal

[![GitHub license](https://img.shields.io/github/license/emako/wpfui.terminal)](https://github.com/emako/wpfui.terminal/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/WPF-UI.Terminal.svg)](https://nuget.org/packages/WPF-UI.Terminal)
[![Actions](https://github.com/emako/wpfui.terminal/actions/workflows/library.nuget.yml/badge.svg)](https://github.com/emako/wpfui.terminal/actions/workflows/library.nuget.yml)
[![Platform](https://img.shields.io/badge/platform-Windows-blue?logo=windowsxp&color=1E9BFA)](https://dotnet.microsoft.com/zh-cn/download/dotnet/latest/runtime)

WPF UI Terminal is a WPF terminal emulator control library that wraps [Microsoft.Terminal.Wpf](https://www.nuget.org/packages/CI.Microsoft.Terminal.Wpf) and Win32 ConPTY APIs to provide a fully-featured embedded terminal control for .NET / WPF applications.

Built on the shoulders of [WPF UI](https://github.com/lepoco/wpfui), it integrates seamlessly with the Fluent design language.

Ported from [EasyWindowsTerminalControl](https://github.com/MitchCapper/EasyWindowsTerminalControl) with added support for .NET Framework (net472, net48, net481).

### 🚀 Getting started

Install the [NuGet package](https://www.nuget.org/packages/WPF-UI.Terminal):

```
dotnet add package WPF-UI.Terminal
```

Add the terminal XML namespace to your XAML:

```xaml
<Window xmlns:t="http://schemas.lepo.co/wpfui/2022/xaml/terminal">
```

Then place a `TerminalControl` anywhere in your UI:

```xaml
<t:TerminalControl
    StartupCommandLine="powershell.exe"
    Theme="Dark"
    FontFamilyWhenSettingTheme="Cascadia Code"
    FontSizeWhenSettingTheme="12" />
```

### 👋 Controls

#### TerminalControl

The main WPF user control that hosts a full terminal emulator. It wraps `Microsoft.Terminal.Wpf.TerminalControl` and a `TermPTY` backend.

**Properties:**

| Property | Type | Description |
|---|---|---|
| `StartupCommandLine` | `string` | Command to run on startup (default: `powershell.exe`) |
| `Theme` (write-only) | `TerminalTheme?` | Sets the terminal color theme |
| `IsReadOnly` (write-only) | `bool?` | When true, user cannot give input through the terminal UI |
| `IsCursorVisible` (write-only) | `bool?` | Shows or hides the terminal cursor |
| `InputCapture` | `INPUT_CAPTURE` | Flags to capture Tab / Direction keys inside the control |
| `Win32InputMode` | `bool` | Enables extended key event handling (default: `true`) |
| `LogConPTYOutput` | `bool` | Logs ConPTY output to a `StringBuilder` |
| `FontFamilyWhenSettingTheme` | `FontFamily` | Font family applied when setting theme (default: Cascadia Code) |
| `FontSizeWhenSettingTheme` | `int` | Font size applied when setting theme (default: 12) |
| `ConPTYTerm` | `TermPTY` | The backend connection object |
| `Terminal` (read-only) | `TerminalControl` | Direct access to the underlying `Microsoft.Terminal.Wpf.TerminalControl` |

**Methods:**

| Method | Description |
|---|---|
| `RestartTerm(TermPTY? useTerm, bool disposeOld)` | Restarts the terminal process |
| `DisconnectConPTYTerm()` | Disconnects and returns the current `TermPTY` |

#### TermPTY

Manages communication with the underlying console via a pseudo-console (ConPTY). Implements `ITerminalConnection` from `Microsoft.Terminal.Wpf`.

```csharp
var term = new TermPTY();
term.Start("cmd.exe", 80, 30);
term.WriteToTerm("dir\r\n");
string output = term.GetConsoleText(stripVTCodes: true);
```

**Members:**

| Member | Description |
|---|---|
| `Start(command, width, height, logOutput, factory)` | Starts the pseudoconsole and child process |
| `WriteToTerm(input)` | Writes input (with VT-100 support) to the console |
| `WriteToTermBinary(input)` | Writes raw bytes to the console |
| `WriteToUITerminal(str)` | Simulates output from the program to the terminal window |
| `GetConsoleText(stripVTCodes)` | Returns the full console output log |
| `StripColors(str)` | Strips VT-100 escape codes from a string |
| `SetReadOnly(readOnly, updateCursor)` | Enables / disables read-only mode |
| `SetCursorVisibility(visible)` | Shows or hides the cursor |
| `ClearUITerminal(fullReset)` | Clears the terminal screen |
| `Resize(columns, rows)` | Resizes the pseudoconsole |
| `Win32DirectInputMode(enable)` | Toggles Win32 extended input mode |
| `CloseStdinToApp()` | Closes the input stream |
| `StopExternalTermOnly()` | Kills the child process |
| `InterceptOutputToUITerminal` | Delegate to intercept/modify output before it reaches the UI |
| `InterceptInputToTermApp` | Delegate to intercept/modify user input before it reaches the app |
| `TermReady` | Event fired when the console is ready |
| `TerminalOutput` | Event fired with terminal output data |

#### ReadDelimitedTermPTY

Extends `TermPTY` to buffer output and only send it to the UI after a specific delimiter string is seen. Useful for structured output (JSON, XML, custom terminators).

```csharp
var term = new ReadDelimitedTermPTY(
    delimiter: "\r\n",
    MaxWaitTimeoutForDelimiter: TimeSpan.FromSeconds(5)
);
term.Start("my-cli-tool.exe");

term.SetReadOutputDelimiter("</output>",
    TimeSpan.FromMilliseconds(500));
```

### ⚙️ XAML Namespace

```xaml
xmlns:t="http://schemas.lepo.co/wpfui/2022/xaml/terminal"
```

Maps to `Wpf.Ui.Terminal` and `Wpf.Ui.Terminal.Controls`.

### 🏗️ Internals

The library provides extension points for advanced scenarios:

- **`PseudoConsole`** - Wraps the Win32 Pseudo Console handle (`Create`, `Resize`)
- **`PseudoConsolePipe`** - Creates anonymous Win32 pipes for I/O
- **`PseudoConsoleApi`** - P/Invoke wrappers for `conpty.dll`
- **`ProcessFactory`** - Static utility for starting child processes with pseudoconsole attributes
- **`IProcessFactory`** / **`IProcess`** - Interfaces for custom process creation (DI support)

### 🎯 Requirements

- Windows (terminal requires Win32 ConPTY support, available since Windows 10 v1809)
- .NET Framework 4.7.2+ / .NET 8.0+ / .NET 9.0+ / .NET 10.0+

### 📦 NuGet

```
WPF-UI.Terminal
https://www.nuget.org/packages/WPF-UI.Terminal
```

### 📄 License

MIT
