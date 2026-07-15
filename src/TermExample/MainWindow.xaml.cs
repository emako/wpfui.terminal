using Microsoft.Terminal.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Terminal;
using TerminalControl = Wpf.Ui.Terminal.TerminalControl;

namespace TermExample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// when mirror mode is on the duplicate new button still shows the terminal attached to the old window too
    /// </summary>
    private const bool MirrorMode = false;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = Binds;
    }

    private DataBinds Binds { get; set; } = new();

    public MainWindow(TermPTY existingTerm)
    {
        InitializeComponent();
        DataContext = Binds;
        basicTermControl.DisconnectConPTYTerm();//This should be used but only after the TerminalContainer patch is applied
        basicTermControl.ConPTYTerm = existingTerm;
    }

    public class DataBinds : INotifyPropertyChanged
    {
        public void TriggerPropChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public static string StartupCommand => "powershell.exe";

        private static Color AlphaOverrideColor(Color color, byte alphaOverride) => Color.FromArgb(alphaOverride, color.R, color.G, color.B);

        private static readonly Color BackroundColor = Color.FromArgb(255, 0, 0, 30);

        public event PropertyChangedEventHandler? PropertyChanged;

        public static SolidColorBrush BackroundColorBrush => new(BackroundColor);

        public TerminalTheme Theme { get; set; } = new()
        {
            DefaultBackground = TerminalControl.ColorToVal(BackroundColor),
            DefaultForeground = TerminalControl.ColorToVal(Colors.White),
            DefaultSelectionBackground = 0xcccccc,
            //SelectionBackgroundAlpha = 0.5f,
            CursorStyle = CursorStyle.BlinkingBar,
            ColorTable = [0x0C0C0C, 0x1F0FC5, 0x0EA113, 0x009CC1, 0xDA3700, 0x981788, 0xDD963A, 0xCCCCCC, 0x767676, 0x5648E7, 0x0CC616, 0xA5F1F9, 0xFF783B, 0x9E00B4, 0xD6D661, 0xF2F2F2],
        };
    }

    private async void RefocusKB()
    {
        await Task.Delay(50);
        basicTermControl.Focus();
        Keyboard.Focus(basicTermControl);
    }

    private void ShowBufferClicked(object sender, RoutedEventArgs e)
    {
        var msg = basicTermControl.ConPTYTerm.GetConsoleText();
        if (Keyboard.IsKeyDown(Key.LeftShift))
            basicTermControl.ConPTYTerm.ConsoleOutputLog.Clear();
        else
            MessageBoxShow(msg);

        RefocusKB();
    }

    private void ClearTermClicked(object sender, RoutedEventArgs e)
    {
        basicTermControl.ConPTYTerm.ClearUITerminal();
        RefocusKB();
    }

    private void RestartTermClicked(object sender, RoutedEventArgs e)
    {
        _ = basicTermControl.RestartTerm();
        RefocusKB();
    }

    private void DuplicateClicked(object sender, RoutedEventArgs e)
    {
        // Don't really recommend doing this basic cloning we will sync our size at least so the positionings are correct.
        var wind = new MainWindow(basicTermControl.ConPTYTerm);
        if (MirrorMode)
#pragma warning disable CS0162 // Unreachable code detected
            wind.SizeChanged += (sender, _) => MirrorSizeChanged(sender);
#pragma warning restore CS0162 // Unreachable code detected
        else
            basicTermControl.DisconnectConPTYTerm();
        wind.Show();
    }

    private void MirrorSizeChanged(object sender)
    {
        var wind = (MainWindow)sender;

        Width = wind.Width;
        Height = wind.Height;
    }

    private void ShowProcessOutputClicked(object sender, RoutedEventArgs e)
    {
        var wind = new ProcessOutput();
        wind.Show();
    }

    private static void MessageBoxShow(string msg) => MessageBox.Show(msg);
}
