using System.Collections.Generic;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Attribute;
using AlleyCat.Autowire;
using AlleyCat.Common;
using AlleyCat.Control;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using LanguageExt;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character
{
    public class HumanoidFactory : CharacterFactory<Humanoid, MorphableRace, IPairedEyeSight, ILocomotion>
    {
        [Service]
        public Option<IPlayerControl> PlayerControl { get; set; }

        protected override void PostConstruct()
        {
            base.PostConstruct();

            if (this.IsPlayer())
            {
                PlayerControl.Iter(c => c.Character = this.OfType<IHumanoid>());
            }
        }

        protected override Validation<string, Humanoid> CreateService(
            string key,
            string displayName,
            MorphableRace race,
            IEnumerable<IAttribute> attributes,
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
                attributes,
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
