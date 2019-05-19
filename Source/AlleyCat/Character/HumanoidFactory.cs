using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class HumanoidFactory : CharacterFactory<Humanoid, MorphableRace, IPairedEyeSight, ILocomotion>
    {
        protected override Validation<string, Humanoid> CreateService(
            string key,
            string displayName,
            MorphableRace race,
            IPairedEyeSight vision,
            ILocomotion locomotion,
            Skeleton skeleton,
            IActionSet actions,
            IAnimationManager animationManager,
            KinematicBody node,
            ILoggerFactory loggerFactory)
        {
            return new Humanoid(
                key,
                displayName,
                race,
                Sex,
                vision,
                locomotion,
                skeleton,
                animationManager,
                actions,
                Optional(Markers).Flatten(),
                node,
                loggerFactory);
        }
    }
}
