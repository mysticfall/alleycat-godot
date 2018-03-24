using AlleyCat.Common;
using Godot;

namespace AlleyCat.Animation
{
    public interface IRigged : IAnimatable, ITransformable
    {
        Skeleton Skeleton { get; }
    }
}
