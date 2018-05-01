using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class BoneMorph : Morph<float, BoneMorphDefinition>
    {
        public Skeleton Skeleton { get; }

        public IAnimationManager AnimationManager { get; }

        protected IEnumerable<int> BoneIndexes { get; }

        private readonly IDisposable _disposable;

        public BoneMorph(
            [NotNull] Skeleton skeleton,
            [NotNull] IAnimationManager manager,
            [NotNull] BoneMorphDefinition definition) : base(definition)
        {
            Ensure.Any.IsNotNull(skeleton, nameof(skeleton));
            Ensure.Any.IsNotNull(manager, nameof(manager));

            Skeleton = skeleton;
            AnimationManager = manager;

            BoneIndexes = definition.Bones.Select(FindBone).ToList();

            _disposable = AnimationManager.OnAdvance.Subscribe(_ => Apply());
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

        protected override void Dispose(bool disposing)
        {
            _disposable?.Dispose();

            base.Dispose(disposing);
        }
    }
}
