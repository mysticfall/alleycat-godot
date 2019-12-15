using AlleyCat.Autowire;
using AlleyCat.Event;
using AlleyCat.Game;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    [AutowireContext]
    public abstract class BaseAnimationManagerFactory<T> : GameNodeFactory<T> where T : AnimationManager
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Service]
        public Option<AnimationPlayer> Player { get; set; }

        private const string EventPrefix = "event.";

        protected override Validation<string, T> CreateService(ILoggerFactory loggerFactory)
        {
            return Player
                .ToValidation("Missing the animation player.")
                .Bind(player => CreateService(player, loggerFactory));
        }

        protected abstract Validation<string, T> CreateService(AnimationPlayer player, ILoggerFactory loggerFactory);

        [UsedImplicitly]
        private void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        private void FireEvent(string name, [CanBeNull] object argument) =>
            Service.Iter(s => s.FireEvent(new TriggerEvent(name, Optional(argument), s)));

        public override bool _Set(string property, object value)
        {
            if (property.StartsWith(EventPrefix) && value is float v)
            {
                var name = property.Substring(EventPrefix.Length());

                Service.Iter(s => s.FireEvent(new ValueChangeEvent(name, v, s)));

                return true;
            }

            return base._Set(property, value);
        }
    }
}
