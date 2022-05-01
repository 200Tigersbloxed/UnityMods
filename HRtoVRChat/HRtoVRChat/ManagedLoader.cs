using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HRtoVRChat;

public static class ManagedLoader
{
    /// <summary>
    /// Loads all Assemblies from EmbeddedResources when AppDomain requests an assembly be resolved. 
    /// Huge shoutout to benaclejames for help with this! 
    /// https://github.com/benaclejames/VRCFaceTracking/blob/master/VRCFaceTracking/DependencyManager.cs#L30
    /// </summary>
    public static void Setup()
    {
        AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string dll = new AssemblyName(e.Name).Name + ".dll";
            List<string> resources = assembly.GetManifestResourceNames().Where(resource => resource.EndsWith(dll)).ToList();
            if (resources.Count > 0)
            {
                string resource = resources[0];
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            byte[] data = ms.ToArray();
                            return Assembly.Load(data);
                        }
                    }
                    return null;
                }
            }
            return null;
        };
    }
}