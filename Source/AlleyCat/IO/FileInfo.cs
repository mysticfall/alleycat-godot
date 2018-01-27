using System;
using System.IO;
using EnsureThat;
using JetBrains.Annotations;
using File = Godot.File;

namespace AlleyCat.IO
{
    public class FileInfo : IWritableFileInfo
    {
        public const string Separator = "/";

        [NotNull]
        public string Name { get; }

        [NotNull]
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
                    file.Open(Path, (int) File.ModeFlags.Read);

                    return file.GetLen();
                }
            }
        }

        [CanBeNull]
        public string PhysicalPath => null;

        public DateTimeOffset LastModified
        {
            get
            {
                using (var file = new File())
                {
                    return new DateTimeOffset(file.GetModifiedTime(Path), TimeSpan.Zero);
                }
            }
        }

        public FileInfo([NotNull] string path)
        {
            Ensure.String.IsNotNullOrWhiteSpace(path, nameof(path));

            Path = path;

            var index = Path.LastIndexOf("/", StringComparison.InvariantCulture);

            Name = index < 0 ? "" : Path.Substring(index + 1);
        }

        [NotNull]
        public Stream CreateReadStream() => FileStream.Open(Path);

        [NotNull]
        public Stream CreateWriteStream() => FileStream.Open(Path, FileAccess.Write);

        [NotNull]
        public Stream CreateReadWriteStream() => FileStream.Open(Path, FileAccess.ReadWrite);

        public override string ToString() => $"FileInfo({Path})";
    }
}
