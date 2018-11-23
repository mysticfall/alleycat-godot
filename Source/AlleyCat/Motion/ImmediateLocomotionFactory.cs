using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public class ImmediateLocomotionFactory : LocomotionFactory<ImmediateLocomotion, Spatial>
    {
        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        protected override Validation<string, ImmediateLocomotion> CreateService(Spatial target, ILogger logger)
        {
            Ensure.That(target, nameof(target)).IsNotNull();
            Ensure.That(logger, nameof(logger)).IsNotNull();

            return new ImmediateLocomotion(target, ProcessMode, this, Active, logger);
        }
    }
}
