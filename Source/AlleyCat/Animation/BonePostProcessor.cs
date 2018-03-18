using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public abstract class BonePostProcessor : IAnimationPostProcessor
    {
        [NotNull]
        public Skeleton Skeleton { get; }

        protected BonePostProcessor([NotNull] Skeleton skeleton)
        {
            Ensure.Any.IsNotNull(skeleton, nameof(skeleton));

            Skeleton = skeleton;
        }

        public abstract void BeforeFrame(PostProcessingAnimationPlayer player);

        public abstract void AfterFrame(PostProcessingAnimationPlayer player, float delta);
    }
}
