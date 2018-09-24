using System;
using AlleyCat.Autowire;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    [Singleton(typeof(IAction))]
    public abstract class Action : AutowiredNode, IAction
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr(_displayName);

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => true;

        [Export, UsedImplicitly] private string _key;

        [Export, UsedImplicitly] private string _displayName;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        public void Execute(IActionContext context)
        {
            Ensure.Any.IsNotNull(context);

            if (Active && Valid && AllowedFor(context))
            {
                DoExecute(context);
            }
        }

        protected abstract void DoExecute(IActionContext context);

        public abstract bool AllowedFor([CanBeNull] IActionContext context);

        public bool AllowedFor(object context) => AllowedFor(context as IActionContext);
    }
}
