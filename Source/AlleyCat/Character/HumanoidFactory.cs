using AlleyCat.Animation;
using AlleyCat.Character.Morph;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using LanguageExt;

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
            IAnimationManager animationManager)
        {
            Ensure.That(key, nameof(key)).IsNotNullOrEmpty();
            Ensure.That(displayName, nameof(displayName)).IsNotNullOrEmpty();
            Ensure.That(race, nameof(race)).IsNotNull();
            Ensure.That(vision, nameof(vision)).IsNotNull();
            Ensure.That(locomotion, nameof(locomotion)).IsNotNull();
            Ensure.That(skeleton, nameof(skeleton)).IsNotNull();
            Ensure.That(animationManager, nameof(animationManager)).IsNotNull();

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
                this);
        }
    }
}
