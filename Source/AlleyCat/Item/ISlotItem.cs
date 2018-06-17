using AlleyCat.Condition.Generic;

namespace AlleyCat.Item
{
    public interface ISlotItem : IItem, ISlotConfiguration, IRestricted<ISlotContainer>
    {
    }
}
