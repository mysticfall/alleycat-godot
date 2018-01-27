using System;
using System.IO;
using EnsureThat;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Directory = Godot.Directory;

namespace AlleyCat.IO
{
    public class DirectoryInfo : IFileInfo
    {
        [NotNull]
        public string Name { get; }

        [NotNull]
        public string Path { get; }

        public bool Exists
        {
            get
            {
                using (var directory = new Directory())
                {
                    return directory.DirExists(Name);
                }
            }
        }

        public bool IsDirectory => true;

        public long Length => -1;

        [CanBeNull]
        public string PhysicalPath => null;

        public DateTimeOffset LastModified { get; } = new DateTimeOffset(0, TimeSpan.Zero);

        public DirectoryInfo([NotNull] string path)
        {
            Ensure.String.IsNotNullOrWhiteSpace(path, nameof(path));

            Path = path;

            var index = Path.LastIndexOf("/", StringComparison.InvariantCulture);

            Name = index < 0 ? "" : Path.Substring(index + 1);
        }

        [NotNull]
        public Stream CreateReadStream()
        {
            throw new InvalidOperationException($"The path represents  a directory: '{Path}'.");
        }

        public override string ToString() => $"DirectoryInfo({Path})";
    }
}
