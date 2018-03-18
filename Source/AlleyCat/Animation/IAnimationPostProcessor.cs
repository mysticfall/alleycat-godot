using JetBrains.Annotations;

namespace AlleyCat.Animation
{
    public interface IAnimationPostProcessor
    {
        void BeforeFrame([NotNull] PostProcessingAnimationPlayer player);

        void AfterFrame([NotNull] PostProcessingAnimationPlayer player, float delta);
    }
}
