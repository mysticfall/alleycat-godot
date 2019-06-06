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
    public class SeekableAnimator : Animator
    {
        public float Position
        {
            get => _position.Value;
            set => _position.OnNext(value);
        }

        public IObservable<float> OnPositionChange => _position.AsObservable();

        protected string Parameter { get; }

        protected AnimationNodeTimeSeek TimeSeekNode { get; }

        private readonly BehaviorSubject<float> _position;

        public SeekableAnimator(
            string key,
            string parameter,
            AnimationNodeTimeSeek timeSeekNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) : base(key, animationNode, context)
        {
            Ensure.That(parameter, nameof(parameter)).IsNotNull();
            Ensure.That(timeSeekNode, nameof(timeSeekNode)).IsNotNull();

            Parameter = parameter;
            TimeSeekNode = timeSeekNode;

            var current = (float?) context.AnimationTree.Get(parameter) ?? 0f;

            _position = CreateSubject(current);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnPositionChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Context.AnimationTree.Set(Parameter, v), this);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                OnPositionChange
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => this.LogTrace("Changed seek position: {}.", v), this);
            }
        }

        public new static Option<SeekableAnimator> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));

            var seekerKey = name + " Seek";

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            return (
                from timeSeek in parent.FindAnimationNode<AnimationNodeTimeSeek>(seekerKey)
                from animation in parent.FindAnimationNode<AnimationNodeAnimation>(name)
                select (timeSeek, animation)).Map(t =>
            {
                var key = string.Join(":", parent.Key, name);
                var parameter = string.Join("/",
                    new[] {"parameters", parent.Key, seekerKey, "seek_position"}.Where(v => v.Length > 0));

                return new SeekableAnimator(key, parameter, t.timeSeek, t.animation, context);
            });
        }
    }

    public static class SeekerExtensions
    {
        public static Option<SeekableAnimator> FindSeekableAnimator(this IAnimationGraph graph, string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));

            return graph.FindDescendantControl<SeekableAnimator>(path);
        }
    }
}
