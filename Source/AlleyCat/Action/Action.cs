using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;

namespace AlleyCat.Action
{
    public abstract class Action : GameObject, IAction
    {
        public string Key { get; }

        public virtual string DisplayName { get; }

        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        private readonly BehaviorSubject<bool> _active;

        protected Action(string key, string displayName, bool active = true)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            Key = key;
            DisplayName = displayName;
         
            _active = new BehaviorSubject<bool>(active).AddTo(this);
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
