﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WinForm_Launcher.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SchackMediaViewer.exe")]
        public string ScreenSaverAppFilenameAndExtension {
            get {
                return ((string)(this["ScreenSaverAppFilenameAndExtension"]));
            }
            set {
                this["ScreenSaverAppFilenameAndExtension"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Media Schack")]
        public string ScreenSaverDisplayNameInControlPanel {
            get {
                return ((string)(this["ScreenSaverDisplayNameInControlPanel"]));
            }
            set {
                this["ScreenSaverDisplayNameInControlPanel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseAltScreenSaverDirectory {
            get {
                return ((bool)(this["UseAltScreenSaverDirectory"]));
            }
            set {
                this["UseAltScreenSaverDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AltScreenSaverDirectory {
            get {
                return ((string)(this["AltScreenSaverDirectory"]));
            }
            set {
                this["AltScreenSaverDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool dbgUseSeparateScreenSaverAppPerLaunchMode {
            get {
                return ((bool)(this["dbgUseSeparateScreenSaverAppPerLaunchMode"]));
            }
            set {
                this["dbgUseSeparateScreenSaverAppPerLaunchMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string dbgSepFullScreenAppFullyQualifiedPathAndFilename {
            get {
                return ((string)(this["dbgSepFullScreenAppFullyQualifiedPathAndFilename"]));
            }
            set {
                this["dbgSepFullScreenAppFullyQualifiedPathAndFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string dbgSepDesktopSettingsAppFullyQualifiedPathAndFilename {
            get {
                return ((string)(this["dbgSepDesktopSettingsAppFullyQualifiedPathAndFilename"]));
            }
            set {
                this["dbgSepDesktopSettingsAppFullyQualifiedPathAndFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string dbgSepCPSettingsAppFullyQualifiedPathAndFilename {
            get {
                return ((string)(this["dbgSepCPSettingsAppFullyQualifiedPathAndFilename"]));
            }
            set {
                this["dbgSepCPSettingsAppFullyQualifiedPathAndFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string dbgSepCPPreviewAppFullyQualifiedPathAndFilename {
            get {
                return ((string)(this["dbgSepCPPreviewAppFullyQualifiedPathAndFilename"]));
            }
            set {
                this["dbgSepCPPreviewAppFullyQualifiedPathAndFilename"] = value;
            }
        }
    }
}
