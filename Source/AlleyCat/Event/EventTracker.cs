using System.Diagnostics;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Event
{
    public abstract class EventTracker<T> : Node where T : Node
    {
        protected Option<T> Parent { get; private set; } = None;

        public override void _EnterTree()
        {
            base._EnterTree();

            Parent = Some(GetParent() as T);

            Debug.Assert(Parent.IsSome, "_parent.IsSome");
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            Parent = None;

            Debug.Assert(Parent.IsNone, "_parent.IsNone");
        }

        public override void _Ready()
        {
            base._Ready();

            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            if (what == NotificationPredelete)
            {
                Parent.Iter(Disconnect);
            }
        }

        protected virtual void Disconnect(T parent)
        {
        }
    }
}
