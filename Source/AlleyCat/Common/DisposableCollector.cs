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
                    "Node has been detached from parent, or already disposed.");
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

        public override void _EnterTree()
        {
            base._EnterTree();

            _disposables = new List<IDisposable>();
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            DisposeItems();
        }

        public override void Dispose(bool disposing)
        {
            DisposeItems();

            base.Dispose(disposing);
        }

        private void DisposeItems()
        {
            _disposables?.ForEach(d => d.Dispose());
            _disposables = null;
        }
    }
}
