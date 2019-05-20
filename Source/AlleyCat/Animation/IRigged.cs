using System;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IRigged : IAnimatable, ITransformable
    {
        Skeleton Skeleton { get; }

        Map<string, SkeletonIK> IKChains { get; }
    }

    public static class RiggedExtensions
    {
        public static SkeletonIK StartIK(
            this IRigged rig,
            string name,
            Transform target,
            float amount = 1f)
        {
            Ensure.That(rig, nameof(rig)).IsNotNull();
            Ensure.That(name, nameof(name)).IsNotNull();

            var chain = rig.IKChains[name];

            chain.Target = target;
            chain.Interpolation = Mathf.Clamp(amount, 0, 1);

            chain.Start();

            return chain;
        }

        public static void StopIK(this IRigged rig, string name)
        {
            Ensure.That(rig, nameof(rig)).IsNotNull();

            rig.IKChains.Find(name).Match(
                chain =>
                {
                    chain.Interpolation = 0f;
                    chain.Stop();
                },
                () => throw new ArgumentOutOfRangeException(
                    nameof(name), $"No IKChain exists with the name: '{name}'.")
            );
        }
    }
}
