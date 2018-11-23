using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
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
            string blendAmountParameter,
            AnimationNodeBlend2 blenderNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) :
            this(blendAmountParameter, null, blenderNode, animationNode, context)
        {
        }

        public Blender(
            string blendAmountParameter,
            Option<string> timeScaleParameter,
            AnimationNodeBlend2 blenderNode,
            AnimationNodeAnimation animationNode,
            AnimationGraphContext context) : base(context)
        {
            Ensure.That(blendAmountParameter, nameof(blendAmountParameter)).IsNotNull();
            Ensure.That(blenderNode, nameof(blenderNode)).IsNotNull();
            Ensure.That(animationNode, nameof(animationNode)).IsNotNull();

            BlendAmountParameter = blendAmountParameter;
            TimeScaleParameter = timeScaleParameter;

            BlenderNode = blenderNode;
            AnimationNode = animationNode;

            var current = AnimationNode.Animation.TrimToOption().Bind(context.Player.FindAnimation);

            _animation = new BehaviorSubject<Option<Godot.Animation>>(current).DisposeWith(this);

            _animation
                .Select(a => a.Map(context.Player.AddAnimation).ValueUnsafe())
                .Subscribe(AnimationNode.SetAnimation)
                .DisposeWith(this);

            _animation
                .Subscribe(animation =>
                {
                    var filters = new Array();

                    animation.Bind(FindTransformTracks).Iter(filters.Add);

                    if (animation.IsNone) Amount = 0;

                    BlenderNode.Filters = filters;
                    BlenderNode.FilterEnabled = filters.Any();
                })
                .DisposeWith(this);

            var currentAmount = (float) context.AnimationTree.Get(blendAmountParameter);

            _amount = new BehaviorSubject<float>(currentAmount).DisposeWith(this);

            _amount
                .Subscribe(v => context.AnimationTree.Set(blendAmountParameter, v))
                .DisposeWith(this);

            var currentSpeed = timeScaleParameter
                .Map(context.AnimationTree.Get).OfType<float>().HeadOrNone().IfNone(1f);

            _timeScale = new BehaviorSubject<float>(currentSpeed).DisposeWith(this);

            timeScaleParameter.Iter(param =>
            {
                _timeScale
                    .Subscribe(v => context.AnimationTree.Set(param, v))
                    .DisposeWith(this);
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

            var clampedAmount = Mathf.Clamp(amount, 0, 1);
            var clampedTransition = Mathf.Min(transition, 0);

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
                    .Subscribe(v => Amount = v, () => Amount = clampedAmount);
            }
            else
            {
                Amount = clampedAmount;
            }
        }

        public void Unblend(float transition = 0f)
        {
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
                    .Subscribe(v => Amount = v, () => Amount = 0f);
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
            Ensure.Any.IsNotNull(context, nameof(context));

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            var animationNodeKey = name + " Animation";

            var timeScaleNodeKey = name + " Speed";
            var timeScaleNode = parent.FindAnimationNode<AnimationNodeTimeScale>(timeScaleNodeKey);

            return (
                from blender in parent.FindAnimationNode<AnimationNodeBlend2>(name)
                from animation in parent.FindAnimationNode<AnimationNodeAnimation>(animationNodeKey)
                select (blender, animation)).Map(t =>
            {
                var parameterBlendAmount = string.Join("/",
                    new[] {"parameters", parent.Path, name, "blend_amount"}.Where(v => v.Length > 0));

                var parameterTimeScale = timeScaleNode == null
                    ? null
                    : string.Join("/",
                        new[] {"parameters", parent.Path, timeScaleNodeKey, "scale"}.Where(v => v.Length > 0));

                return new Blender(
                    parameterBlendAmount, parameterTimeScale, t.blender, t.animation, context);
            });
        }
    }

    public static class BlenderExtensions
    {
        public static Option<Blender> FindBlender(this IAnimationGraph graph, string path)
        {
            Ensure.That(graph, nameof(graph)).IsNotNull();
            Ensure.That(path, nameof(path)).IsNotNull();

            return graph.FindDescendantControl<Blender>(path);
        }
    }
}
