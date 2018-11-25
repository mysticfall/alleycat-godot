using System.Collections.Generic;
using AlleyCat.Action;
using AlleyCat.Animation;
using AlleyCat.Character.Morph;
using AlleyCat.Common;
using AlleyCat.Logging;
using AlleyCat.Motion;
using AlleyCat.Sensor;
using Godot;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Character
{
    public class Humanoid : Character<MorphableRace, IPairedEyeSight, ILocomotion>, IHumanoid
    {
        public IMorphSet Morphs { get; }

        public Humanoid(
            string key,
            string displayName,
            MorphableRace race,
            Sex sex,
            IPairedEyeSight vision,
            ILocomotion locomotion,
            Skeleton skeleton,
            IAnimationManager animationManager,
            IEnumerable<IAction> actions,
            IEnumerable<Marker> markers,
            Spatial node,
            ILoggerFactory loggerFactory) : base(
            key,
            displayName,
            race,
            sex,
            vision,
            locomotion,
            skeleton,
            animationManager,
            actions,
            markers,
            node,
            loggerFactory)
        {
            var groups = Race.MorphGroups.Find(Sex).Flatten().Freeze();
            var definitions = groups.Bind(g => g.Definitions);

            var morphs = definitions.Map(d => d.CreateMorph(this)).Freeze();

            if (Logger.IsEnabled(LogLevel.Debug))
            {
                morphs.Iter(m => this.LogDebug("Found morph '{}'.", m));
            }

            Morphs = new MorphSet(groups, morphs).DisposeWith(this);
        }
    }
}
