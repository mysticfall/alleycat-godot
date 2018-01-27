using System.IO;
using Microsoft.Extensions.FileProviders;

namespace AlleyCat.IO
{
    public interface IWritableFileInfo : IFileInfo
    {
        Stream CreateWriteStream();

        Stream CreateReadWriteStream();
    }
}
