using System;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace AlleyCat.Mesh
{
    public class BlendMap
    {
        public int Width { get; }

        public int Height { get; }

        public Vector3 Min { get; }

        public Vector3 Max { get; }

        protected Arr<Rgba32> Data { get; }

        public BlendMap(IFileInfo file, Vector3 min, Vector3 max)
        {
            Ensure.That(file, nameof(file)).IsNotNull();

            Min = min;
            Max = max;

            using (var stream = file.CreateReadStream())
            {
                var image = Image.Load<Rgba32>(stream);

                Width = image.Width;
                Height = image.Height;

                Data = image.GetPixelSpan().ToArray();
            }
        }

        public Vector3 GetOffset(Vector2 uv)
        {
            var index = (int) (Width * uv.y + uv.x);

            if (Data.Count <= index)
            {
                throw new ArgumentOutOfRangeException(nameof(uv),
                    $"The specified coordinate ({uv.x}, {uv.y}) lies outside the image area ({Width} x {Height}).");
            }

            var color = Data[index].ToVector4();

            return new Vector3(Convert(color.X, 0), Convert(color.Y, 1), Convert(color.Z, 2));
        }

        private float Convert(float value, int index) => value / (Max[index] - Min[index]) + Min[index];
    }
}
