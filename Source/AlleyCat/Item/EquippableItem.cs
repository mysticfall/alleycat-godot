using System.Collections.Generic;
using System.Linq;
using AlleyCat.Autowire;
using AlleyCat.Common;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    [AutowireContext]
    public class EquippableItem : ItemEntity
    {
        public IReadOnlyDictionary<string, EquipConfiguration> Configurations { get; private set; } =
            Enumerable.Empty<EquipConfiguration>().ToDictionary();

        [Service] private IEnumerable<EquipConfiguration> _configurations;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Configurations = _configurations.ToDictionary();
        }
    }
}
