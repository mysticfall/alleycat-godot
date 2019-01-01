using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Animation;
using AlleyCat.Logging;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class BoneMorph : RangedMorph<BoneMorphDefinition>
    {
        public Skeleton Skeleton { get; }

        public IAnimationManager AnimationManager { get; }

        protected IEnumerable<int> BoneIndexes { get; }

        public BoneMorph(
            Skeleton skeleton,
            IAnimationManager manager,
            BoneMorphDefinition definition,
            ILoggerFactory loggerFactory) : base(definition, loggerFactory)
        {
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(manager, nameof(manager)).IsNotNull();

            Skeleton = skeleton;
            AnimationManager = manager;

            int FindBone(string name)
            {
                var index = Skeleton.FindBone(name);

                if (index == -1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(name),
                        $"The morph '{Definition.Key}' contains a non-existent bone: '{name}'.");
                }

                return index;
            }

            BoneIndexes = definition.Bones.Select(FindBone).Freeze();

            if (!BoneIndexes.Any())
            {
                throw new ArgumentOutOfRangeException(
                    nameof(definition),
                    $"The morph '{Definition.Key}' does not have any target bones.");
            }

            AnimationManager.OnAdvance
                .TakeUntil(Disposed.Where(identity))
                .Subscribe(_ => Apply(), this);
        }

        protected override void Apply(float value)
        {
            var defaultScale = new Vector3(1, 1, 1) * Definition.Default;
            var deltaScale = new Vector3(1, 1, 1) * Value - defaultScale;

            var scale = defaultScale + deltaScale * Definition.Modifier;

            foreach (var index in BoneIndexes)
            {
                var pose = Skeleton.GetBonePose(index);

                Skeleton.SetBonePose(index, pose.Scaled(scale));
            }
        }
    }
}
