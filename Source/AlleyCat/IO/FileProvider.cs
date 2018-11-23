using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AlleyCat.IO
{
    public class FileProvider : IFileProvider
    {
        public IFileInfo GetFileInfo(string subPath) => new FileInfo(subPath);

        public IDirectoryContents GetDirectoryContents(string subPath) => new DirectoryContents(subPath);

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
