using AlleyCat.Common;
using AlleyCat.Condition.Generic;

namespace AlleyCat.Item
{
    public interface ISlotItem : INamed, IRestricted<ISlotContainer>
    {
        string Slot { get; }
    }
}
