using System;
using AlleyCat.Common;
using AlleyCat.IO;
using Godot;

namespace AlleyCat.Mesh
{
    public class BlendMap : IDisposable
    {
        public int Width { get; }

        public int Height { get; }

        public Vector3 Min { get; }

        public Vector3 Max { get; }

        protected Image Image { get; }

        public BlendMap(FileInfo file, Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;

            Image = new Image();

            Image.Load(file.Path).ThrowOnError();
            Image.Lock();

            Width = Image.GetWidth();
            Height = Image.GetHeight();
        }

        public Vector3 GetOffset(Vector2 uv)
        {
            var x = (int) (uv.x * Width);
            var y = (int) (uv.y * Height);

            var color = Image.GetPixel(x, y);

            return new Vector3(Convert(color.r, 0), Convert(color.g, 1), Convert(color.b, 2));
        }

        public void Dispose() => Image.Dispose();

        private float Convert(float value, int index) => value * (Max[index] - Min[index]) + Min[index];
    }
}
