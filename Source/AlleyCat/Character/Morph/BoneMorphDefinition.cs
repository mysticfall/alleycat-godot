using System;
using System.Collections.Generic;
using System.Linq;
using AlleyCat.Animation;
using EnsureThat;
using Godot;
using Godot.Collections;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class BoneMorphDefinition : RangedMorphDefinition
    {
        public IEnumerable<string> Bones => _bones ?? Enumerable.Empty<string>();

        [Export, UsedImplicitly]
        public BoneMorphType MorphType { get; private set; }

        [Export, UsedImplicitly]
        public Vector3 Modifier { get; private set; }

        [Export] private Array<string> _bones;

        public override IMorph CreateMorph(IMorphable morphable)
        {
            Ensure.That(morphable, nameof(morphable)).IsNotNull();

            var rig = morphable as IRigged;
            var skeleton = rig?.Skeleton;

            if (skeleton == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not implement IRigged interface.");
            }

            return new BoneMorph(skeleton, rig.AnimationManager, this);
        }
    }
}
