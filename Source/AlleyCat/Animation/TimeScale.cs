using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public class TimeScale : AnimationControl
    {
        public float Speed
        {
            get => _speed.Value;
            set => _speed.OnNext(value);
        }

        public IObservable<float> OnSpeedChange => _speed.AsObservable();

        protected string Parameter { get; }

        private readonly BehaviorSubject<float> _speed;

        public TimeScale(string key, string parameter, AnimationGraphContext context) : base(key, context)
        {
            Ensure.That(parameter, nameof(parameter)).IsNotNull();

            Parameter = parameter;

            var current = (float) context.AnimationTree.Get(parameter);

            _speed = CreateSubject(current);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnSpeedChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Context.AnimationTree.Set(Parameter, v), this);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                OnSpeedChange
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => this.LogTrace("Changed time scale: {}.", v), this);
            }
        }

        public static Option<TimeScale> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(parent, nameof(parent)).IsNotNull();

            if (parent.FindAnimationNode<AnimationNodeTimeScale>(name).IsNone) return None;

            var parameter = string.Join("/",
                new[] {"parameters", parent.Key, name, "scale"}.Where(v => v.Length > 0));

            return new TimeScale(string.Join(":", parent.Key, name), parameter, context);
        }
    }

    public static class TimeScalextensions
    {
        public static Option<TimeScale> FindTimeScale(this IAnimationGraph graph, string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));

            return graph.FindDescendantControl<TimeScale>(path);
        }
    }
}
