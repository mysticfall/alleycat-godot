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
using Unit = System.Reactive.Unit;

namespace AlleyCat.Animation
{
    public class Trigger : Animator
    {
        public IObservable<Unit> OnTrigger => _trigger.AsObservable();

        protected string Parameter { get; }

        protected AnimationNodeOneShot OneShotNode { get; }

        private readonly ISubject<Unit> _trigger;

        public Trigger(
            string key,
            string parameter,
            AnimationNodeOneShot oneShotNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) : base(key, animationNode, context)
        {
            Ensure.Any.IsNotNull(parameter, nameof(parameter));
            Ensure.Any.IsNotNull(oneShotNode, nameof(oneShotNode));

            Parameter = parameter;
            OneShotNode = oneShotNode;

            _trigger = CreateSubject<Unit>();
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnTrigger
                .TakeUntil(Disposed.Where(identity))
                .Where(_ => Animation.IsSome)
                .Subscribe(_ => Context.AnimationTree.Set(Parameter, true), this);

            if (Logger.IsEnabled(LogLevel.Trace))
            {
                OnTrigger
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(_ => this.LogTrace("Animation was triggered."), this);
            }
        }

        public void Fire() => _trigger.OnNext(Unit.Default);

        public new static Option<Trigger> TryCreate(
            string name, IAnimationGraph parent, AnimationGraphContext context)
        {
            Ensure.That(parent, nameof(parent)).IsNotNull();

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            var animationNodeKey = name + " Animation";

            return (
                from oneShot in parent.FindAnimationNode<AnimationNodeOneShot>(name)
                from animation in parent.FindAnimationNode<AnimationNodeAnimation>(animationNodeKey)
                select (oneShot, animation)).Map(t =>
            {
                var key = string.Join(":", parent.Key, name);
                var parameter = string.Join("/",
                    new[] {"parameters", parent.Key, name, "scale"}.Where(v => v.Length > 0));

                return new Trigger(key, parameter, t.oneShot, t.animation, context);
            });
        }
    }

    public static class TriggerExtensions
    {
        public static Option<Trigger> FindTrigger(this IAnimationGraph graph, string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));

            return graph.FindDescendantControl<Trigger>(path);
        }
    }
}
