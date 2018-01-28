using System;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Autowire
{
    public interface INodeProcessor : IComparable<INodeProcessor>
    {
        AutowirePhase ProcessPhase { get; }

        void Process([NotNull] IAutowireContext context, [NotNull] Node node);
    }
}
