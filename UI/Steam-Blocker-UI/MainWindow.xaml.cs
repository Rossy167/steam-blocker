using System;
using System.Windows;
using System.Diagnostics;

namespace Steam_Blocker_UI
{
    public partial class MainWindow : Window
    {
        private string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow()
        {
            InitializeComponent();
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
            CommandTextBox.Text = "Steam has been blocked";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            runPSCommand("Unblock-Steam");
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
