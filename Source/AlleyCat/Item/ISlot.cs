using AlleyCat.Common;
using AlleyCat.Condition.Generic;

namespace AlleyCat.Item
{
    public interface ISlot : INamed, IRestricted<ISlotItem>
    {
    }
}
