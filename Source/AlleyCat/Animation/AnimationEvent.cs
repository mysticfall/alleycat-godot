using EnsureThat;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    public struct AnimationEvent
    {
        public string Name { get; }

        public Option<object> Argument { get; }

        public IAnimationManager Source { get; }

        public AnimationEvent(string name, IAnimationManager source) : this(name, None, source)
        {
        }

        public AnimationEvent(string name, Option<object> argument, IAnimationManager source)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Name = name;
            Argument = argument;
            Source = source;
        }
    }
}
