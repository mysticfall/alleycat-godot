using EnsureThat;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace AlleyCat.IO
{
    public class FileProvider : IFileProvider
    {
        public IFileInfo GetFileInfo(string subPath)
        {
            Ensure.That(subPath, nameof(subPath)).IsNotNull();

            return new FileInfo(subPath);
        }

        public IDirectoryContents GetDirectoryContents(string subPath)
        {
            Ensure.That(subPath, nameof(subPath)).IsNotNull();
           
            return new DirectoryContents(subPath);
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
