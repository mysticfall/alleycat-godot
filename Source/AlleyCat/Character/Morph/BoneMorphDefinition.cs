using System;
using System.Collections.Generic;
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
        public BoneMorphType MorphType { get; private set; }

        [Export, UsedImplicitly]
        public Vector3 Modifier { get; private set; }

        [Export, UsedImplicitly] private string _bone;

        [Export, UsedImplicitly] private bool _mirrored;

        public override void _Ready()
        {
            base._Ready();

            if (_bone != null)
            {
                Bones = _mirrored ? new[] {_bone + "_L", _bone + "_R"} : new[] {_bone};
            }
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

            return new BoneMorph(skeleton, rig.AnimationManager, this);
        }
    }
}
