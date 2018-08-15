using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationStateManager : IAnimationManager
    {
        AnimationTree AnimationTree { get; }

        AnimationNodeStateMachine States { get; }

        IReadOnlyDictionary<string, AnimationBlender> Blenders { get; }
    }

    public static class AnimationStateManagerExtensions
    {
        public static void Blend(
            [NotNull] this IAnimationStateManager manager,
            [NotNull] string position,
            [NotNull] Godot.Animation animation,
            float amount = 1f,
            float timeScale = 1f,
            float transition = 0f)
        {
            Ensure.Any.IsNotNull(manager, nameof(manager));
            Ensure.Any.IsNotNull(position, nameof(position));
            Ensure.Any.IsNotNull(animation, nameof(animation));

            manager.Blenders.TryGetValue(position, out var blender);

            if (blender == null || !(amount > 0)) return;

            blender.Animation = animation;
            blender.TimeScale = timeScale;

            if (transition > 0)
            {
                var elapsed = manager.OnAdvance
                    .Scan(0f, (total, delta) => total + delta);

                var done = elapsed
                    .Where(v => v >= transition)
                    .Take(1);

                elapsed
                    .Select(v => Mathf.Min(v / transition * amount, 1f))
                    .TakeUntil(done)
                    .Subscribe(v => blender.Amount = v, () => blender.Amount = amount);
            }
            else
            {
                blender.Amount = amount;
            }
        }

        public static void Unblend(
            [NotNull] this IAnimationStateManager manager,
            [NotNull] string position,
            float transition = 0f)
        {
            Ensure.Any.IsNotNull(manager, nameof(manager));
            Ensure.Any.IsNotNull(position, nameof(position));

            manager.Blenders.TryGetValue(position, out var blender);

            if (blender == null) return;

            if (transition > 0)
            {
                var amount = blender.Amount;
                var elapsed = manager.OnAdvance
                    .Scan(0f, (total, delta) => total + delta);

                var done = elapsed
                    .Where(v => v >= transition)
                    .Take(1);

                elapsed
                    .Select(v => Mathf.Max((1f - v / transition) * amount, 0f))
                    .TakeUntil(done)
                    .Subscribe(v => blender.Amount = v, () => blender.Amount = 0f);
            }
            else
            {
                blender.Amount = 0f;
            }
        }
    }
}
