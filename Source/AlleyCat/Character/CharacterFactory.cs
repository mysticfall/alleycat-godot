using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Attribute;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Game;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class CharacterFactory<TCharacter, TRace, TVision, TLocomotion> :
        DelegateObjectFactory<TCharacter, KinematicBody>
        where TCharacter : Character<TRace, TVision, TLocomotion>, IDelegateObject<KinematicBody>
        where TRace : Race
        where TVision : class, IVision
        where TLocomotion : class, ILocomotion
    {
        [Export]
        public string Key { get; set; }

        [Export]
        public string DisplayName { get; set; }

        [Export]
        public string Race { get; set; }

        [Export]
        public Sex Sex { get; set; }

        [Service]
        public IEnumerable<IAttribute> Attributes { get; set; }

        [Service]
        public Option<TVision> Vision { get; set; }

        [Service]
        public Option<TLocomotion> Locomotion { get; set; }

        [Service]
        public Option<IAnimationManager> AnimationManager { get; set; }

        [Service]
        public Option<Skeleton> Skeleton { get; set; }

        [Service]
        public Option<IRaceRegistry> RaceRegistry { get; set; }

        [Service]
        public Option<IActionSet> Actions { get; set; }

        [Service(local: true)]
        public IEnumerable<Marker> Markers { get; set; }

        protected override Validation<string, TCharacter> CreateService(KinematicBody node,
            ILoggerFactory loggerFactory)
        {
            var key = Key.TrimToOption().IfNone(() => Name);
            var displayName = DisplayName.TrimToOption().Map(Tr).IfNone(key);

            return
                from skeleton in Skeleton
                    .ToValidation("Failed to find the skeleton.")
                from vision in Vision
                    .ToValidation("Failed to find the vision component.")
                from locomotion in Locomotion
                    .ToValidation("Failed to find the locomotion component.")
                from raceName in Race.TrimToOption()
                    .ToValidation("Missing the race name.")
                from raceRegistry in RaceRegistry
                    .ToValidation("Failed to find the race registry.")
                from race in raceRegistry.Races.Find(raceName).OfType<TRace>().HeadOrNone()
                    .ToValidation($"Unknown race was specified: '{raceName}'.")
                from actions in Actions
                    .ToValidation("Failed to find the action set.")
                from animationManager in AnimationManager
                    .ToValidation("Failed to find the animation manager.")
                from character in CreateService(
                    key, displayName, race, Attributes, vision, locomotion, skeleton, actions,
                    animationManager, node, loggerFactory)
                select character;
        }

        protected abstract Validation<string, TCharacter> CreateService(
            string key,
            string displayName,
            TRace race,
            IEnumerable<IAttribute> attributes,
            TVision vision,
            TLocomotion locomotion,
            Skeleton skeleton,
            IActionSet actions,
            IAnimationManager animationManager,
            KinematicBody node,
            ILoggerFactory loggerFactory);
    }
}
