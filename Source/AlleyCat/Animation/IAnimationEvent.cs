using System;
using System.Collections.Generic;
using EnsureThat;
using LanguageExt;

namespace AlleyCat.Animation
{
    public interface IAnimationEvent
    {
        string Name { get; }

        IEnumerable<string> Path { get; }

        IAnimationManager Source { get; }
    }

    public struct TriggerEvent : IAnimationEvent
    {
        public string Name { get; }

        public IEnumerable<string> Path => _path.Value;

        public Option<object> Argument { get; }

        public IAnimationManager Source { get; }

        private readonly Lazy<IEnumerable<string>> _path;

        public TriggerEvent(string name, Option<object> argument, IAnimationManager source)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Name = name;
            Argument = argument;
            Source = source;

            _path = new Lazy<IEnumerable<string>>(() => name.Split('.'));
        }
    }

    public struct ValueChangeEvent : IAnimationEvent
    {
        public string Name { get; }

        public IEnumerable<string> Path => _path.Value;

        public float Value { get; }

        public IAnimationManager Source { get; }

        private readonly Lazy<IEnumerable<string>> _path;

        public ValueChangeEvent(string name, float value, IAnimationManager source)
        {
            Ensure.That(name, nameof(name)).IsNotNull();
            Ensure.That(source, nameof(source)).IsNotNull();

            Name = name;
            Value = value;
            Source = source;

            _path = new Lazy<IEnumerable<string>>(() => name.Split('.'));
        }
    }
}
