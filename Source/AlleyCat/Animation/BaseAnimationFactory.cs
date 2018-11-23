using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Animation
{
    [AutowireContext]
    public abstract class BaseAnimationManagerFactory<T> : GameObjectFactory<T> where T : AnimationManager
    {
        [Export]
        public bool Active { get; set; } = true;

        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        [Service]
        public Option<AnimationPlayer> Player { get; set; }

        protected override Validation<string, T> CreateService(ILogger logger)
        {
            return Player
                .ToValidation("Missing the animation player.")
                .Bind(player => CreateService(player, logger));
        }

        protected abstract Validation<string, T> CreateService(AnimationPlayer player, ILogger logger);

        [UsedImplicitly]
        private void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        private void FireEvent(string name, [CanBeNull] object argument) =>
            Service.Iter(s => s.FireEvent(name, Optional(argument)));
    }
}
