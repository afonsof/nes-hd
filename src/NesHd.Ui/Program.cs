/*
This file is part of NesHd
A Nintendo Entertainment System Emulator.

Copyright © 2012 Afonso França de Oliveira

NesHd is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

NesHd is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see http://www.gnu.org/licenses/

This file incorporates work covered by the following copyright and
permission notice:

Copyright © 2009 - 2010 Ala Hadid (AHD)
http://mynes.sourceforge.net.

Permission to use, copy, modify, and/or distribute this software
for any purpose with or without fee is hereby granted, provided
that the above copyright notice and this permission notice appear
in all copies.
 
THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL
WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE
AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR
CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */
using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using NesHd.Ui.Properties;
using NesHd.Ui.WinForms;

namespace NesHd.Ui
{
    static class Program
    {
        static Settings _settings;
        static Frm_Main _mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command lines</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Let's see if My Nes is running or not
            foreach (Process PR in Process.GetProcesses())
            {
                if (PR.MainWindowTitle.Length >= "My Nes".Length)
                {
                    if (PR.MainWindowTitle.Substring(0, "My Nes".Length) == "My Nes" & PR != Process.GetCurrentProcess())
                    {
                        PR.CloseMainWindow();//Kill the old one
                        break;
                    }
                }
            }
            //Load settings
            _settings = new Settings();
            _settings.Reload();
            //Build control profile if it's the first time of launch
            Frm_ControlsSettings.BuildControlProfile();
            //Get paths..... I never trust the ./
            try 
            {
                if (_settings.StateFloder.Substring(0, 2) == @".\")
                {
                    _settings.StateFloder = Path.GetFullPath(_settings.StateFloder);
                }
                if (_settings.SnapshotsFolder.Substring(0, 2) == @".\")
                {
                    _settings.SnapshotsFolder = Path.GetFullPath(_settings.SnapshotsFolder);
                }
            }
            catch { }
            //Load the main window
            _mainForm = new Frm_Main(args);
            _mainForm.LoadSettings();
            _mainForm.ShowDialogs();
            Application.Run(_mainForm);
        }
        public static Settings Settings
        { get { return _settings; } }
        public static Frm_Main Form_Main
        { get { return _mainForm; } set { _mainForm = value; } }
        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="FilePath">Full path of the file</param>
        /// <returns>Return file size + unit lable</returns>
        public static string GetFileSize(string FilePath)
        {
            if (File.Exists(Path.GetFullPath(FilePath)) == true)
            {
                FileInfo Info = new FileInfo(FilePath);
                string Unit = " Byte";
                double Len = Info.Length;
                if (Info.Length >= 1024)
                {
                    Len = Info.Length / 1024.00;
                    Unit = " KB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " MB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " GB";
                }
                return Len.ToString("F2") + Unit;
            }
            return "";
        }
        /// <summary>
        /// Get the version
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
