using AlleyCat.Common;
using AlleyCat.Condition.Generic;
using AlleyCat.Game;

namespace AlleyCat.Item
{
    public interface ISlot : IGameResource, INamed, IRestricted<ISlotItem>
    {
    }
}
