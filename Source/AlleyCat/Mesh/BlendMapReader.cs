using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DirectoryInfo = AlleyCat.IO.DirectoryInfo;
using FileInfo = AlleyCat.IO.FileInfo;

namespace AlleyCat.Mesh
{
    public class BlendMapReader : BlendMapConverter
    {
        public BlendMapReader()
        {
        }

        public BlendMapReader(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public BlendMapSet Read(string path) => Read(new FileInfo(path));

        public BlendMapSet Read(FileInfo file)
        {
            Logger.LogDebug("Reading blend map metadata: '{}'.", file.Path);

            using (var reader = new StreamReader(file.CreateReadStream()))
            {
                var directory = file.Directory;
                var contents = reader.ReadToEnd();

                var metadata = JsonConvert.DeserializeObject<Metadata>(contents, SerializerSettings);

                Logger.LogDebug("Found blend map: '{}'.", metadata.Name);

                var position = CreateBlendMap(metadata.Position, directory);

                Logger.LogDebug("Read vertex position map: {}x{}.", position.Width, position.Height);

                var normal = CreateBlendMap(metadata.Normal, directory);

                Logger.LogDebug("Read vertex normal map: {}x{}.", normal.Width, normal.Height);

                return new BlendMapSet(metadata.Name, position, normal, metadata.Seams.ToHashSet());
            }
        }

        protected BlendMap CreateBlendMap(TextureMetadata metadata, DirectoryInfo directory)
        {
            var texture = directory.GetFile(metadata.Texture);

            return new BlendMap(texture, metadata.Min, metadata.Max);
        }
    }
}
