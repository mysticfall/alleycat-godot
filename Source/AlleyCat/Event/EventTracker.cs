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

            var parent = GetParent() as T;

            Debug.Assert(parent != null,
                $"Invalid parent type: '{GetParent()?.GetType()}', expected '{typeof(T)}'.");

            _parent = parent;
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            if (what == NotificationPredelete && _parent != null)
            {
                Disconnect(_parent);
            }
        }

        protected virtual void Disconnect([NotNull] T parent)
        {
        }
    }
}
