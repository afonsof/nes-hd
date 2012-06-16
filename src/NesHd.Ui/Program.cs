using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NesHd.Ui.Properties;
using NesHd.Ui.WinForms;

namespace NesHd.Ui
{
    internal static class Program
    {
        public static Settings Settings { get; private set; }

        public static MainForm MainForm { get; set; }

        /// <summary>
        /// Get the version
        /// </summary>
        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command lines</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Let's see if My Nes is running or not
            foreach (var process in Process.GetProcesses())
            {
                if (process.MainWindowTitle.Length >= "My Nes".Length)
                {
                    if (process.MainWindowTitle.Substring(0, "My Nes".Length) == "My Nes" &
                        process != Process.GetCurrentProcess())
                    {
                        process.CloseMainWindow(); //Kill the old one
                        break;
                    }
                }
            }
            //Load settings
            Settings = new Settings();
            Settings.Reload();
            //Build control profile if it's the first time of launch
            Frm_ControlsSettings.BuildControlProfile();
            //Get paths..... I never trust the ./
            try
            {
                if (Settings.StateFloder.Substring(0, 2) == @".\")
                {
                    Settings.StateFloder = Path.GetFullPath(Settings.StateFloder);
                }
                if (Settings.SnapshotsFolder.Substring(0, 2) == @".\")
                {
                    Settings.SnapshotsFolder = Path.GetFullPath(Settings.SnapshotsFolder);
                }
            }
            catch
            {
            }
            //Load the main window
            MainForm = new MainForm(args);
            MainForm.LoadSettings();
            MainForm.ShowDialogs();
            Application.Run(MainForm);
        }

        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="filePath">Full path of the file</param>
        /// <returns>Return file size + unit lable</returns>
        public static string GetFileSize(string filePath)
        {
            if (File.Exists(Path.GetFullPath(filePath)))
            {
                var info = new FileInfo(filePath);
                var unit = " Byte";
                double len = info.Length;
                if (info.Length >= 1024)
                {
                    len = info.Length/1024.00;
                    unit = " KB";
                }
                if (len >= 1024)
                {
                    len /= 1024.00;
                    unit = " MB";
                }
                if (len >= 1024)
                {
                    len /= 1024.00;
                    unit = " GB";
                }
                return len.ToString("F2") + unit;
            }
            return "";
        }
    }
}