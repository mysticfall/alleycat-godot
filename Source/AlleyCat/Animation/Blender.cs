using System;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Array = Godot.Collections.Array;

namespace AlleyCat.Animation
{
    public class Blender : AnimationControl
    {
        [CanBeNull]
        public Godot.Animation Animation
        {
            get => _animation.Value;
            set => _animation.Value = value;
        }

        public float Amount
        {
            get => _amount.Value;
            set => _amount.Value = value;
        }

        public float TimeScale
        {
            get => _timeScale.Value;
            set => _timeScale.Value = value;
        }

        public IObservable<Godot.Animation> OnAnimationChange => _animation;

        public IObservable<float> OnAmountChange => _amount;

        public IObservable<float> OnTimeScaleChange => _timeScale;

        protected string BlendAmountParameter { get; }

        protected string TimeScaleParameter { get; }

        protected AnimationNodeBlend2 BlenderNode { get; }

        protected AnimationNodeAnimation AnimationNode { get; }

        private readonly ReactiveProperty<Godot.Animation> _animation;

        private readonly ReactiveProperty<float> _amount;

        private readonly ReactiveProperty<float> _timeScale;

        public Blender(
            [NotNull] string blendAmountParameter,
            [NotNull] AnimationNodeBlend2 blenderNode,
            [NotNull] AnimationNodeAnimation animationNode,
            AnimationGraphContext context) :
            this(blendAmountParameter, null, blenderNode, animationNode, context)
        {
        }

        public Blender(
            [NotNull] string blendAmountParameter,
            [CanBeNull] string timeScaleParameter,
            [NotNull] AnimationNodeBlend2 blenderNode,
            [NotNull] AnimationNodeAnimation animationNode,
            AnimationGraphContext context) : base(context)
        {
            Ensure.Any.IsNotNull(blendAmountParameter, nameof(blendAmountParameter));
            Ensure.Any.IsNotNull(blenderNode, nameof(blenderNode));
            Ensure.Any.IsNotNull(animationNode, nameof(animationNode));

            BlendAmountParameter = blendAmountParameter;
            TimeScaleParameter = timeScaleParameter;

            BlenderNode = blenderNode;
            AnimationNode = animationNode;

            var currentAnim = AnimationNode.Animation;

            _animation = new ReactiveProperty<Godot.Animation>(
                string.IsNullOrEmpty(currentAnim) ? null : context.Player.GetAnimation(currentAnim));

            _animation.Subscribe(v =>
            {
                AnimationNode.Animation = v?.GetKey();

                var filters = new Array();

                if (v != null)
                {
                    FindTransformTracks(v).ToList().ForEach(filters.Add);
                }
                else
                {
                    Amount = 0;
                }

                BlenderNode.Filters = filters;
                BlenderNode.FilterEnabled = filters.Any();
            });

            var currentAmount = (float) context.AnimationTree.Get(blendAmountParameter);

            _amount = new ReactiveProperty<float>(currentAmount);
            _amount.Subscribe(v => context.AnimationTree.Set(blendAmountParameter, v));

            var currentSpeed = timeScaleParameter != null ? (float) context.AnimationTree.Get(timeScaleParameter) : 1f;

            _timeScale = new ReactiveProperty<float>(currentSpeed);
            _timeScale
                .Where(_ => timeScaleParameter != null)
                .Subscribe(v => context.AnimationTree.Set(timeScaleParameter, v));
        }

        public void Blend(
            [NotNull] Godot.Animation animation,
            float timeScale = 1f,
            float amount = 1f,
            float transition = 0f)
        {
            Ensure.Any.IsNotNull(animation, nameof(animation));

            Ensure.Comparable.IsGte(timeScale, 0f, nameof(timeScale));
            Ensure.Comparable.IsLte(timeScale, 1f, nameof(timeScale));
            Ensure.Comparable.IsGte(amount, 0f, nameof(amount));
            Ensure.Comparable.IsLte(amount, 1f, nameof(amount));
            Ensure.Comparable.IsGte(transition, 0f, nameof(transition));

            Animation = animation;
            TimeScale = timeScale;

            if (transition > 0)
            {
                var elapsed = Context.OnAdvance
                    .Scan(0f, (total, delta) => total + delta);

                var done = elapsed
                    .Where(v => v >= transition)
                    .Take(1);

                elapsed
                    .Select(v => Mathf.Min(v / transition * amount, 1f))
                    .TakeUntil(done)
                    .Subscribe(v => Amount = v, () => Amount = amount);
            }
            else
            {
                Amount = amount;
            }
        }

        public void Unblend(float transition = 0f)
        {
            Ensure.Comparable.IsGte(transition, 0f, nameof(transition));

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

        public override void Dispose()
        {
            _animation?.Dispose();
            _amount?.Dispose();
            _timeScale?.Dispose();
        }

        public static Blender Create(
            [NotNull] string name,
            [NotNull] IAnimationGraph parent,
            [NotNull] AnimationGraphContext context)
        {
            Ensure.Any.IsNotNull(name, nameof(name));
            Ensure.Any.IsNotNull(parent, nameof(parent));
            Ensure.Any.IsNotNull(context, nameof(context));

            var blenderNode = parent.GetAnimationNode(name) as AnimationNodeBlend2;

            //TODO Resolve in an automatic fashion when it becomes possible to manipulate node connections from code.
            var animationNodeKey = name + " Animation";
            var animationNode = parent.GetAnimationNode(animationNodeKey) as AnimationNodeAnimation;

            var timeScaleNodeKey = name + " Speed";
            var timeScaleNode = parent.GetAnimationNode(timeScaleNodeKey) as AnimationNodeTimeScale;

            if (blenderNode == null || animationNode == null) return null;

            var parameterBlendAmount = string.Join("/",
                new[] {"parameters", parent.Path, name, "blend_amount"}.Where(v => v.Length > 0));

            var parameterTimeScale = timeScaleNode == null
                ? null
                : string.Join("/",
                    new[] {"parameters", parent.Path, timeScaleNodeKey, "scale"}.Where(v => v.Length > 0));

            return new Blender(parameterBlendAmount, parameterTimeScale, blenderNode, animationNode, context);
        }
    }

    public static class BlenderExtensions
    {
        [CanBeNull]
        public static Blender GetBlender(
            [NotNull] this IAnimationGraph graph, [NotNull] string path)
        {
            Ensure.Any.IsNotNull(graph, nameof(graph));
            Ensure.Any.IsNotNull(path, nameof(path));

            return graph.GetDescendantControl(path) as Blender;
        }
    }
}
