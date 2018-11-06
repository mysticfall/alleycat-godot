using AlleyCat.Autowire;
using AlleyCat.Setting.Project;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace AlleyCat.Motion
{
    public abstract class KinematicLocomotionFactory<TLocomotion> : LocomotionFactory<TLocomotion, KinematicBody>
        where TLocomotion : KinematicLocomotion
    {
        [Export]
        public bool ApplyGravity { get; set; } = true;

        [Service]
        public Option<IOptions<Physics3DSettings>> PhysicsSettings { get; set; }

        protected override Validation<string, TLocomotion> CreateService(KinematicBody target)
        {
            Ensure.That(target, nameof(target)).IsNotNull();

            return PhysicsSettings.Bind(v => Optional(v.Value))
                .ToValidation("Failed to read physics 3D settings.")
                .Bind(settings => CreateService(target, settings));
        }

        protected abstract Validation<string, TLocomotion> CreateService(KinematicBody target,
            Physics3DSettings physicsSettings);
    }
}
