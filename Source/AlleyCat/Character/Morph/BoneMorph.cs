using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class BoneMorph : Morph<float, BoneMorphDefinition>, IAnimationPostProcessor
    {
        public Skeleton Skeleton { get; }

        public PostProcessingAnimationPlayer AnimationPlayer { get; }

        protected IEnumerable<int> BoneIndexes { get; }

        public BoneMorph(
            [NotNull] Skeleton skeleton,
            [NotNull] PostProcessingAnimationPlayer player,
            [NotNull] BoneMorphDefinition definition) : base(definition)
        {
            Ensure.Any.IsNotNull(skeleton, nameof(skeleton));
            Ensure.Any.IsNotNull(player, nameof(player));

            Skeleton = skeleton;
            AnimationPlayer = player;

            BoneIndexes = definition.Bones.Select(FindBone).ToList();

            AnimationPlayer.Processors.Add(this);
        }

        private int FindBone(string name)
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

        public void BeforeFrame(PostProcessingAnimationPlayer player)
        {
        }

        public void AfterFrame(PostProcessingAnimationPlayer player, float delta)
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

        protected override void Dispose(bool disposing)
        {
            AnimationPlayer?.Processors.Remove(this);

            base.Dispose(disposing);
        }
    }
}
