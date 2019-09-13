using System;
using System.Linq;
using AlleyCat.IO;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using static Godot.File;
using Color = SixLabors.ImageSharp.Color;

namespace AlleyCat.Mesh
{
    public class BlendMapGenerator
    {
        public IMeshData<MorphedVertex> Data { get; }

        public int Size { get; }

        public float Padding { get; }

        public float Tolerance { get; }

        protected ILogger Logger { get; }

        public BlendMapGenerator(
            IMeshData<MorphedVertex> data,
            int size = 512,
            float tolerance = 0.001f,
            float padding = 2f) : this(data, new NullLoggerFactory(), size, tolerance, padding)
        {
        }

        public BlendMapGenerator(
            IMeshData<MorphedVertex> data,
            ILoggerFactory loggerFactory,
            int size = 512,
            float tolerance = 0.001f,
            float padding = 2f)
        {
            Ensure.That(data, nameof(data)).IsNotNull();
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();
            Ensure.That(size, nameof(size)).IsGt(0);
            Ensure.That(tolerance, nameof(tolerance)).IsGte(0);
            Ensure.That(padding, nameof(padding)).IsGte(0);

            Data = data;
            Size = size;
            Tolerance = tolerance;
            Padding = padding;

            Logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        public void Generate(string prefix)
        {
            Ensure.That(prefix, nameof(prefix)).IsNotNull();

            Logger.LogInformation("Generating blend maps for '{}'.", prefix);

            var position = Generate(new FileInfo($"{prefix}.png"), v => v.Position());
            var normal = Generate(new FileInfo($"{prefix}.normal.png"), v => v.Normal());

            var metadata = new BlendMapMetadata(position, normal);

            var resolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented,
            };

            var json = JsonConvert.SerializeObject(metadata, settings);
            var path = $"{prefix}.json";

            Logger.LogDebug("Writing blend map metadata: '{}'.", path);

            using (var file = new File())
            {
                file.Open(path, ModeFlags.Write);
                file.StoreString(json);
                file.Close();
            }
        }

        protected BlendMapTextureMetadata Generate(FileInfo file, Func<IVertex, Vector3> extractor)
        {
            Logger.LogDebug("Generating blend map: '{}'.", file.Path);

            var (min, max) = DetermineRange(extractor);

            using (var texture = new Image<Rgba32>(Size, Size))
            {
                texture.Mutate(ctx => Draw(ctx, extractor, min, max));

                using (var stream = file.CreateWriteStream())
                {
                    texture.Save(stream, new PngEncoder());
                }
            }

            return new BlendMapTextureMetadata(file.Name, min, max);
        }

        protected void Draw(IImageProcessingContext image, Func<IVertex, Vector3> extractor, Vector3 min, Vector3 max)
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

            Logger.LogDebug($"Processing {Data.Count / 3} triangles.");

            Data
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

        protected (Vector3, Vector3) DetermineRange(Func<IVertex, Vector3> extractor)
        {
            Vector3 Agg(Func<float, float, float> agg, Vector3 v1, Vector3 v2) =>
                new Vector3(agg(v1.x, v2.x), agg(v1.y, v2.y), agg(v1.z, v2.z));

            Vector3 Min(Vector3 d1, Vector3 d2) => Agg(Math.Min, d1, d2);
            Vector3 Max(Vector3 d1, Vector3 d2) => Agg(Math.Max, d1, d2);

            (Vector3, Vector3) MinMax((Vector3, Vector3) agg, Vector3 v) => (Min(v, agg.Item1), Max(v, agg.Item2));

            return Data.Map(v => extractor(v) - extractor(v.Basis)).Fold((Vector3.Zero, Vector3.Zero), MinMax);
        }
    }
}
