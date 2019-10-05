using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AlleyCat.Mesh
{
    public abstract class BlendMapConverter : ILoggable
    {
        public JsonSerializerSettings SerializerSettings
        {
            get => _settings ?? DefaultSerializerSettings;
            set => _settings = value;
        }

        public ILogger Logger { get; }

        private JsonSerializerSettings _settings;

        protected BlendMapConverter() : this(new NullLoggerFactory())
        {
        }

        protected BlendMapConverter(ILoggerFactory loggerFactory)
        {
            Ensure.That(loggerFactory, nameof(loggerFactory)).IsNotNull();

            Logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        public static JsonSerializerSettings DefaultSerializerSettings
        {
            get
            {
                var resolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = resolver,
                    Formatting = Formatting.Indented,
                };

                return settings;
            }
        }

        protected class Metadata
        {
            public string Name { get; }

            public TextureMetadata Position { get; }

            public TextureMetadata Normal { get; }

            [JsonConstructor]
            public Metadata(string name, TextureMetadata position, TextureMetadata normal)
            {
                Name = name;
                Position = position;
                Normal = normal;
            }
        }

        protected class TextureMetadata
        {
            public string Texture { get; }

            public Vector3 Min { get; }

            public Vector3 Max { get; }

            [JsonConstructor]
            public TextureMetadata(string texture, Vector3 min, Vector3 max)
            {
                Texture = texture;
                Min = min;
                Max = max;
            }
        }
    }
}
