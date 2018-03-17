using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Character.Morph
{
    public class MorphGroup : Directory<IMorphDefinition>, IMorphGroup
    {
        public string Key => _key ?? Name;

        public virtual string DisplayName => Tr("morph." + Key);

        [Export, UsedImplicitly] private string _key;
    }
}
