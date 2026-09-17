using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Steam_Blocker_WPF
{
    public partial class MainWindow : Window
    {
        private readonly string scriptPath;

        public MainWindow()
        {
            InitializeComponent();
            scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "Steam-Blocker.ps1");
        }

        private async Task<CommandResult> RunScriptAsync(string mode)
        {
            if (!File.Exists(scriptPath))
            {
                return new CommandResult(1, string.Empty, "The PowerShell engine was not found next to the application.");
            }

            return await Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0\\powershell.exe"),
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + scriptPath + "\" -Mode " + mode,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    var output = process.StandardOutput.ReadToEndAsync();
                    var error = process.StandardError.ReadToEndAsync();
                    process.WaitForExit();
                    return new CommandResult(process.ExitCode, output.Result, error.Result);
                }
            });
        }

        private async Task ExecuteAsync(string mode, string workingText, string successText)
        {
            SetBusy(true, workingText);
            try
            {
                var result = await RunScriptAsync(mode);
                if (result.ExitCode == 0)
                {
                    StatusText.Text = successText;
                    DetailsText.Text = result.Output.Trim();
                    StatusText.Foreground = new SolidColorBrush(Color.FromRgb(101, 214, 163));
                }
                else
                {
                    StatusText.Text = "Operation could not be completed";
                    DetailsText.Text = string.IsNullOrWhiteSpace(result.Error) ? result.Output.Trim() : result.Error.Trim();
                    StatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 132, 132));
                }
            }
            catch (Exception exception)
            {
                StatusText.Text = "Unexpected error";
                DetailsText.Text = exception.Message;
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 132, 132));
            }
            finally
            {
                SetBusy(false, string.Empty);
            }
        }

        private void SetBusy(bool isBusy, string message)
        {
            BlockSteamButton.IsEnabled = !isBusy;
            UnblockSteamButton.IsEnabled = !isBusy;
            ValidateSteamButton.IsEnabled = !isBusy;
            ActivityBar.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
            if (isBusy)
            {
                StatusText.Text = message;
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(243, 246, 250));
            }
        }

        private async void BlockSteam(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync("Block", "Applying firewall rules...", "Steam is blocked");
        }

        private async void UnblockSteamButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync("Unblock", "Removing firewall rules...", "Steam is unblocked");
        }

        private async void ValidateSteamButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteAsync("Validate", "Locating Steam...", "Steam installation found");
        }

        private sealed class CommandResult
        {
            public CommandResult(int exitCode, string output, string error)
            {
                ExitCode = exitCode;
                Output = output;
                Error = error;
            }

            public int ExitCode { get; private set; }
            public string Output { get; private set; }
            public string Error { get; private set; }
        }
    }
}
