using AlleyCat.Event;
using EnsureThat;
using Godot;
using LanguageExt;

namespace AlleyCat.Motion
{
    public class ImmediateLocomotionFactory : LocomotionFactory<ImmediateLocomotion, Spatial>
    {
        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        protected override Validation<string, ImmediateLocomotion> CreateService(Spatial target)
        {
            Ensure.That(target, nameof(target)).IsNotNull();

            return new ImmediateLocomotion(target, ProcessMode, this, Active);
        }
    }
}
