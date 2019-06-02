using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Control
{
    public class ActionTapInput : AxisInput
    {
        public string Action
        {
            get => _action;
            set
            {
                Ensure.That(value, nameof(value)).IsNotNullOrWhiteSpace();

                _action = value;
            }
        }

        public IEnumerable<string> Actions { get; }

        public int MaximumTapsPerSecond
        {
            get => _maximumTapsPerSecond;
            set => _maximumTapsPerSecond = Mathf.Clamp(value, 1, 10);
        }

        public float TapCountingWindow
        {
            get => _tapCountingWindow;
            set => _tapCountingWindow = Mathf.Clamp(value, 0.1f, 5f);
        }

        public int TapCountingResolution
        {
            get => _tapCountingResolution;
            set => _tapCountingResolution = Mathf.Clamp(value, 1, 100);
        }

        private string _action;

        private int _maximumTapsPerSecond = 5;

        private float _tapCountingWindow = 0.5f;

        private int _tapCountingResolution = 20;

        public ActionTapInput(
            string key,
            string action,
            IInputSource source,
            ITimeSource timeSource,
            bool active,
            ILoggerFactory loggerFactory) : base(key, source, timeSource, active, loggerFactory)
        {
            Ensure.That(action, nameof(action)).IsNotEmptyOrWhitespace();

            Action = action;
            Actions = Seq1(action);
        }

        protected override IObservable<float> CreateRawObservable()
        {
            var minRate = 1f / MaximumTapsPerSecond;

            var taps = Source.OnInput
                .Where(e => e.IsActionReleased(Action))
                .SelectMany(_ => Observable
                    .Interval(TimeSpan.FromMilliseconds(minRate), TimeSource.Scheduler)
                    .Take(TapCountingResolution))
                .Select(_ => 1)
                .Buffer(
                    TimeSpan.FromSeconds(TapCountingWindow),
                    TimeSpan.FromSeconds(minRate / TapCountingResolution),
                    TimeSource.Scheduler)
                .Select(v => v.Count / (float) MaximumTapsPerSecond / (float) TapCountingResolution)
                .Select(v => Mathf.Min(v, 1));

            return taps.DistinctUntilChanged();
        }

        public override bool ConflictsWith(IInput other) =>
            other != this && 
            other is IActionInput input && 
            Actions.Any(input.Actions.Contains);
    }
}
