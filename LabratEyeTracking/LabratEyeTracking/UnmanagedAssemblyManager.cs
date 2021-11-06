using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace LabratEyeTracking
{
    public static class UnmanagedAssemblyManager
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        public static string dir = Path.Combine(Application.streamingAssetsPath, "LabratEyeTracking");
        public static List<string> srLibs = new List<string>
        {
            "libHTC_License.dll",
            "nanomsg.dll",
            "SRWorks_Log.dll",
            "ViveSR_Client.dll",
            "SRanipal.dll"
        };
        public static List<string> pmLibs = new List<string>
        {
            "PimaxEyeTracker.dll"
        };

        public static void Initialize(int sdkType)
        {
            // Create Directory if it doesn't exist
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            switch (sdkType)
            {
                case 1:
                    foreach (string lib in srLibs)
                    {
                        string currentLib = Path.Combine(dir, lib);
                        bool dontload = false;
                        switch (lib)
                        {
                            case "libHTC_License.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.libHTC_License);
                                break;
                            case "nanomsg.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.nanomsg);
                                break;
                            case "SRWorks_Log.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.SRWorks_Log);
                                break;
                            case "ViveSR_Client.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.ViveSR_Client);
                                break;
                            case "SRanipal.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.SRanipal);
                                break;
                            default:
                                dontload = true;
                                break;
                        }
                        if (!dontload)
                            LoadUnmanagedAssembly(currentLib);
                    }
                    break;
                case 2:
                    foreach(string lib in pmLibs)
                    {
                        string currentLib = Path.Combine(dir, lib);
                        bool dontload = false;
                        switch (lib)
                        {
                            case "PimaxEyeTracker.dll":
                                if (!File.Exists(currentLib))
                                    File.WriteAllBytes(currentLib, Properties.Resources.PimaxEyeTracker);
                                break;
                            default:
                                dontload = true;
                                break;
                        }
                        if (!dontload)
                            LoadUnmanagedAssembly(currentLib);
                    }
                    break;
            }
        }

        private static void LoadUnmanagedAssembly(string libPath)
        {
            if (File.Exists(libPath))
                try { LoadLibrary(libPath); }
                catch { LogHelper.Error("Failed to load Library at Path: " + libPath); }
            else
                LogHelper.Error("Unmanaged Assembly does not exist at Path: " + libPath);
        }
    }
}
