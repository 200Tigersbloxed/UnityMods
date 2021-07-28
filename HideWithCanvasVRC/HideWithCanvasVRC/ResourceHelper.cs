using System.IO;
using UnityEngine;

namespace HideWithCanvasVRC
{
    public static class ResourceHelper
    {
        public static readonly string directory = Path.Combine(Application.streamingAssetsPath, "hidewithcanvasvrc");

        public static void Init()
        {
            // Create Directory for stuff
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if (!File.Exists(Path.Combine(directory, "hidewithcanvasvrc")))
                File.WriteAllBytes(Path.Combine(directory, "hidewithcanvasvrc"), Resources.hidewithcanvasvrc);
        }
    }
}
