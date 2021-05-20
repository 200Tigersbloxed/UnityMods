using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace LabratEyeTracking
{
    public static class UnmanagedAssemblyManager
    {
        public static string dir = Directory.GetParent(Application.dataPath).ToString();
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
            switch (sdkType)
            {
                case 1:
                    foreach (string lib in srLibs)
                    {
                        switch (lib)
                        {
                            case "libHTC_License.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.libHTC_License);
                                break;
                            case "nanomsg.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.nanomsg);
                                break;
                            case "SRWorks_Log.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.SRWorks_Log);
                                break;
                            case "ViveSR_Client.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.ViveSR_Client);
                                break;
                            case "SRanipal.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.SRanipal);
                                break;
                        }
                    }
                    break;
                case 2:
                    foreach(string lib in pmLibs)
                    {
                        switch (lib)
                        {
                            case "PimaxEyeTracker.dll":
                                if (!File.Exists(Path.Combine(dir, lib)))
                                    File.WriteAllBytes(Path.Combine(dir, lib), Properties.Resources.PimaxEyeTracker);
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
