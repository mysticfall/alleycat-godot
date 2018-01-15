using System.Diagnostics;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Event
{
    public abstract class EventTracker<T> : Node where T : Node
    {
        [NotNull]
        protected T Parent
        {
            get
            {
                Debug.Assert(_parent != null, "Node is not attached to parent yet.");

                return _parent;
            }
        }

        private T _parent;

        public override void _Ready()
        {
            base._Ready();

            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
        }

        public override void _EnterTree()
        {
            base._EnterTree();

            var parent = GetParent() as T;

            Debug.Assert(parent != null,
                $"Invalid parent type: '{GetParent()?.GetType()}', expected '{typeof(T)}'.");

            _parent = parent;

            Connect(parent);
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            if (_parent != null)
            {
                Disconnect(_parent);
            }

            _parent = null;
        }

        protected virtual void Connect([NotNull] T parent)
        {
        }

        protected virtual void Disconnect([NotNull] T parent)
        {
        }
    }
}
