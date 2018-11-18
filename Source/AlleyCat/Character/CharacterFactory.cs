using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Common.Generic;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using EnsureThat;
using Godot;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    [AutowireContext]
    public abstract class CharacterFactory<TCharacter, TRace, TVision, TLocomotion> : KinematicBody,
        IGameObjectFactory<TCharacter>
        where TCharacter : Character<TRace, TVision, TLocomotion>
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
        public Option<TVision> Vision { get; set; }

        [Service]
        public Option<TLocomotion> Locomotion { get; set; }

        [Service]
        public Option<IAnimationManager> AnimationManager { get; set; }

        [Service]
        public Option<Skeleton> Skeleton { get; set; }

        [Service]
        public Option<IRaceRegistry> RaceRegistry { get; set; }

        [Service(local: true)]
        public IEnumerable<IAction> Actions { get; set; } = Seq<IAction>();

        [Service(local: true)]
        public IEnumerable<Marker> Markers { get; set; } = Seq<Marker>();

        public virtual IEnumerable<Type> ProvidedTypes => TypeUtils.FindInjectableTypes<TCharacter>();

        public Validation<string, TCharacter> Service { get; private set; } =
            Fail<string, TCharacter>("The factory has not been initialized yet.");

        Validation<string, object> IGameObjectFactory.Service => Service.Map(v => (object) v);

        protected Validation<string, TCharacter> CreateService()
        {
            var key = Key.TrimToOption().IfNone(GetName);
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
                from animationManager in AnimationManager
                    .ToValidation("Failed to find the animation manager.")
                from character in CreateService(key, displayName, race, vision, locomotion, skeleton, animationManager)
                select character;
        }

        protected abstract Validation<string, TCharacter> CreateService(
            string key,
            string displayName,
            TRace race,
            TVision vision,
            TLocomotion locomotion,
            Skeleton skeleton,
            IAnimationManager animationManager);

        public void AddServices(IServiceCollection collection)
        {
            Ensure.That(collection, nameof(collection)).IsNotNull();

            if (Service.IsSuccess)
            {
                throw new InvalidOperationException("The service has been already created.");
            }

            (Service = CreateService()).BiIter(
                service => ProvidedTypes.Iter(type => collection.AddSingleton(type, service)),
                error => GD.Print(error) // TODO Need a better way to handle errors (i.e. using a logger)
            );
        }

        public override void _Ready()
        {
            base._Ready();

            this.Autowire();
        }

        [PostConstruct]
        protected virtual void PostConstruct() => Service.SuccessAsEnumerable().Iter(s => s.Initialize());

        protected override void Dispose(bool disposing)
        {
            Service.SuccessAsEnumerable().Iter(s => s.DisposeQuietly());
            Service = Fail<string, TCharacter>("The factory has been disposed.");

            base.Dispose(disposing);
        }
    }
}
