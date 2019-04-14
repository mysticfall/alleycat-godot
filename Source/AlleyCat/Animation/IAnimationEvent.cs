using EnsureThat;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimationEvent
    {
        string Name { get; }

        IAnimationManager Source { get; }
    }

    public struct TriggerEvent : IAnimationEvent
    {
        public string Name { get; }

        public Option<object> Argument { get; }

        public IAnimationManager Source { get; }

        public TriggerEvent(string name, Option<object> argument, IAnimationManager source)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Name = name;
            Argument = argument;
            Source = source;
        }
    }

    public struct ValueChangeEvent : IAnimationEvent
    {
        public string Name { get; }

        public float Value { get; }

        public IAnimationManager Source { get; }

        public ValueChangeEvent(string name, float value, IAnimationManager source)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Name = name;
            Value = value;
            Source = source;
        }
    }
}
