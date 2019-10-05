using System;
using System.IO;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using File = Godot.File;

namespace AlleyCat.IO
{
    public struct FileInfo : IWritableFileInfo
    {
        public const string Separator = "/";

        public string Name { get; }

        public string Path { get; }

        public bool Exists
        {
            get
            {
                using (var file = new File())
                {
                    return file.FileExists(Path);
                }
            }
        }

        public bool IsDirectory => false;

        public long Length
        {
            get
            {
                using (var file = new File())
                {
                    file.Open(Path, File.ModeFlags.Read);

                    return file.GetLen();
                }
            }
        }

        [CanBeNull]
        public string PhysicalPath => ProjectSettings.GlobalizePath(Path);

        public DateTimeOffset LastModified
        {
            get
            {
                using (var file = new File())
                {
                    return new DateTimeOffset((long) file.GetModifiedTime(Path), TimeSpan.Zero);
                }
            }
        }

        public DirectoryInfo Directory
        {
            get
            {
                var index = Path.LastIndexOf(Separator, StringComparison.Ordinal);

                return new DirectoryInfo(Path.Substring(0, index));
            }
        }

        public FileInfo(string path)
        {
            Ensure.That(path, nameof(path)).IsNotNull();

            Path = path;

            var index = Path.LastIndexOf("/", StringComparison.InvariantCulture);

            Name = index < 0 ? "" : Path.Substring(index + 1);
        }

        public Stream CreateReadStream() => FileStream.Open(Path);

        public Stream CreateWriteStream() => FileStream.Open(Path, FileAccess.Write);

        public Stream CreateReadWriteStream() => FileStream.Open(Path, FileAccess.ReadWrite);

        public override string ToString() => $"FileInfo({Path})";
    }
}
