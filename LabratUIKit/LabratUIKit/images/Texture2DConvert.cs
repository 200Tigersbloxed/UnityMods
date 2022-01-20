using System.Drawing;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Color = System.Drawing.Color;

namespace LabratUIKit.images
{
    public static class ImageTools
    {
        [CanBeNull]
        public static byte[] GetImageBytes(string filename, string fileending)
        {
            using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"LabratUIKit.images.{filename}.{fileending}"))
            using (MemoryStream ms = new MemoryStream())
            {
                if (stream != null)
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
                LogHelper.Error("Stream is null!", ShouldStackFrame:false);
            }
            return null;
        }
    }
    
    public class Texture2DConvert
    {
        private readonly Bitmap Image;
        
        public Texture2DConvert([NotNull]Bitmap Texture) => this.Image = Texture;

        public Texture2DConvert([NotNull]byte[] ImageMemory)
        {
            using (MemoryStream ms = new MemoryStream(ImageMemory))
                Image = new Bitmap(ms);
        }

        [NotNull] public Texture2D ToTexture2D()
        {
            Texture2D texture = new Texture2D(Image.Width, Image.Height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Trilinear
            };
            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    Color pixelColor = Image.GetPixel(x, y);
                    UnityEngine.Color unity_pixelColor =
                        new UnityEngine.Color(pixelColor.R / 255.0f, pixelColor.G / 255.0f, 
                            pixelColor.B / 255.0f, pixelColor.A / 255.0f);
                    texture.SetPixel(x, Image.Height - y, unity_pixelColor);
                }
            }
            texture.Apply();
            return texture;
        }
    }
}