using System.Collections;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;

namespace AlleyCat.IO
{
    public class DirectoryContents : IDirectoryContents
    {
        private readonly string _path;

        public DirectoryContents([NotNull] string path)
        {
            Ensure.String.IsNotNullOrWhiteSpace(path, nameof(path));

            _path = path;
        }

        public IEnumerator<IFileInfo> GetEnumerator() => new FileEnumerator(_path);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Exists
        {
            get
            {
                using (var directory = new Directory())
                {
                    return directory.DirExists(_path);
                }
            }
        }
    }
}
