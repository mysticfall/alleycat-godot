using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Event;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using LanguageExt;
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

        protected override Validation<string, T> CreateService()
        {
            return Player
                .ToValidation("Missing the animation player.")
                .Bind(CreateService);
        }

        protected abstract Validation<string, T> CreateService(AnimationPlayer player);

        [UsedImplicitly]
        private void FireEvent(string name) => FireEvent(name, null);

        [UsedImplicitly]
        private void FireEvent(string name, [CanBeNull] object argument)
        {
            Ensure.That(name, nameof(name)).IsNotNull();

            Service.Iter(s => s.FireEvent(name, Optional(argument)));
        }
    }
}
