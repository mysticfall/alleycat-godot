using Godot;

namespace AlleyCat.Common
{
    public interface IBounded
    {
        AABB Bounds { get; }
    }
}
