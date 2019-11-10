using System;
using System.IO;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Directory = Godot.Directory;

namespace AlleyCat.IO
{
    public struct DirectoryInfo : IFileInfo, IEquatable<DirectoryInfo>
    {
        public string Name { get; }

        public string Path { get; }

        public bool Exists
        {
            get
            {
                using (var directory = new Directory())
                {
                    return directory.Open(Path) == Error.Ok;
                }
            }
        }

        public bool IsDirectory => true;

        public long Length => -1;

        [CanBeNull]
        public string PhysicalPath => ProjectSettings.GlobalizePath(Path);

        public DateTimeOffset LastModified => new DateTimeOffset(0, TimeSpan.Zero);

        public DirectoryInfo(string path)
        {
            Path = path;

            var index = Path.LastIndexOf("/", StringComparison.InvariantCulture);

            Name = index < 0 ? "" : Path.Substring(index + 1);
        }

        public IDirectoryContents Contents => new FileProvider().GetDirectoryContents(Path);

        public FileInfo GetFile(string name) => new FileInfo(string.Join(FileInfo.Separator, Path, name));

        public Stream CreateReadStream()
        {
            throw new InvalidOperationException($"The path represents  a directory: '{Path}'.");
        }

        public void Create(bool recursive = true)
        {
            using (var directory = new Directory())
            {
                if (recursive)
                {
                    directory.MakeDir(Path).ThrowOnError();
                }
                else
                {
                    directory.MakeDirRecursive(Path).ThrowOnError();
                }
            }
        }

        public override string ToString() => $"DirectoryInfo({Path})";

        public bool Equals(DirectoryInfo other) => Path == other.Path;

        public override bool Equals(object obj) => obj is DirectoryInfo other && Equals(other);

        public override int GetHashCode() => Path != null ? Path.GetHashCode() : 0;

        public static bool operator ==(DirectoryInfo left, DirectoryInfo right) => left.Equals(right);

        public static bool operator !=(DirectoryInfo left, DirectoryInfo right) => !left.Equals(right);
    }
}
