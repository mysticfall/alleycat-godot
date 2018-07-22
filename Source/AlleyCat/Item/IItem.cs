using AlleyCat.Common;

namespace AlleyCat.Item
{
    public interface IItem : INamed
    {
        string Description { get; }
    }
}
