using System.Diagnostics;
using System.Linq;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Game
{
    public abstract class DelegateObjectFactory<TObj, TDel> : GameObjectFactory<TObj>
        where TObj : IDelegateObject<TDel>
        where TDel : Node
    {
        private Option<Node> _parent;

        public override void _Notification(int what)
        {
            base._Notification(what);

            switch (what)
            {
                case NotificationParented:
                    var parent = GetParent();

                    Debug.Assert(parent != null, "parent != null");

                    parent.Connect("ready", this, nameof(OnParentReady));
                    parent.AssignDelegate(this);

                    _parent = Some(parent);
                    break;
                case NotificationUnparented:
                    _parent.Iter(DelegateObjectExtensions.ClearDelegate);
                    _parent = None;
                    break;
            }
        }

        public override void _Ready()
        {
        }

        private void OnParentReady()
        {
            base._Ready();
        }

        protected override Validation<string, TObj> CreateService(ILoggerFactory loggerFactory)
        {
            var node = Optional(GetParent()).OfType<TDel>()
                .HeadOrInvalid("The node does not have a suitable parent.");

            return node.Bind(n => CreateService(n, loggerFactory));
        }

        protected abstract Validation<string, TObj> CreateService(TDel node, ILoggerFactory loggerFactory);
    }
}
