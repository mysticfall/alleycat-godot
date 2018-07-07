using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AlleyCat.IO
{
    public class FileProvider : IFileProvider
    {
        [NotNull]
        public IFileInfo GetFileInfo([NotNull] string subPath) => new FileInfo(subPath);

        [NotNull]
        public IDirectoryContents GetDirectoryContents([NotNull] string subPath) =>
            new DirectoryContents(subPath);

        [NotNull]
        public IChangeToken Watch([NotNull] string filter) => NullChangeToken.Singleton;
    }
}
