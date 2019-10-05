using System;
using System.Linq;
using AlleyCat.IO;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using static Godot.File;
using Color = SixLabors.ImageSharp.Color;

namespace AlleyCat.Mesh
{
    public class BlendMapWriter : BlendMapConverter
    {
        public int Size
        {
            get => _size;
            set => _size = Mathf.Max(value, 100);
        }

        public float Padding
        {
            get => _padding;
            set => _padding = Mathf.Max(value, 0f);
        }

        public float Tolerance
        {
            get => _tolerance;
            set => _tolerance = Mathf.Max(value, 0f);
        }

        private int _size = 512;

        private float _padding = 2f;

        private float _tolerance = 0.001f;

        public BlendMapWriter()
        {
        }

        public BlendMapWriter(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public void Write(IMeshData<MorphedVertex> data, string name, DirectoryInfo directory)
        {
            Ensure.That(data, nameof(data)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            Logger.LogInformation("Generating blend map '{}' under '{}'.", name, directory.Path);

            var position = Generate(directory.GetFile($"{name}.png"), data, v => v.Position());
            var normal = Generate(directory.GetFile($"{name}.normal.png"), data, v => v.Normal());

            var manifest = directory.GetFile($"{name}.json");

            Logger.LogDebug("Writing blend map metadata: '{}'.", manifest);

            using (var file = new File())
            {
                var metadata = new Metadata(data.Key, position, normal);
                var json = JsonConvert.SerializeObject(metadata, SerializerSettings);

                file.Open(manifest.Path, ModeFlags.Write);
                file.StoreString(json);
                file.Close();
            }
        }

        protected TextureMetadata Generate(
            FileInfo file, 
            IMeshData<MorphedVertex> data, 
            Func<IVertex, Vector3> extractor)
        {
            Logger.LogDebug("Generating blend map: '{}'.", file.Path);

            var (min, max) = DetermineRange(data, extractor);

            using (var texture = new Image<Rgba32>(Size, Size))
            {
                texture.Mutate(ctx => Draw(ctx, data, extractor, min, max));

                using (var stream = file.CreateWriteStream())
                {
                    texture.Save(stream, new PngEncoder());
                }
            }

            return new TextureMetadata(file.Name, min, max);
        }

        protected void Draw(
            IImageProcessingContext image,
            IMeshData<MorphedVertex> data,
            Func<IVertex, Vector3> extractor,
            Vector3 min,
            Vector3 max)
        {
            Color ToColor(Vector3 value)
            {
                var red = ToColorComponent(value, 0);
                var green = ToColorComponent(value, 1);
                var blue = ToColorComponent(value, 2);

                return Color.FromRgb(red, green, blue);
            }

            byte ToColorComponent(Vector3 value, int index)
            {
                var length = max[index] - min[index];

                return length > 0 ? (byte) ((value[index] - min[index]) / length * 255) : (byte) min[index];
            }

            bool Validate(Triangle<MorphedVertex> triangle) => triangle.Points
                .Map(p => extractor(p) - extractor(p.Basis))
                .Map(v => v.Length())
                .ForAll(v => v >= Tolerance);

            (PointF[] path, Color[] colors) CalculatePath(Arr<MorphedVertex> points)
            {
                var path = points
                    .Bind(p => p.UV())
                    .Map(p => p * Size)
                    .Map(p => new PointF(p.x, p.y))
                    .ToArr();

                var center = path.Aggregate((p1, p2) => p1 + p2) / path.Count;

                PointF Pad(PointF point)
                {
                    var (x, y) = point - center;
                    var direction = new Vector2(x, y).Normalized();

                    return point + new PointF(direction.x, direction.y) * Padding;
                }

                var colors = points
                    .Map(p => extractor(p) - extractor(p.Basis))
                    .Map(ToColor);

                return (path.Map(Pad).ToArray(), colors.ToArray());
            }

            Logger.LogDebug($"Processing {data.Count / 3} triangles.");

            data
                .Triangles()
                .Filter(Validate)
                .Map(t => t.Points)
                .Map(CalculatePath)
                .Iter(v =>
                {
                    var (path, colors) = v;
                    var brush = new PathGradientBrush(path, colors);

                    image.FillPolygon(brush, path);
                });
        }

        protected (Vector3, Vector3) DetermineRange(IMeshData<MorphedVertex> data, Func<IVertex, Vector3> extractor)
        {
            Vector3 Agg(Func<float, float, float> agg, Vector3 v1, Vector3 v2) =>
                new Vector3(agg(v1.x, v2.x), agg(v1.y, v2.y), agg(v1.z, v2.z));

            Vector3 Min(Vector3 d1, Vector3 d2) => Agg(Math.Min, d1, d2);
            Vector3 Max(Vector3 d1, Vector3 d2) => Agg(Math.Max, d1, d2);

            (Vector3, Vector3) MinMax((Vector3, Vector3) agg, Vector3 v) => (Min(v, agg.Item1), Max(v, agg.Item2));

            return data.Map(v => extractor(v) - extractor(v.Basis)).Fold((Vector3.Zero, Vector3.Zero), MinMax);
        }
    }
}
