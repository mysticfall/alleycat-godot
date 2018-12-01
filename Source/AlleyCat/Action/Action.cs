using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Logging;
using EnsureThat;
using Microsoft.Extensions.Logging;

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

        protected Action(
            string key,
            string displayName,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();

            Key = key;
            DisplayName = displayName;

            _active = new BehaviorSubject<bool>(active).DisposeWith(this);
        }

        public void Execute(IActionContext context)
        {
            var allowed = AllowedFor(context);

            if (Active && Valid && allowed)
            {
                this.LogDebug("Executing action with context: '{}'.", context);

                DoExecute(context);
            }
            else
            {
                this.LogDebug(
                    "Not executing action: Active = {}, Valid = {}, Allowed = {}.",
                    Active,
                    Valid,
                    allowed);
            }
        }

        protected abstract void DoExecute(IActionContext context);

        public abstract bool AllowedFor(IActionContext context);

        public bool AllowedFor(object context) => context is IActionContext ac && AllowedFor(ac);
    }
}
