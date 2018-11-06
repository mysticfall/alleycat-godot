using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using LanguageExt;

namespace AlleyCat.Motion
{
    public abstract class LocomotionFactory<TLocomotion, TTarget> : GameObjectFactory<TLocomotion>
        where TLocomotion : ILocomotion
        where TTarget : Spatial
    {
        [Export]
        public bool Active { get; set; } = true;

        [Node]
        public Option<TTarget> Target { get; set; }

        [Export] private NodePath _targetPath;

        protected override Validation<string, TLocomotion> CreateService() =>
            Target.ToValidation("Missing locomotion target.").Bind(CreateService);

        protected abstract Validation<string, TLocomotion> CreateService(TTarget target);
    }
}
