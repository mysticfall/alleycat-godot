using System;
using AlleyCat.Autowire;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Action
{
    [Singleton(typeof(IAction))]
    public abstract class Action : AutowiredNode, IAction
    {
        public string Key => _key ?? Name;

        [Export]
        public bool Active
        {
            get => _active.Value;
            set => _active.Value = value;
        }

        public IObservable<bool> OnActiveStateChange => _active;

        public virtual bool Valid => true;

        [Export, UsedImplicitly] private string _key;

        private readonly ReactiveProperty<bool> _active = new ReactiveProperty<bool>(true);

        public void Execute(IActor actor)
        {
            if (Active && Valid && AllowedFor(actor))
            {
                DoExecute(actor);
            }
        }

        protected abstract void DoExecute(IActor actor);

        public abstract bool AllowedFor([CanBeNull] IActor context);

        public bool AllowedFor(object context) => AllowedFor(context as IActor);
    }
}
