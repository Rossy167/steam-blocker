using System;
using System.Windows;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Steam_Blocker_UI
{
    public partial class MainWindow : Window
    {
        private string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow()
        {
            InitializeComponent();
            CommandTextBox.IsReadOnly = true;

        }


        private void runPSCommand(string command)
        { 
            using (Process cmd = new Process())
            {
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("powershell -command " + command);
                cmd.StandardInput.Flush();

                cmd.StandardInput.Close();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            runPSCommand("Block-Steam");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\Program Files (x86)\\Common Files\\Steam\\SteamService.exe' -Profile any -Direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\Program Files (x86)\\Steam\\bin\\cef\\cef.win7x64\\steamwebhelper.exe' -Profile any -Direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\program files (x86)\\steam\\steam.exe' -Profile any -direction Outbound -Displayname 'Block-Steam Rossy' | Out-Null");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\Program Files (x86)\\Common Files\\Steam\\SteamService.exe' -Profile any -Direction Inbound -Displayname 'Block-Steam Rossy' | Out-Null");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\Program Files (x86)\\Steam\bin\\cef\\cef.win7x64\\steamwebhelper.exe' -Profile any -Direction Inbound -Displayname 'Block-Steam Rossy' | Out-Null");
            runPSCommand("New-NetFirewallRule -Action block -Program 'C:\\program files (x86)\\steam\\steam.exe' -Profile any -direction Inbound -Displayname 'Block-Steam Rossy' | Out-Null");
            CommandTextBox.Text = "Steam has been blocked";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            runPSCommand("Get-NetFirewallRule | Where-Object DisplayName -eq 'Block-Steam Rossy' | Remove-NetFirewallRule | Out-Null");
            CommandTextBox.Text = "Steam has been unblocked";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            runPSCommand("cd " + currentDir);
            runPSCommand(".\\Save-BlockSteamToProfile.ps1");
            CommandTextBox.Text = "The other two buttons will now always work";
        }
    }
}
