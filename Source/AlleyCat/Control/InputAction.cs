using System;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Common;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Control
{
    public abstract class InputAction : Action.Action
    {
        public ITriggerInput Input { get; }

        protected abstract Option<IActionContext> CreateActionContext();

        protected InputAction(
            string key,
            string displayName,
            ITriggerInput input,
            bool active,
            ILogger logger) : base(key, displayName, active, logger)
        {
            Ensure.That(input, nameof(input)).IsNotNull();

            Input = input;
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            Input
                .Where(v => v && Active)
                .Select(_ => CreateActionContext())
                .Where(c => c.Exists(AllowedFor))
                .Subscribe(c => c.Iter(Execute))
                .DisposeWith(this);
        }
    }
}
