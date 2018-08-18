using System.Collections.Generic;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IRigged : IAnimatable, ITransformable
    {
        Skeleton Skeleton { get; }

        IReadOnlyDictionary<string, SkeletonIK> IKChains { get; }
    }

    public static class RiggedExtensions
    {
        public static SkeletonIK StartIK(
            [NotNull] this IRigged rig,
            [NotNull] string name,
            Transform target,
            float amount = 1f)
        {
            Ensure.Any.IsNotNull(rig, nameof(rig));
            Ensure.Any.IsNotNull(name, nameof(name));

            Ensure.Comparable.IsGte(amount, 0f, nameof(amount));
            Ensure.Comparable.IsLte(amount, 1f, nameof(amount));

            var chain = rig.IKChains[name];

            chain.Target = target;
            chain.Interpolation = amount;

            chain.Start();

            return chain;
        }

        public static void StopIK([NotNull] this IRigged rig, [NotNull] string name)
        {
            Ensure.Any.IsNotNull(rig, nameof(rig));
            Ensure.Any.IsNotNull(name, nameof(name));

            rig.IKChains.TryGetValue(name, out var chain);

            chain?.SetInterpolation(0f);
            chain?.Stop();
        }
    }
}
