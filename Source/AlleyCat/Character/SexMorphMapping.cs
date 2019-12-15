using System.Collections.Generic;
using AlleyCat.Morph;
using Godot;

namespace AlleyCat.Character
{
    public class SexMorphMapping : Resource
    {
        [Export]
        public Sex Sex { get; set; }

        [Export]
        public IEnumerable<MorphGroupFactory> MorphGroups { get; set; }
    }
}
