using System;
using System.IO;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Directory = Godot.Directory;

namespace AlleyCat.IO
{
    public class DirectoryInfo : IFileInfo
    {
        public string Name { get; }

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
        public string PhysicalPath => ProjectSettings.GlobalizePath(Path);

        public DateTimeOffset LastModified { get; } = new DateTimeOffset(0, TimeSpan.Zero);

        public DirectoryInfo(string path)
        {
            Path = path;

            var index = Path.LastIndexOf("/", StringComparison.InvariantCulture);

            Name = index < 0 ? "" : Path.Substring(index + 1);
        }

        public Stream CreateReadStream()
        {
            throw new InvalidOperationException($"The path represents  a directory: '{Path}'.");
        }

        public override string ToString() => $"DirectoryInfo({Path})";
    }
}
