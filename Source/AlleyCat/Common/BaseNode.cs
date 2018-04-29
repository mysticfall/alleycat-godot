using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public class BaseNode : Node, IDisposableCollector
    {
        private IList<IDisposable> _disposables;

        public BaseNode()
        {
        }

        public BaseNode([NotNull] string name)
        {
            Ensure.Any.IsNotNull(name, nameof(name));

            Name = name;
        }

        public void Collect(IDisposable disposable)
        {
            Ensure.Any.IsNotNull(disposable, nameof(disposable));

            if (_disposables == null)
            {
                _disposables = new List<IDisposable>();
            }
            else if (_disposables.Contains(disposable))
            {
                return;
            }

            _disposables.Add(disposable);
        }

        protected override void Dispose(bool disposing)
        {
            _disposables?.Where(d => d != null).Reverse().ToList().ForEach(d => d.Dispose());
            _disposables = null;

            base.Dispose(disposing);
        }
    }
}
