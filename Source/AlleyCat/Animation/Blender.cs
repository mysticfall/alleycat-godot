using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using static LanguageExt.Prelude;
using Array = Godot.Collections.Array;

namespace AlleyCat.Animation
{
    public class Blender : AnimationControl, IAnimator
    {
        public Option<Godot.Animation> Animation
        {
            get => _animation.Value;
            set => _animation.OnNext(value);
        }

        public float Amount
        {
            get => _amount.Value;
            set => _amount.OnNext(Mathf.Clamp(value, 0, 1));
        }

        public float TimeScale
        {
            get => _timeScale.Value;
            set => _timeScale.OnNext(Mathf.Clamp(value, 0, 1));
        }

        public IObservable<Option<Godot.Animation>> OnAnimationChange => _animation.AsObservable();

        public IObservable<float> OnAmountChange => _amount.AsObservable();

        public IObservable<float> OnTimeScaleChange => _timeScale.AsObservable();

        protected string BlendAmountParameter { get; }

        protected Option<string> TimeScaleParameter { get; }

        protected AnimationNodeBlend2 BlenderNode { get; }

        protected AnimationNodeAnimation AnimationNode { get; }

        private readonly BehaviorSubject<Option<Godot.Animation>> _animation;

        private readonly BehaviorSubject<float> _amount;

        private readonly BehaviorSubject<float> _timeScale;

        public Blender(
            string key,
            string blendAmountParameter,
            AnimationNodeBlend2 blenderNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) :
            this(key, blendAmountParameter, None, blenderNode, animationNode, context)
        {
        }

        public Blender(
            string key,
            string blendAmountParameter,
            Option<string> timeScaleParameter,
            AnimationNodeBlend2 blenderNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) : base(key, context)
        {
            Ensure.That(blendAmountParameter, nameof(blendAmountParameter)).IsNotNull();
            Ensure.That(blenderNode, nameof(blenderNode)).IsNotNull();
            Ensure.That(animationNode, nameof(animationNode)).IsNotNull();

            BlendAmountParameter = blendAmountParameter;
            TimeScaleParameter = timeScaleParameter;

            BlenderNode = blenderNode;
            AnimationNode = animationNode;

            var currentAnim = AnimationNode.Animation.TrimToOption().Bind(context.Player.FindAnimation);
            var currentAmount = (float) context.AnimationTree.Get(blendAmountParameter);
            var currentSpeed = timeScaleParameter
                .Map(context.AnimationTree.Get)
                .OfType<float>()
                .HeadOrNone()
                .IfNone(1f);

            _animation = CreateSubject(currentAnim);
            _amount = CreateSubject(currentAmount);
            _timeScale = CreateSubject(currentSpeed);
        }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            OnAnimationChange
                .Select(a => a.Map(Context.Player.AddAnimation).ValueUnsafe())
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(AnimationNode.SetAnimation, this);

            OnAnimationChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(animation =>
                {
                    var filters = new Array();

                    animation.Bind(FindTransformTracks).Iter(p => filters.Add(p));

                    if (animation.IsNone) Amount = 0;

                    BlenderNode.Filters = filters;
                    BlenderNode.FilterEnabled = filters.Count > 0;
                }, this);

            OnAmountChange
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(v => Context.AnimationTree.Set(BlendAmountParameter, v), this);

            TimeScaleParameter.Iter(param =>
            {
                OnTimeScaleChange
                    .TakeUntil(Disposed.Where(identity))
                    .Subscribe(v => Context.AnimationTree.Set(param, v), this);
            });
        }

        public void Blend(
            Godot.Animation animation,
            float timeScale = 1f,
            float amount = 1f,
            float transition = 0f)
        {
            Ensure.That(animation, nameof(animation)).IsNotNull();

            Animation = animation;
            TimeScale = Mathf.Clamp(timeScale, 0, 1);

            this.LogDebug("Blending animation: '{}' (timeScale = {}, amount = {}, transition = {}).",
                animation, timeScale, amount, transition);

            var clampedAmount = Mathf.Clamp(amount, 0, 1);
            var clampedTransition = Mathf.Max(transition, 0);

            if (transition > 0)
            {
                var elapsed = Context.OnAdvance
                    .Scan(0f, (total, delta) => total + delta);

                var done = elapsed
                    .Where(v => v >= clampedTransition)
                    .Take(1);

                elapsed
                    .Select(v => clampedTransition > 0 ? v / clampedTransition : 0f)
                    .Select(v => Mathf.Min(v * clampedAmount, 1f))
                    .TakeUntil(done)
                    .Subscribe(v => Amount = v, () => Amount = clampedAmount, this);
            }
            else
            {
                Amount = clampedAmount;
            }
        }

        public void Unblend(float transition = 0f)
        {
            this.LogDebug("Unblending animation: (transition = {}).", transition);

            if (transition > 0)
            {
                var elapsed = Context.OnAdvance
                    .Scan(0f, (total, delta) => total + delta);

                var done = elapsed
                    .Where(v => v >= transition)
                    .Take(1);

                elapsed
                    .Select(v => Mathf.Max((1f - v / transition) * Amount, 0f))
                    .TakeUntil(done)
                    .Subscribe(v => Amount = v, () => Amount = 0f, this);
            }
            else
            {
                Amount = 0f;
            }
        }

        public static Option<Blender> TryCreate(
            string name,
            IAnimationGraph parent,
            AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            var animationNodeKey = name + " Animation";

            var timeScaleNodeKey = name + " Speed";
            var timeScaleNode = parent.FindAnimationNode<AnimationNodeTimeScale>(timeScaleNodeKey);

            return (
                from blender in parent.FindAnimationNode<AnimationNodeBlend2>(name)
                from animation in parent.FindAnimationNode<AnimationNodeAnimation>(animationNodeKey)
                select (blender, animation)).Map(t =>
            {
                var key = string.Join(":", parent.Key, name);
                var parameterBlendAmount = string.Join("/",
                    new[] {"parameters", parent.Key, name, "blend_amount"}.Where(v => v.Length > 0));

                var parameterTimeScale = timeScaleNode == null
                    ? null
                    : string.Join("/",
                        new[] {"parameters", parent.Key, timeScaleNodeKey, "scale"}.Where(v => v.Length > 0));

                return new Blender(
                    key, parameterBlendAmount, parameterTimeScale, t.blender, t.animation, context);
            });
        }
    }

    public static class BlenderExtensions
    {
        public static Option<Blender> FindBlender(this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();

            return graph.FindDescendantControl<Blender>(path);
        }
    }
}
