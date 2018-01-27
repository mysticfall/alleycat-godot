using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AlleyCat.IO
{
    public class FileProvider : IFileProvider
    {
        [NotNull]
        public IFileInfo GetFileInfo([NotNull] string subpath) => new FileInfo(subpath);

        [NotNull]
        public IDirectoryContents GetDirectoryContents([NotNull] string subpath) =>
            new DirectoryContents(subpath);

        [NotNull]
        public IChangeToken Watch([NotNull] string filter) => NullChangeToken.Singleton;
    }
}
