using System;
using System.Collections.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Common
{
    public class DisposableCollector : Node
    {
        private List<IDisposable> _disposables;

        public void Add([NotNull] IDisposable disposable)
        {
            Ensure.Any.IsNotNull(disposable, nameof(disposable));

            if (_disposables == null)
            {
                throw new InvalidOperationException(
                    "DisposableCollector has not been initialized yet.");
            }

            _disposables.Add(disposable);
        }

        public override void _Ready()
        {
            base._Ready();

            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);

            _disposables = new List<IDisposable>();
        }

        public override void Dispose(bool disposing)
        {
            _disposables?.ForEach(d => d.Dispose());
            _disposables = null;

            base.Dispose(disposing);
        }
    }
}
