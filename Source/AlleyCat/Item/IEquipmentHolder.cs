using AlleyCat.Animation;
using AlleyCat.Common;

namespace AlleyCat.Item
{
    public interface IEquipmentHolder : IRigged, IMarkable
    {
        IEquipmentContainer Equipments { get; }
    }
}
