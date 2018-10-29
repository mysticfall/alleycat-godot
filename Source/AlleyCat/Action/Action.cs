using System;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using static LanguageExt.Prelude;

namespace AlleyCat.Action
{
    [Singleton(typeof(IAction))]
    public abstract class Action : AutowiredNode, IAction
    {
        public string Key => _key.TrimToOption().IfNone(GetName);

        public virtual string DisplayName => Optional(_displayName).Map(Tr).IfNone(() => Key);

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        [Export] private string _key;

        [Export] private string _displayName;

        private readonly ReactiveProperty<bool> _active;

        protected Action()
        {
            _active = new ReactiveProperty<bool>(true).AddTo(this);
        }

        public void Execute(IActionContext context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            if (Active && Valid && AllowedFor(context))
            {
                DoExecute(context);
            }
        }

        protected abstract void DoExecute(IActionContext context);

        public abstract bool AllowedFor(IActionContext context);

        public bool AllowedFor(object context)
        {
            Ensure.That(context, nameof(context)).IsNotNull();

            return context is IActionContext ac && AllowedFor(ac);
        }
    }
}
