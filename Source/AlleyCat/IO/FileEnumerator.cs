using System.Collections;
using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;

namespace AlleyCat.IO
{
    public class FileEnumerator : IEnumerator<IFileInfo>
    {
        private const string CurrentDir = ".";

        private const string ParentDir = "..";

        public IFileInfo Current => _current;

        object IEnumerator.Current => Current;

        private readonly Directory _directory;

        private IFileInfo _current;

        private readonly string _path;

        private readonly bool _skipNavigational;

        private readonly bool _skipHidden;

        private readonly bool _endsWithSeparator;

        public FileEnumerator(
            [NotNull] string path, bool skipNavigational = false, bool skipHidden = false)
        {
            Ensure.String.IsNotNullOrWhiteSpace(path, nameof(path));

            _path = path;
            _skipNavigational = skipNavigational;
            _skipHidden = skipHidden;

            _endsWithSeparator = _path.EndsWith(FileInfo.Separator);

            _directory = new Directory();

            try
            {
                _directory.Open(path).ThrowIfNecessary(
                    e => $"Failed to open directory: '{path}' ({e}).");
            }
            finally
            {
                _directory.Dispose();
            }

            _directory.ListDirBegin(skipNavigational, skipHidden);
        }

        public bool MoveNext()
        {
            var path = _directory.GetNext();

            while (path == CurrentDir || path == ParentDir)
            {
                path = _directory.GetNext();
            }

            var hasNext = !string.IsNullOrEmpty(path);

            if (hasNext)
            {
                var absolute = string.Join(_endsWithSeparator ? "" : "/", _path, path);

                if (_directory.CurrentIsDir())
                {
                    _current = new DirectoryInfo(absolute);
                }
                else
                {
                    _current = new FileInfo(absolute);
                }
            }
            else
            {
                _current = null;
            }

            return hasNext;
        }

        public void Reset()
        {
            _directory.ListDirEnd();
            _directory.ListDirBegin(_skipNavigational, _skipHidden);
        }

        public void Dispose()
        {
            _directory.ListDirEnd();
            _directory.Dispose();
        }
    }
}
