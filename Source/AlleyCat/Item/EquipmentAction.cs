using AlleyCat.Autowire;

namespace AlleyCat.Item
{
    public abstract class EquipmentAction : Action.Action
    {
        [Ancestor]
        public Equipment Item { get; private set; }
    }
}
