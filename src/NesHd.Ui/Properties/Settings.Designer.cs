﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NesHd.Ui.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("195")]
        public int Win_X {
            get {
                return ((int)(this["Win_X"]));
            }
            set {
                this["Win_X"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("99")]
        public int Win_Y {
            get {
                return ((int)(this["Win_Y"]));
            }
            set {
                this["Win_Y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("563")]
        public int Win_H {
            get {
                return ((int)(this["Win_H"]));
            }
            set {
                this["Win_H"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("521")]
        public int Win_W {
            get {
                return ((int)(this["Win_W"]));
            }
            set {
                this["Win_W"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowDebugger {
            get {
                return ((bool)(this["ShowDebugger"]));
            }
            set {
                this["ShowDebugger"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int DebuggerWin_X {
            get {
                return ((int)(this["DebuggerWin_X"]));
            }
            set {
                this["DebuggerWin_X"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int DebuggerWin_Y {
            get {
                return ((int)(this["DebuggerWin_Y"]));
            }
            set {
                this["DebuggerWin_Y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public int DebuggerWin_H {
            get {
                return ((int)(this["DebuggerWin_H"]));
            }
            set {
                this["DebuggerWin_H"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("600")]
        public int DebuggerWin_W {
            get {
                return ((int)(this["DebuggerWin_W"]));
            }
            set {
                this["DebuggerWin_W"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection Recents {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["Recents"]));
            }
            set {
                this["Recents"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SlimDX")]
        public global::NesHd.Core.Output.Video.GraphicDevices GFXDevice {
            get {
                return ((global::NesHd.Core.Output.Video.GraphicDevices)(this["GFXDevice"]));
            }
            set {
                this["GFXDevice"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NTSC")]
        public global::NesHd.Core.TvFormat TV {
            get {
                return ((global::NesHd.Core.TvFormat)(this["TV"]));
            }
            set {
                this["TV"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Stretch")]
        public string Size {
            get {
                return ((string)(this["Size"]));
            }
            set {
                this["Size"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool NoLimiter {
            get {
                return ((bool)(this["NoLimiter"]));
            }
            set {
                this["NoLimiter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool SoundEnabled {
            get {
                return ((bool)(this["SoundEnabled"]));
            }
            set {
                this["SoundEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\\\Snapshots")]
        public string SnapshotsFolder {
            get {
                return ((string)(this["SnapshotsFolder"]));
            }
            set {
                this["SnapshotsFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".bmp")]
        public string SnapshotFormat {
            get {
                return ((string)(this["SnapshotFormat"]));
            }
            set {
                this["SnapshotFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\\\StateSaves")]
        public string StateFloder {
            get {
                return ((string)(this["StateFloder"]));
            }
            set {
                this["StateFloder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::NesHd.Core.PPU.PaletteFormat PaletteFormat {
            get {
                return ((global::NesHd.Core.PPU.PaletteFormat)(this["PaletteFormat"]));
            }
            set {
                this["PaletteFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Square1 {
            get {
                return ((bool)(this["Square1"]));
            }
            set {
                this["Square1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Square2 {
            get {
                return ((bool)(this["Square2"]));
            }
            set {
                this["Square2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DMC {
            get {
                return ((bool)(this["DMC"]));
            }
            set {
                this["DMC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Noize {
            get {
                return ((bool)(this["Noize"]));
            }
            set {
                this["Noize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Triangle {
            get {
                return ((bool)(this["Triangle"]));
            }
            set {
                this["Triangle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("195")]
        public int Browser_X {
            get {
                return ((int)(this["Browser_X"]));
            }
            set {
                this["Browser_X"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("99")]
        public int Browser_Y {
            get {
                return ((int)(this["Browser_Y"]));
            }
            set {
                this["Browser_Y"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("563")]
        public int Browser_H {
            get {
                return ((int)(this["Browser_H"]));
            }
            set {
                this["Browser_H"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("521")]
        public int Browser_W {
            get {
                return ((int)(this["Browser_W"]));
            }
            set {
                this["Browser_W"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowBrowser {
            get {
                return ((bool)(this["ShowBrowser"]));
            }
            set {
                this["ShowBrowser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::NesHd.Ui.Misc.MFolderCollection Folders {
            get {
                return ((global::NesHd.Ui.Misc.MFolderCollection)(this["Folders"]));
            }
            set {
                this["Folders"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("213")]
        public int Container1 {
            get {
                return ((int)(this["Container1"]));
            }
            set {
                this["Container1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("186")]
        public int Container2 {
            get {
                return ((int)(this["Container2"]));
            }
            set {
                this["Container2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("180")]
        public int Column_Name {
            get {
                return ((int)(this["Column_Name"]));
            }
            set {
                this["Column_Name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("84")]
        public int Column_Size {
            get {
                return ((int)(this["Column_Size"]));
            }
            set {
                this["Column_Size"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("86")]
        public int Column_Mapper {
            get {
                return ((int)(this["Column_Mapper"]));
            }
            set {
                this["Column_Mapper"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Details")]
        public global::System.Windows.Forms.View View {
            get {
                return ((global::System.Windows.Forms.View)(this["View"]));
            }
            set {
                this["View"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AutoSaveSRAM {
            get {
                return ((bool)(this["AutoSaveSRAM"]));
            }
            set {
                this["AutoSaveSRAM"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PauseWhenFocusLost {
            get {
                return ((bool)(this["PauseWhenFocusLost"]));
            }
            set {
                this["PauseWhenFocusLost"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-500")]
        public int Volume {
            get {
                return ((int)(this["Volume"]));
            }
            set {
                this["Volume"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Fullscreen {
            get {
                return ((bool)(this["Fullscreen"]));
            }
            set {
                this["Fullscreen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowMenuAndStatus {
            get {
                return ((bool)(this["ShowMenuAndStatus"]));
            }
            set {
                this["ShowMenuAndStatus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Stereo {
            get {
                return ((bool)(this["Stereo"]));
            }
            set {
                this["Stereo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::NesHd.Ui.Misc.ControlProfilesCollection ControlProfiles {
            get {
                return ((global::NesHd.Ui.Misc.ControlProfilesCollection)(this["ControlProfiles"]));
            }
            set {
                this["ControlProfiles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::NesHd.Ui.Misc.ControlProfile CurrentControlProfile {
            get {
                return ((global::NesHd.Ui.Misc.ControlProfile)(this["CurrentControlProfile"]));
            }
            set {
                this["CurrentControlProfile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool VRC6Pulse1 {
            get {
                return ((bool)(this["VRC6Pulse1"]));
            }
            set {
                this["VRC6Pulse1"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool VRC6Pulse2 {
            get {
                return ((bool)(this["VRC6Pulse2"]));
            }
            set {
                this["VRC6Pulse2"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool VRC6Sawtooth {
            get {
                return ((bool)(this["VRC6Sawtooth"]));
            }
            set {
                this["VRC6Sawtooth"] = value;
            }
        }
    }
}
