using System;
using System.Collections.Generic;
using System.Diagnostics;
using AlleyCat.Animation;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class BoneMorphDefinition : RangedMorphDefinition
    {
        public IEnumerable<string> Bones { get; private set; }

        [Export, UsedImplicitly]
        public string Bone { get; private set; }

        [Export, UsedImplicitly]
        public bool Mirrored { get; private set; }

        [Export, UsedImplicitly]
        public BoneMorphType MorphType { get; private set; }

        [Export, UsedImplicitly]
        public Vector3 Modifier { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Debug.Assert(Bone != null, "Bone != null");

            Bones = Mirrored ? new[] {Bone + ".L", Bone + ".R"} : new[] {Bone};
        }

        public override IMorph CreateMorph(IMorphable morphable)
        {
            Ensure.Any.IsNotNull(morphable, nameof(morphable));

            var rig = morphable as IRigged;
            var skeleton = rig?.Skeleton;

            if (skeleton == null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not implement IRigged interface.");
            }

            if (!(rig.AnimationPlayer is PostProcessingAnimationPlayer player))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(morphable),
                    "The specified morphable does not have an associated PostProcessingAnimationPlayer.");
            }

            return new BoneMorph(skeleton, player, this);
        }
    }
}
