using System.Collections;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using Microsoft.Extensions.FileProviders;

namespace AlleyCat.IO
{
    public class DirectoryContents : IDirectoryContents
    {
        private readonly string _path;

        public DirectoryContents(string path)
        {
            Ensure.That(path, nameof(path)).IsNotNull();

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
