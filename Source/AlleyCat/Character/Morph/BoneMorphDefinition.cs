using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using AlleyCat.Common;
using EnsureThat;
using Godot;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace AlleyCat.Character.Morph
{
    public class BoneMorphDefinition : RangedMorphDefinition
    {
        public IEnumerable<string> Bones { get; }

        public BoneMorphType MorphType { get; }

        public Vector3 Modifier { get; }

        public BoneMorphDefinition(
            string key,
            string displayName,
            IEnumerable<string> bones,
            BoneMorphType morphType,
            Vector3 modifier,
            Range<float> range,
            float defaultValue,
            ILogger logger) : base(key, displayName, range, defaultValue, logger)
        {
            Bones = bones?.Freeze();

            Ensure.Enumerable.HasItems(Bones, nameof(bones));

            MorphType = morphType;
            Modifier = modifier;
        }

        public override IMorph CreateMorph(IMorphable morphable)
        {
            var morph = Optional(morphable)
                .OfType<IRigged>().Map(r => new BoneMorph(r.Skeleton, r.AnimationManager, this))
                .HeadOrNone();

            return morph.IfNone(() => throw new ArgumentOutOfRangeException(nameof(morphable),
                "The specified morphable does not implement IRigged interface."));
        }
    }
}
