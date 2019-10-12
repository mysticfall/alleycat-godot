using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using AlleyCat.IO;
using AlleyCat.Logging;
using AlleyCat.Mesh.Generic;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using static Godot.File;
using static Godot.Viewport;

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

        protected Node Context { get; }

        private int _size = 512;

        private float _padding = 2f;

        private float _tolerance = 0.001f;

        public BlendMapWriter(Node context) : this(context, new NullLoggerFactory())
        {
        }

        public BlendMapWriter(Node context, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            Context = context;
        }

        public void Write(IMeshData<MorphableVertex> data, string name, DirectoryInfo directory)
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
            IMeshData<MorphableVertex> data,
            Func<IVertex, Vector3> extractor)
        {
            Logger.LogDebug("Generating blend map: '{}'.", file.Path);

            var (min, max) = DetermineRange(data, extractor);

            var canvas = new Canvas(ctx => { Draw(ctx, data, extractor, min, max); })
            {
                RectSize = new Vector2(Size, Size),
                AnchorRight = 1,
                AnchorBottom = 1
            };

            var viewport = new Viewport
            {
                Size = new Vector2(Size, Size),
                Usage = UsageEnum.Usage2d,
                Disable3d = true,
                Hdr = false,
                GuiDisableInput = true,
                RenderTargetUpdateMode = UpdateMode.Once,
                RenderTargetVFlip = true
            };

            viewport.AddChild(canvas);
            Context.AddChild(viewport);

            canvas.OnProcess()
                .Where(_ => canvas.Finished)
                .Skip(1)
                .Do(_ => viewport.GetTexture().GetData().SavePng(file.Path).ThrowOnError())
                .Do(_ => viewport.QueueFree())
                .Subscribe(this);

            return new TextureMetadata(file.Name, min, max);
        }

        protected void Draw(
            CanvasItem canvas,
            IMeshData<MorphableVertex> data,
            Func<IVertex, Vector3> extractor,
            Vector3 min,
            Vector3 max)
        {
            Color ToColor(Vector3 value)
            {
                var red = ToColorComponent(value, 0);
                var green = ToColorComponent(value, 1);
                var blue = ToColorComponent(value, 2);

                return new Color(red, green, blue);
            }

            float ToColorComponent(Vector3 value, int index)
            {
                var length = max[index] - min[index];

                return length > 0 ? (value[index] - min[index]) / length : min[index];
            }

            bool Validate(Triangle<MorphableVertex> triangle) => triangle.Points
                .Map(p => extractor(p) - extractor(p.Basis))
                .Map(v => v.Length())
                .ForAll(v => v >= Tolerance);

            (Vector2[] path, Color[] colors) CalculatePath(Arr<MorphableVertex> points)
            {
                var path = points
                    .Bind(p => p.UV())
                    .Map(p => p * Size)
                    .Map(p => new Vector2(p.x, p.y))
                    .ToArr();

                var center = path.Aggregate((p1, p2) => p1 + p2) / path.Count;

                Vector2 Pad(Vector2 point)
                {
                    var p = point - center;
                    var direction = new Vector2(p.x, p.y).Normalized();

                    return point + new Vector2(direction.x, direction.y) * Padding;
                }

                var colors = points
                    .Map(p => extractor(p) - extractor(p.Basis))
                    .Map(ToColor);

                return (path.Map(Pad).ToArray(), colors.ToArray());
            }

            Logger.LogDebug($"Processing {data.Count / 3} triangles.");

            canvas.DrawRect(new Rect2(0, 0, Size, Size), ToColor(Vector3.Zero));

            data
                .Triangles()
                .Filter(Validate)
                .Map(t => t.Points)
                .Map(CalculatePath)
                .Iter(v => canvas.DrawPolygon(v.path, v.colors));
        }

        protected (Vector3, Vector3) DetermineRange(IMeshData<MorphableVertex> data, Func<IVertex, Vector3> extractor)
        {
            Vector3 Agg(Func<float, float, float> agg, Vector3 v1, Vector3 v2) =>
                new Vector3(agg(v1.x, v2.x), agg(v1.y, v2.y), agg(v1.z, v2.z));

            Vector3 Min(Vector3 d1, Vector3 d2) => Agg(Math.Min, d1, d2);
            Vector3 Max(Vector3 d1, Vector3 d2) => Agg(Math.Max, d1, d2);

            (Vector3, Vector3) MinMax((Vector3, Vector3) agg, Vector3 v) => (Min(v, agg.Item1), Max(v, agg.Item2));

            return data.Map(v => extractor(v) - extractor(v.Basis)).Fold((Vector3.Zero, Vector3.Zero), MinMax);
        }

        internal class Canvas : ColorRect
        {
            private readonly Action<CanvasItem> _callback;

            public bool Finished { get; private set; }

            public Canvas(Action<CanvasItem> callback)
            {
                _callback = callback;
            }

            public override void _Draw()
            {
                base._Draw();

                if (Finished) return;

                Finished = true;

                _callback(this);
            }
        }
    }
}
