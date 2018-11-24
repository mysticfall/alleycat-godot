using AlleyCat.Animation;
using AlleyCat.Character.Morph;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;
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
            IAnimationManager animationManager,
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
                Actions,
                Markers,
                this,
                loggerFactory);
        }
    }
}
