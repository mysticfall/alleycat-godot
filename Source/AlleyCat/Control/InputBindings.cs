using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using AlleyCat.Game;
using EnsureThat;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class InputBindings : GameObject, IInputBindings
    {
        public bool Active
        {
            get => _active.Value;
            set => _active.OnNext(value);
        }

        public IObservable<bool> OnActiveStateChange => _active.AsObservable();

        public Map<string, IInput> Inputs { get; }

        private readonly BehaviorSubject<bool> _active;

        public InputBindings(
            IEnumerable<IInput> inputs,
            bool active,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            Ensure.That(inputs, nameof(inputs)).IsNotNull();

            Inputs = inputs.ToMap();

            _active = CreateSubject(active);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnActiveStateChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Inputs.Values.Iter(i => i.Active = v), this);
        }
    }
}
