using AlleyCat.Event;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Motion
{
    public class ImmediateLocomotionFactory : LocomotionFactory<ImmediateLocomotion, Spatial>
    {
        [Export]
        public ProcessMode ProcessMode { get; set; } = ProcessMode.Idle;

        protected override Validation<string, ImmediateLocomotion> CreateService(
            Spatial target, ILoggerFactory loggerFactory)
        {
            return new ImmediateLocomotion(target, ProcessMode, this, Active, loggerFactory);
        }
    }
}
