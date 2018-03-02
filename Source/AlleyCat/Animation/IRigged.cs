using Godot;

namespace AlleyCat.Animation
{
    public interface IRigged : IAnimatable
    {
        Skeleton Skeleton { get; }
    }
}
